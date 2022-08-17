//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
//Blizzless Project 2022 
using FluentNHibernate.Mapping;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Mapper
{
	public class DBGuildMemberMapper : ClassMap<DBGuildMember>
	{
		public DBGuildMemberMapper()
		{
			Table("guild_members");
			Id(e => e.Id).CustomType<PostgresUserTypeNullable>().GeneratedBy.Sequence("guildmembers_seq").UnsavedValue(null);
			References(e => e.DBGuild);
			References(e => e.DBGameAccount);
			Map(e => e.Note).Length(50);
			Map(e => e.Rank);
		}
	}
}
