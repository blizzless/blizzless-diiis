using FluentNHibernate.Data;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Entities
{
	public class DBInventory : Entity
	{
		public new virtual ulong Id { get; set; }
		public virtual DBGameAccount DBGameAccount { get; set; }
		public virtual DBToon DBToon { get; set; }
		public virtual bool ForSale { get; set; }
		public virtual int LocationX { get; set; }
		public virtual int LocationY { get; set; }
		public virtual int EquipmentSlot { get; set; }
		public virtual int HirelingId { get; set; }
		public virtual bool isHardcore { get; set; }
		public virtual bool Unidentified { get; set; }
		public virtual int FirstGem { get; set; }
		public virtual int SecondGem { get; set; }
		public virtual int ThirdGem { get; set; }
		public virtual int Version { get; set; }
		public virtual int GbId { get; set; }
		public virtual int Count { get; set; }
		public virtual byte[] RareItemName { get; set; }
		public virtual int DyeType { get; set; }
		public virtual int Quality { get; set; }
		public virtual int Durability { get; set; }
		public virtual int Binding { get; set; }
		public virtual int Rating { get; set; }
		public virtual string Affixes { get; set; }
		public virtual string Attributes { get; set; }
		public virtual int TransmogGBID { get; set; }

		/*public override bool Equals(object obj)
		{
			var other = obj as DBInventory;
			return (other != null) && (IsTransient ? ReferenceEquals(this, other) : Id == other.Id);
		}
		
		public virtual bool IsTransient { get { return Id == 0; } }
		
		private int? _cachedHashcode; // because Hashcode should not change
		
		public override int GetHashCode()
		{
			if (_cachedHashcode == null)
				_cachedHashcode = IsTransient ? base.GetHashCode() : Id.GetHashCode();

			return _cachedHashcode.Value;
		}*/
	}
}
