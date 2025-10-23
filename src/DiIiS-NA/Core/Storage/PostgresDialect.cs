using System.Data;
using NHibernate.Dialect;

namespace DiIiS_NA.Core.Storage
{
	public class PostgresDialect : PostgreSQL82Dialect
	{
		public PostgresDialect()
		{
			RegisterColumnType(DbType.UInt16, "int2");
			RegisterColumnType(DbType.UInt32, "int4");
			RegisterColumnType(DbType.UInt64, "int8");
		}
	}
}
