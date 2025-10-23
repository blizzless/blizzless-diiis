using FluentNHibernate.Data;

namespace DiIiS_NA.Core.Storage.WorldSceneBase.Entities
{
    public class DRLG_Container : Entity
    {
        public new virtual ulong Id { get; protected set; }
        public virtual long WorldSNO { get; set; }
        public virtual long RangeofScenes { get; set; }
        public virtual long Params { get; set; }
    }
}
