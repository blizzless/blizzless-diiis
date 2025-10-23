using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
using FluentNHibernate.Mapping;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Mapper
{
	public class DBInventoryMapper : ClassMap<DBInventory>
	{
		public DBInventoryMapper()
		{
			Table("items");
			Id(e => e.Id).CustomType<PostgresUserTypeNullable>().GeneratedBy.Sequence("items_seq").UnsavedValue(null);
			References(e => e.DBGameAccount).Nullable();
			References(e => e.DBToon).Nullable();
			Map(e => e.EquipmentSlot);
			Map(e => e.ForSale).Not.Nullable().Default("false");
			Map(e => e.HirelingId).Not.Nullable().Default("0");
			Map(e => e.LocationX);
			Map(e => e.LocationY);
			Map(e => e.isHardcore).Not.Nullable().Default("false");
			Map(e => e.Unidentified).Not.Nullable().Default("false");
			Map(e => e.FirstGem).Not.Nullable().Default("-1");
			Map(e => e.SecondGem).Not.Nullable().Default("-1");
			Map(e => e.ThirdGem).Not.Nullable().Default("-1");
			Map(e => e.GbId);
			Map(e => e.Version).Not.Nullable().Default("1");
			Map(e => e.Count).Default("1");
			Map(e => e.RareItemName).Nullable();
			Map(e => e.DyeType).Default("0");
			Map(e => e.Quality).Default("1");
			Map(e => e.Binding).Default("0");
			Map(e => e.Durability).Default("0");
			Map(e => e.Rating).Default("0");
			Map(e => e.Affixes);
			Map(e => e.Attributes).Length(2500);
			Map(e => e.TransmogGBID).Not.Nullable().Default("-1");
		}
	}
}
