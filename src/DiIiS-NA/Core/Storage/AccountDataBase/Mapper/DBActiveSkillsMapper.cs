using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
using FluentNHibernate.Mapping;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Mapper
{
	public class DBActiveSkillsMapper : ClassMap<DBActiveSkills>
	{
		public DBActiveSkillsMapper()
		{
			Table("skills");
			Id(e => e.Id).CustomType<PostgresUserTypeNullable>().GeneratedBy.Sequence("skills_seq").UnsavedValue(null);
			References(e => e.DBToon);
			Map(e => e.Rune0);
			Map(e => e.Skill0);
			Map(e => e.Rune1);
			Map(e => e.Skill1);
			Map(e => e.Rune2);
			Map(e => e.Skill2);
			Map(e => e.Rune3);
			Map(e => e.Skill3);
			Map(e => e.Rune4);
			Map(e => e.Skill4);
			Map(e => e.Rune5);
			Map(e => e.Skill5);

			Map(e => e.Passive0);
			Map(e => e.Passive1);
			Map(e => e.Passive2);
			Map(e => e.Passive3).Not.Nullable().Default("-1");

			Map(e => e.PotionGBID).Not.Nullable().Default("-1");
		}
	}
}
