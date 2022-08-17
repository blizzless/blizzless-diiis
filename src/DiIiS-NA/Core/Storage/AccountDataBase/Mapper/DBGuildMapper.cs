//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
//Blizzless Project 2022 
using FluentNHibernate.Mapping;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Mapper
{
	public class DBGuildMapper : ClassMap<DBGuild>
	{
		public DBGuildMapper()
		{
			Table("guilds");
			Id(e => e.Id).CustomType<PostgresUserTypeNullable>().GeneratedBy.Sequence("guilds_seq").UnsavedValue(null);
			Map(e => e.Name).Length(50);
			Map(e => e.Tag).Length(6);
			Map(e => e.Description);
			Map(e => e.MOTD);
			Map(e => e.Category);
			Map(e => e.Language);
			Map(e => e.IsLFM);
			Map(e => e.IsInviteRequired);
			Map(e => e.Rating);
			References(e => e.Creator);
			Map(e => e.Ranks);
			Map(e => e.Disbanded);
		}
	}
}
