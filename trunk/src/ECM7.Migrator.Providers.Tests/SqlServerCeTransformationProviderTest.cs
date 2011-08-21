using ECM7.Migrator.Exceptions;

namespace ECM7.Migrator.Providers.Tests
{
	using ECM7.Migrator.Framework;
	using ECM7.Migrator.Providers.SqlServer;

	using NUnit.Framework;

	[TestFixture, Category("SqlServerCe")]
	public class SqlServerCeTransformationProviderTest 
		: TransformationProviderConstraintBase<SqlServerCeTransformationProvider>
	{
		public override string ConnectionStrinSettingsName
		{
			get
			{
				return "SqlServerCeConnectionString";
			}
		}

		public override bool UseTransaction
		{
			get { return true; }
		}

		protected override string BatchSql
		{
			get
			{
				return @"
				insert into [TestTwo] ([Id], [TestId]) values (11, 111)
				GO
				insert into [TestTwo] ([Id], [TestId]) values (22, 222)
				GO
				insert into [TestTwo] ([Id], [TestId]) values (33, 333)
				GO
				insert into [TestTwo] ([Id], [TestId]) values (44, 444)
				GO
				go
				insert into [TestTwo] ([Id], [TestId]) values (55, 555)
				";
			}
		}

		[Test, ExpectedException(typeof(MigrationException))]
		public override void CanAddCheckConstraint()
		{
			base.CanAddCheckConstraint();
		}

		[Test, ExpectedException(typeof(MigrationException))]
		public override void RemoveCheckConstraint()
		{
			base.RemoveCheckConstraint();
		}

		[Test, ExpectedException(typeof(MigrationException))]
		public override void RenameColumnThatExists()
		{
			base.RenameColumnThatExists();
		}
	}
}