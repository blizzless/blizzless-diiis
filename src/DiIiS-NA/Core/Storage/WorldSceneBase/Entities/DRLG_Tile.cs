//Blizzless Project 2022 
using FluentNHibernate.Data;

namespace DiIiS_NA.Core.Storage.WorldSceneBase.Entities
{
    public class DRLG_Tile : Entity
    {
        public new virtual ulong Id { get; protected set; }
        public virtual int Head_Container { get; set; }
        public virtual int Type { get; set; } //0 - Вход, 1 - Выход, 2 - Пути, 3 - Тупики
        public virtual int SNOHandle_Id { get; set; }
        public virtual int SNOLevelArea { get; set; }
        public virtual int SNOMusic { get; set; }
        public virtual int SNOWeather { get; set; }
    }
}
