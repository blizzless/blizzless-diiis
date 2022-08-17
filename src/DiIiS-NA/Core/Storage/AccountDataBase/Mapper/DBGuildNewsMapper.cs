//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
//Blizzless Project 2022 
using FluentNHibernate.Mapping;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Mapper
{
	public class DBGuildNewsMapper : ClassMap<DBGuildNews>
	{
		public DBGuildNewsMapper()
		{
			Table("guild_news");
			Id(e => e.Id).CustomType<PostgresUserTypeNullable>().GeneratedBy.Sequence("guildnews_seq").UnsavedValue(null);
			References(e => e.DBGuild);
			References(e => e.DBGameAccount);
			Map(e => e.Type);
			Map(e => e.Time).CustomType<PostgresUserType>();
			Map(e => e.Data);
		}
	}
}
