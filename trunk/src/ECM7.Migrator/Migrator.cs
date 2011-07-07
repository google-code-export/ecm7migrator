namespace ECM7.Migrator
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Reflection;
	using Framework;
	using Loader;

	using log4net;

	/// <summary>
	/// Migrations mediator.
	/// </summary>
	public class Migrator
	{
		/// <summary>
		/// ���������
		/// </summary>
		private readonly ITransformationProvider provider;

		/// <summary>
		/// ��������� ���������� � ���������
		/// </summary>
		private readonly MigrationAssembly migrationAssembly;

		/// <summary>
		/// ������
		/// </summary>
		private readonly ILog logger;

		/// <summary>
		/// ���� ��� ���������� ��������
		/// </summary>
		private string Key
		{
			get { return migrationAssembly.Key; }
		}

		#region constructors

		/// <summary>
		/// �������������
		/// </summary>
		public Migrator(string dialectTypeName, string connectionString, Assembly asm, ILog logger)
			: this(ProviderFactory.Create(dialectTypeName, connectionString, logger), asm, logger)
		{
		}

		/// <summary>
		/// �������������
		/// </summary>
		public Migrator(ITransformationProvider provider, Assembly asm, ILog logger)
		{
			Require.IsNotNull(logger, "�� ����� ������, ����������� ��������� ILog");
			this.logger = logger;

			Require.IsNotNull(provider, "�� ����� ��������� ����");
			this.provider = provider;

			Require.IsNotNull(asm, "�� ������ ������ � ����������");
			this.migrationAssembly = new MigrationAssembly(asm, logger);
		}

		#endregion

		/// <summary>
		/// Returns registered migration <see cref="System.Type">types</see>.
		/// </summary>
		public ReadOnlyCollection<MigrationInfo> AvailableMigrations
		{
			get { return this.migrationAssembly.MigrationsTypes; }
		}

		/// <summary>
		/// Returns the current migrations applied to the database.
		/// </summary>
		public IList<long> GetAppliedMigrations()
		{
			return provider.GetAppliedMigrations(Key);
		}

		/// <summary>
		/// Get or set the event logger.
		/// </summary>
		public ILog Logger
		{
			get { return logger; }
		}

		/// <summary>
		/// Migrate the database to a specific version.
		/// Runs all migration between the actual version and the
		/// specified version.
		/// If <c>version</c> is greater then the current version,
		/// the <c>Up()</c> method will be invoked.
		/// If <c>version</c> lower then the current version,
		/// the <c>Down()</c> method of previous migration will be invoked.
		/// If <c>dryrun</c> is set, don't write any changes to the database.
		/// </summary>
		/// <param name="databaseVersion">The version that must became the current one</param>
		public void Migrate(long databaseVersion = -1)
		{

			long targetVersion = databaseVersion < 0 ? this.migrationAssembly.LastVersion : databaseVersion;

			IList<long> appliedMigrations = provider.GetAppliedMigrations(Key);
			IList<long> availableMigrations = this.migrationAssembly.MigrationsTypes
				.Select(mInfo => mInfo.Version).ToList();

			MigrationPlan plan = BuildMigrationPlan(targetVersion, appliedMigrations, availableMigrations);

			long currentDatabaseVersion = plan.StartVersion;
			Logger.Started(currentDatabaseVersion, targetVersion);

			foreach (long currentExecutedVersion in plan)
			{
				ExecuteMigration(currentExecutedVersion, currentDatabaseVersion);

				currentDatabaseVersion = currentExecutedVersion;
			}
		}

		/// <summary>
		/// ���������� ��������
		/// </summary>
		/// <param name="targetVersion">������ ����������� ��������</param>
		/// <param name="currentDatabaseVersion">������� ������ ��</param>
		public void ExecuteMigration(long targetVersion, long currentDatabaseVersion)
		{
			IMigration migration = this.migrationAssembly.InstantiateMigration(targetVersion, this.provider);
			Require.IsNotNull(migration, "�� ������� �������� ������ {0}", targetVersion);

			try
			{
				this.provider.BeginTransaction();

				if (targetVersion <= currentDatabaseVersion)
				{
					this.logger.MigrateDown(targetVersion, migration.Name);
					migration.Down();
					this.provider.MigrationUnApplied(targetVersion, Key);
				}
				else
				{
					this.logger.MigrateUp(targetVersion, migration.Name);
					migration.Up();
					this.provider.MigrationApplied(targetVersion, Key);
				}

				this.provider.Commit();
			}
			catch (Exception ex)
			{
				this.Logger.Exception(targetVersion, migration.Name, ex);

				// ��� ������ ���������� ���������
				this.provider.Rollback();
				this.Logger.RollingBack(currentDatabaseVersion);
				throw;
			}
		}

		/// <summary>
		/// �������� ������ ������ ��� ����������
		/// </summary>
		/// <param name="target">������ ����������</param>
		/// <param name="appliedMigrations">������ ������ ����������� ��������</param>
		/// <param name="availableMigrations">������ ������ ��������� ��������</param>
		public static MigrationPlan BuildMigrationPlan(long target, IEnumerable<long> appliedMigrations, IEnumerable<long> availableMigrations)
		{
			long startVersion = appliedMigrations.IsEmpty() ? 0 : appliedMigrations.Max();
			HashSet<long> set = new HashSet<long>(appliedMigrations);

			// ��������
			var list = availableMigrations.Where(x => x < startVersion && !set.Contains(x));
			if (!list.IsEmpty())
			{
				throw new VersionException(
					"�������� ������������� ��������, ������ ������� ������ ������� ������ ��", list);
			}

			set.UnionWith(availableMigrations);

			var versions = target < startVersion
							? set.Where(n => n <= startVersion && n > target).OrderByDescending(x => x).ToList()
							: set.Where(n => n > startVersion && n <= target).OrderBy(x => x).ToList();

			return new MigrationPlan(versions, startVersion);
		}
	}
}