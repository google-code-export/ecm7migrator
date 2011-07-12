
using System;
using System.Data;
using ECM7.Migrator.Framework;

namespace ECM7.Migrator.Providers.SqlServer
{
	public class SqlServerDialect : Dialect
	{
		public SqlServerDialect()
		{
			RegisterColumnType(DbType.AnsiStringFixedLength, "CHAR(255)");
			RegisterColumnType(DbType.AnsiStringFixedLength, 8000, "CHAR($l)");
			RegisterColumnType(DbType.AnsiString, "VARCHAR(255)");
			RegisterColumnType(DbType.AnsiString, 8000, "VARCHAR($l)");
			RegisterColumnType(DbType.AnsiString, int.MaxValue, "TEXT");
			RegisterColumnType(DbType.Binary, "VARBINARY(8000)");
			RegisterColumnType(DbType.Binary, 8000, "VARBINARY($l)");
			RegisterColumnType(DbType.Binary, 2147483647, "IMAGE");
			RegisterColumnType(DbType.Boolean, "BIT");
			RegisterColumnType(DbType.Byte, "TINYINT");
			RegisterColumnType(DbType.Currency, "MONEY");
			RegisterColumnType(DbType.Date, "DATETIME");
			RegisterColumnType(DbType.DateTime, "DATETIME");
			RegisterColumnType(DbType.Decimal, "DECIMAL");
			RegisterColumnType(DbType.Decimal, 38, "DECIMAL($l, $s)", 2);
			RegisterColumnType(DbType.Double, "DOUBLE PRECISION"); //synonym for FLOAT(53)
			RegisterColumnType(DbType.Guid, "UNIQUEIDENTIFIER");
			RegisterColumnType(DbType.Int16, "SMALLINT");
			RegisterColumnType(DbType.Int32, "INT");
			RegisterColumnType(DbType.Int64, "BIGINT");
			RegisterColumnType(DbType.Single, "REAL"); //synonym for FLOAT(24) 
			RegisterColumnType(DbType.StringFixedLength, "NCHAR(255)");
			RegisterColumnType(DbType.StringFixedLength, 4000, "NCHAR($l)");
			RegisterColumnType(DbType.String, "NVARCHAR(255)");
			RegisterColumnType(DbType.String, 4000, "NVARCHAR($l)");
			RegisterColumnType(DbType.String, 1073741823, "NTEXT");
			RegisterColumnType(DbType.Time, "DATETIME");

			RegisterProperty(ColumnProperty.Identity, "IDENTITY");
		}

		public override Type TransformationProviderType { get { return typeof(SqlServerTransformationProvider); } }

		public override bool SupportsIndex
		{
			get { return false; }
		}

		public override bool NamesNeedsQuote
		{
			get { return true; }
		}

		public override string NamesQuoteTemplate
		{
			get { return "[{0}]"; }
		}

		public override string Default(object defaultValue)
		{
			if (defaultValue.GetType().Equals(typeof(bool)))
			{
				defaultValue = ((bool)defaultValue) ? 1 : 0;
			}

			return String.Format("DEFAULT {0}", defaultValue);
		}
	}
}
