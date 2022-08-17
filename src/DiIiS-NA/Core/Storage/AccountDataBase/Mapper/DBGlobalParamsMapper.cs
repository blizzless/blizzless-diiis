//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
//Blizzless Project 2022 
using FluentNHibernate.Mapping;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Mapper
{
	public class DBGlobalParamsMapper : ClassMap<DBGlobalParams>
	{
		public DBGlobalParamsMapper()
		{
			Table("global_params");
			Id(e => e.Id).CustomType<PostgresUserTypeNullable>().GeneratedBy.Sequence("global_params_seq").UnsavedValue(null);
			Map(e => e.Name);
			Map(e => e.Value).CustomType<PostgresUserType>();
		}
	}
}
