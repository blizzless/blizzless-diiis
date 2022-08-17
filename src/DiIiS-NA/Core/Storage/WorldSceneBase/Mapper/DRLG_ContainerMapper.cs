//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.WorldSceneBase.Entities;
//Blizzless Project 2022 
using FluentNHibernate.Mapping;

namespace DiIiS_NA.Core.Storage.WorldSceneBase.Mapper
{
    public class DRLG_ContainerMapper : ClassMap<DRLG_Container>
    {
        public DRLG_ContainerMapper()
        {
            Id(e => e.Id).GeneratedBy.Native();
            Map(e => e.WorldSNO);
            Map(e => e.RangeofScenes);
            Map(e => e.Params);
        }
    }
}
