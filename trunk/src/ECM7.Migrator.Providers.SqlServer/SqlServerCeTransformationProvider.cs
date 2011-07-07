#region License

//The contents of this file are subject to the Mozilla Public License
//Version 1.1 (the "License"); you may not use this file except in
//compliance with the License. You may obtain a copy of the License at
//http://www.mozilla.org/MPL/
//Software distributed under the License is distributed on an "AS IS"
//basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
//License for the specific language governing rights and limitations
//under the License.

#endregion

using System;
using System.Data;
using System.Data.SqlServerCe;
using ECM7.Migrator.Framework;

namespace ECM7.Migrator.Providers.SqlServer
{
	using log4net;

	/// <summary>
	/// Migration transformations provider for Microsoft SQL Server.
	/// </summary>
	public class SqlServerCeTransformationProvider : SqlServerTransformationProvider
	{
		public SqlServerCeTransformationProvider(Dialect dialect, string connectionString, ILog logger)
			: base(dialect, new SqlCeConnection(connectionString), logger)
		{
		}

		public override bool ConstraintExists(string table, string name)
		{
			using (IDataReader reader =
				ExecuteQuery(string.Format("SELECT cont.constraint_name FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS cont WHERE cont.Constraint_Name='{0}'", name)))
			{
				return reader.Read();
			}
		}

		public override void RenameColumn(string tableName, string oldColumnName, string newColumnName)
		{
			if (ColumnExists(tableName, newColumnName))
				throw new MigrationException(String.Format("Table '{0}' has column named '{1}' already", tableName, newColumnName));

			if (ColumnExists(tableName, oldColumnName))
			{
				Column column = GetColumnByName(tableName, oldColumnName);

				AddColumn(tableName, new Column(newColumnName, column.ColumnType, column.ColumnProperty, column.DefaultValue));
				ExecuteNonQuery(string.Format("UPDATE {0} SET {1}={2}", tableName, newColumnName, oldColumnName));
				RemoveColumn(tableName, oldColumnName);
			}
		}

		// Not supported by SQLCe when we have a better schemadumper which gives the exact sql construction including constraints we may use it to insert into a new table and then drop the old table...but this solution is dangerous for big tables.
		public override void RenameTable(string oldName, string newName)
		{
			throw new MigrationException("SqlServerCe doesn't support table renaming");
		}

		public override void AddCheckConstraint(string name, string table, string checkSql)
		{
			throw new MigrationException("SqlServerCe doesn't support check constraints");
		}

		public override bool IndexExists(string indexName, string tableName)
		{
			string sql = string.Format(
				"select count(*) from INFORMATION_SCHEMA.INDEXES where lower(TABLE_NAME) = '{0}' and lower(INDEX_NAME) = '{1}'",
				Dialect.QuoteIfNeeded(tableName), Dialect.QuoteIfNeeded(indexName));

			int count = Convert.ToInt32(ExecuteScalar(sql));
			return count > 0;
		}

		public override void RemoveIndex(string indexName, string tableName)
		{
			if (!IndexExists(indexName, tableName))
			{
				Logger.WarnFormat("Index {0} is not exists", indexName);
				return;
			}

			string sql = string.Format("DROP INDEX {0}.{1}",
					Dialect.QuoteIfNeeded(tableName),
					Dialect.QuoteIfNeeded(indexName));

			ExecuteNonQuery(sql);

		}

		protected override string FindConstraints(string table, string column)
		{
			return
				string.Format("SELECT cont.constraint_name FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE cont "
					+ "WHERE cont.Table_Name='{0}' AND cont.column_name = '{1}'",
					table, column);
		}
	}
}