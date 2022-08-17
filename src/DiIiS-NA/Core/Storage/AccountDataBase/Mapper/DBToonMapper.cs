//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
//Blizzless Project 2022 
using FluentNHibernate.Mapping;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Mapper
{
	public class DBToonMapper : ClassMap<DBToon>
	{
		public DBToonMapper()
		{
			Table("toons");
			Id(e => e.Id).CustomType<PostgresUserTypeNullable>().GeneratedBy.Sequence("toons_seq").UnsavedValue(null);
			Map(e => e.Class);
			References(e => e.DBGameAccount);
			Map(e => e.Deleted);
			Map(e => e.isHardcore);
			Map(e => e.isSeasoned);
			Map(e => e.Dead);
			Map(e => e.TimeDeadHarcode);
			Map(e => e.StoneOfPortal);
			Map(e => e.CreatedSeason);
			Map(e => e.Experience);
			Map(e => e.ParagonBonuses).CustomSqlType("Bytea");
			Map(e => e.PvERating).Not.Nullable().Default("0");
			Map(e => e.ChestsOpened).Not.Nullable().Default("0");
			Map(e => e.EventsCompleted).Not.Nullable().Default("0");
			Map(e => e.Kills).Not.Nullable().Default("0");
			Map(e => e.Deaths).Not.Nullable().Default("0");
			Map(e => e.ElitesKilled).Not.Nullable().Default("0");
			Map(e => e.GoldGained).Not.Nullable().Default("0");
			Map(e => e.ActiveHireling);
			Map(e => e.CurrentAct);
			Map(e => e.CurrentQuestId);
			Map(e => e.CurrentQuestStepId);
			Map(e => e.CurrentDifficulty);
			Map(e => e.Flags);
			Map(e => e.Level);
			Map(e => e.Stats).Not.Nullable().Default("'0;0;0;0;0;0'");
			Map(e => e.Name);
			Map(e => e.TimePlayed);
			Map(e => e.Lore).Nullable().Default("");
			Map(e => e.Archieved).Not.Nullable().Default("false");
			Map(e => e.WingsActive).Not.Nullable().Default("-1");
			Map(e => e.Cosmetic1);
			Map(e => e.Cosmetic2);
			Map(e => e.Cosmetic3);
			Map(e => e.Cosmetic4);
			Map(e => e.CraftItem1);
			Map(e => e.CraftItem2);
			Map(e => e.CraftItem3);
			Map(e => e.CraftItem4);
			Map(e => e.CraftItem5);
			Map(e => e.BigPortalKey);
			Map(e => e.LeorikKey);
			Map(e => e.HoradricA1);
			Map(e => e.HoradricA2);
			Map(e => e.HoradricA3);
			Map(e => e.HoradricA4);
			Map(e => e.HoradricA5);
		}
	}
}
