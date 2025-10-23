using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
using FluentNHibernate.Mapping;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Mapper
{
	public class DBHirelingMapper : ClassMap<DBHireling>
	{
		public DBHirelingMapper()
		{
			Table("hireling_data");
			Id(e => e.Id).CustomType<PostgresUserTypeNullable>().GeneratedBy.Sequence("hireling_data_seq").UnsavedValue(null);
			References(e => e.DBToon);
			Map(e => e.Class);
			Map(e => e.Skill1SNOId).Not.Nullable().Default("-1");
			Map(e => e.Skill2SNOId).Not.Nullable().Default("-1");
			Map(e => e.Skill3SNOId).Not.Nullable().Default("-1");
			Map(e => e.Skill4SNOId).Not.Nullable().Default("-1");
		}
	}
}
