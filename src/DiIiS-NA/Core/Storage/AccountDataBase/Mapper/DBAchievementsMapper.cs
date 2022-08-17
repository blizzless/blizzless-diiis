//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
//Blizzless Project 2022 
using FluentNHibernate.Mapping;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Mapper
{
	public class DBAchievementsMapper : ClassMap<DBAchievements>
	{
		public DBAchievementsMapper()
		{
			Table("achievements");
			Id(e => e.Id).CustomType<PostgresUserTypeNullable>().GeneratedBy.Sequence("achievements_seq").UnsavedValue(null);
			References(e => e.DBGameAccount).Nullable();
			Map(e => e.AchievementId).CustomType<PostgresUserType>();
			Map(e => e.CompleteTime);
			Map(e => e.IsHardcore).Not.Nullable().Default("false");
			Map(e => e.Quantity).Default("0");
			Map(e => e.Criteria);
			//Map(e => e.Criterias).Not.Nullable().Default("");
		}
	}
}
