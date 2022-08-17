//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
//Blizzless Project 2022 
using FluentNHibernate.Mapping;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Mapper
{
	public class DBQuestHistoryMapper : ClassMap<DBQuestHistory>
	{
		public DBQuestHistoryMapper()
		{
			Table("quests");
			Id(e => e.Id).CustomType<PostgresUserTypeNullable>().GeneratedBy.Sequence("quests_seq").UnsavedValue(null);
			References(e => e.DBToon).Nullable();
			Map(e => e.QuestId);
			Map(e => e.isCompleted).Not.Nullable().Default("false");
			Map(e => e.QuestStep);
		}
	}
}
