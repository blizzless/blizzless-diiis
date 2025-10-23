using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
using FluentNHibernate.Mapping;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Mapper
{
	public class DBCraftMapper : ClassMap<DBCraft>
	{
		public DBCraftMapper()
		{
			Table("craft_data");
			Id(e => e.Id).CustomType<PostgresUserTypeNullable>().GeneratedBy.Sequence("craft_data_seq").UnsavedValue(null);
			References(e => e.DBGameAccount);
			Map(e => e.isHardcore);
			Map(e => e.isSeasoned);
			Map(e => e.Artisan);
			Map(e => e.Level);
			Map(e => e.LearnedRecipes).Not.Nullable().Default("");
		}
	}
}
