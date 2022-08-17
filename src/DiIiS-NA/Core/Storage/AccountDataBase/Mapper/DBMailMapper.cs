//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
//Blizzless Project 2022 
using FluentNHibernate.Mapping;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Mapper
{
	public class DBMailMapper : ClassMap<DBMail>
	{
		public DBMailMapper()
		{
			Table("mail");
			Id(e => e.Id).CustomType<PostgresUserTypeNullable>().GeneratedBy.Sequence("mail_seq").UnsavedValue(null);
			References(e => e.DBToon);
			Map(e => e.Claimed);
			Map(e => e.Title);
			Map(e => e.Body);
			Map(e => e.ItemGBID);
		}
	}
}
