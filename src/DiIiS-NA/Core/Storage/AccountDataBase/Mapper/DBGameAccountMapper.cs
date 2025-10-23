using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
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
			Map(e => e.LastOnline).CustomType<PostgresUserType>();
			References(e => e.LastPlayedHero).Nullable();
			Map(e => e.Flags);
			Map(e => e.Banner).CustomSqlType("Bytea");
			Map(e => e.UIPrefs).CustomSqlType("Bytea");
			Map(e => e.SeenTutorials).CustomSqlType("Bytea");
			Map(e => e.BossProgress).CustomSqlType("Bytea");
			Map(e => e.StashIcons).CustomSqlType("Bytea");
			Map(e => e.ParagonLevel);
			Map(e => e.ParagonLevelHardcore);
			Map(e => e.Experience);
			Map(e => e.ExperienceHardcore);
			//HasMany(e => e.DBToons).Cascade.All();
			//HasMany(e => e.DBInventories).Cascade.All();
			Map(e => e.Gold).CustomType<PostgresUserType>();
			Map(e => e.HardcoreGold).CustomType<PostgresUserType>();
			Map(e => e.Platinum);
			Map(e => e.HardPlatinum);
			Map(e => e.RmtCurrency).CustomType<PostgresUserType>();
			Map(e => e.HardRmtCurrency).CustomType<PostgresUserType>();
			Map(e => e.BloodShards);
			Map(e => e.HardcoreBloodShards);
			Map(e => e.StashSize);
			Map(e => e.HardcoreStashSize);
			Map(e => e.SeasonStashSize);
			Map(e => e.HardSeasonStashSize);
			Map(e => e.ElitesKilled).CustomType<PostgresUserType>();
			Map(e => e.HardElitesKilled).CustomType<PostgresUserType>();
			Map(e => e.TotalKilled).CustomType<PostgresUserType>();
			Map(e => e.HardTotalKilled).CustomType<PostgresUserType>();
			Map(e => e.TotalGold).CustomType<PostgresUserType>();
			Map(e => e.HardTotalGold).CustomType<PostgresUserType>();
			Map(e => e.TotalBloodShards);
			Map(e => e.HardTotalBloodShards);
			Map(e => e.TotalBounties).Not.Nullable().Default("0");
			Map(e => e.TotalBountiesHardcore).Not.Nullable().Default("0");
			Map(e => e.PvPTotalKilled).CustomType<PostgresUserType>();
			Map(e => e.HardPvPTotalKilled).CustomType<PostgresUserType>();
			Map(e => e.PvPTotalWins).CustomType<PostgresUserType>();
			Map(e => e.HardPvPTotalWins).CustomType<PostgresUserType>();
			Map(e => e.PvPTotalGold).CustomType<PostgresUserType>();
			Map(e => e.HardPvPTotalGold).CustomType<PostgresUserType>();
			Map(e => e.CraftItem1);
			Map(e => e.HardCraftItem1);
			Map(e => e.CraftItem2);
			Map(e => e.HardCraftItem2);
			Map(e => e.CraftItem3);
			Map(e => e.HardCraftItem3);
			Map(e => e.CraftItem4);
			Map(e => e.HardCraftItem4);
			Map(e => e.CraftItem5);
			Map(e => e.HardCraftItem5);
			Map(e => e.BigPortalKey);
			Map(e => e.HardBigPortalKey);
			Map(e => e.LeorikKey);
			Map(e => e.HardLeorikKey);
			Map(e => e.VialofPutridness);
			Map(e => e.HardVialofPutridness);
			Map(e => e.IdolofTerror);
			Map(e => e.HardIdolofTerror);
			Map(e => e.HeartofFright);
			Map(e => e.HardHeartofFright);
			Map(e => e.HoradricA1);
			Map(e => e.HardHoradricA1);
			Map(e => e.HoradricA2);
			Map(e => e.HardHoradricA2);
			Map(e => e.HoradricA3);
			Map(e => e.HardHoradricA3);
			Map(e => e.HoradricA4);
			Map(e => e.HardHoradricA4);
			Map(e => e.HoradricA5);
			Map(e => e.HardHoradricA5);
		}
	}
}
