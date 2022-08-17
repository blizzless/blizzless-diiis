//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
//Blizzless Project 2022 
using FluentNHibernate.Mapping;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Mapper
{
	public class DBReportMapper : ClassMap<DBReport>
	{
		public DBReportMapper()
		{
			Table("reports");
			Id(e => e.Id).CustomType<PostgresUserTypeNullable>().GeneratedBy.Sequence("reports_seq").UnsavedValue(null);
			Map(e => e.Type);
			References(e => e.DBGameAccount);
			References(e => e.DBToon);
			References(e => e.Sender);
			Map(e => e.Note);
		}
	}
}
