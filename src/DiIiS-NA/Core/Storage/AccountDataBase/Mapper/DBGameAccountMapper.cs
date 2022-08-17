//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
//Blizzless Project 2022 
using FluentNHibernate.Mapping;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Mapper
{
	public class DBGameAccountMapper : ClassMap<DBGameAccount>
	{
		public DBGameAccountMapper()
		{
			Table("game_accounts");
			Id(e => e.Id).CustomType<PostgresUserTypeNullable>().GeneratedBy.Sequence("game_accounts_seq").UnsavedValue(null);
			References(e => e.DBAccount);
			Map(e => e.Banner).CustomSqlType("Bytea");
			Map(e => e.UIPrefs).CustomSqlType("Bytea");
			Map(e => e.UISettings).CustomSqlType("Bytea");
			Map(e => e.SeenTutorials).CustomSqlType("Bytea");
			Map(e => e.BossProgress).CustomSqlType("Bytea");
			Map(e => e.StashIcons).CustomSqlType("Bytea");
			Map(e => e.LastOnline).CustomType<PostgresUserType>();
			Map(e => e.Flags);
			Map(e => e.ParagonLevel);
			Map(e => e.ParagonLevelHardcore);
			Map(e => e.Experience);
			Map(e => e.ExperienceHardcore);
			//HasMany(e => e.DBToons).Cascade.All();
			//HasMany(e => e.DBInventories).Cascade.All();
			References(e => e.LastPlayedHero).Nullable();
			Map(e => e.Gold).CustomType<PostgresUserType>();
			Map(e => e.Platinum);
			Map(e => e.HardcoreGold).CustomType<PostgresUserType>();
			Map(e => e.RmtCurrency).CustomType<PostgresUserType>();
			Map(e => e.BloodShards);
			Map(e => e.HardcoreBloodShards);
			Map(e => e.StashSize);
			Map(e => e.HardcoreStashSize);
			Map(e => e.SeasonStashSize);
			Map(e => e.ElitesKilled).CustomType<PostgresUserType>();
			Map(e => e.TotalKilled).CustomType<PostgresUserType>();
			Map(e => e.TotalGold).CustomType<PostgresUserType>();
			Map(e => e.TotalBloodShards);
			Map(e => e.TotalBounties).Not.Nullable().Default("0");
			Map(e => e.TotalBountiesHardcore).Not.Nullable().Default("0");
			Map(e => e.PvPTotalKilled).CustomType<PostgresUserType>();
			Map(e => e.PvPTotalWins).CustomType<PostgresUserType>();
			Map(e => e.PvPTotalGold).CustomType<PostgresUserType>();
		}
	}
}
