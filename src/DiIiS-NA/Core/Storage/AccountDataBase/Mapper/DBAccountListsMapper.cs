using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
using FluentNHibernate.Mapping;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Mapper
{
	public class DBAccountListsMapper : ClassMap<DBAccountLists>
	{
		public DBAccountListsMapper()
		{
			Table("account_relations");
			Id(e => e.Id).CustomType<PostgresUserTypeNullable>().GeneratedBy.Sequence("account_relations_seq").UnsavedValue(null);
			References(e => e.ListOwner);
			References(e => e.ListTarget);
			Map(e => e.Type).Not.Nullable().Default("'FRIEND'");
		}
	}
}
