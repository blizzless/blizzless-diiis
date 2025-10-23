using DiIiS_NA.Core.Storage.WorldSceneBase.Entities;
using FluentNHibernate.Mapping;

namespace DiIiS_NA.Core.Storage.WorldSceneBase.Mapper
{
    public class DRLG_TileMapper : ClassMap<DRLG_Tile>
    {
        public DRLG_TileMapper()
        {
            Id(e => e.Id).GeneratedBy.Native();
            Map(e => e.Head_Container);
            Map(e => e.Type);
            Map(e => e.SNOHandle_Id);
            Map(e => e.SNOLevelArea);
            Map(e => e.SNOMusic);
            Map(e => e.SNOWeather);
        }
    }
}
