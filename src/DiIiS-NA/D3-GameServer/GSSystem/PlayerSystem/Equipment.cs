using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;

namespace DiIiS_NA.GameServer.GSSystem.PlayerSystem
{
	public enum EquipmentSlotId
	{
		Inventory = 0, Helm = 1, Chest = 2, Off_Hand = 3, Main_Hand = 4, Hands = 5, Belt = 6, Feet = 7,
		Shoulders = 8, Legs = 9, Bracers = 10, Ring_right = 11, Ring_left = 12, Neck = 13,
		VendorBuyback = 14, Stash = 15, Gold = 16, StashSize = 17, Vendor = 18, Item_Sockets = 20,
		Hireling_RH = 21, Hireling_LH = 22, Hireling_relic = 23, Hireling_Amulet = 24, Hireling_LRing = 25, Hireling_RRing = 26, Hireling_Helm = 27, Hireling_Body = 28,
		Hireling_Hands = 29, Hireling_Belt = 30, Hireling_Feet = 31, Hireling_Shoulders = 32, Hireling_Legs = 33, Hireling_Bracers = 34
	}

	class Equipment : IRevealable
	{
		static readonly Logger Logger = LogManager.CreateLogger();

		public int EquipmentSlots { get { return _equipment.GetLength(0); } }
		public Dictionary<uint, Item> Items { get; private set; }
		private readonly Player _owner; 

		private uint[] _equipment;    

		public bool EquipmentChanged = false;

		public Equipment(Player owner)
		{
			_equipment = new uint[33];
			_owner = owner;
			Items = new Dictionary<uint, Item>();
		}

		/// <summary>
		/// Equips an item in an equipment slot
		/// </summary>
		public void EquipItem(Item item, int slot, bool save = true)
		{
			if (item == null) return;

			_equipment[slot] = item.GlobalID;
			if (!Items.ContainsKey(item.GlobalID))
				Items.Add(item.GlobalID, item);
			item.Owner = _owner;
			item.Attributes[GameAttribute.Item_Equipped] = true; // Probaly should be handled by Equipable class /fasbat
			item.Attributes.SendChangedMessage(_owner.InGameClient);
			item.SetInventoryLocation(slot, 0, 0);
			if (save)
				_owner.Inventory.ChangeItemSlotDB(slot, item);

			EquipmentChanged = true;

			if (item.Attributes[GameAttribute.Item_Quality_Level] > 5)
			{
				_owner.GrantAchievement(74987243307150);
				if (item.Attributes[GameAttribute.Item_Quality_Level] > 7)
					_owner.GrantAchievement(74987243307151);
			}

			if (item.Attributes[GameAttribute.Sockets] > 0)
			{
				if (ItemGroup.IsSubType(item.ItemType, "Helm"))
					_owner.GrantCriteria(74987243307188);
				if (ItemGroup.IsSubType(item.ItemType, "ChestArmor"))
					_owner.GrantCriteria(74987243309924);
				if (ItemGroup.IsSubType(item.ItemType, "Legs"))
					_owner.GrantCriteria(74987243309925);
				if (ItemGroup.IsSubType(item.ItemType, "Weapon"))
					_owner.GrantCriteria(74987243309926);
				if (ItemGroup.IsSubType(item.ItemType, "Amulet"))
					_owner.GrantCriteria(74987243309927);
				if (ItemGroup.IsSubType(item.ItemType, "Ring"))
					_owner.GrantCriteria(74987243309928);
			}
		}

		public void UpdateEqId(int slot, uint newId)
		{
			_equipment[slot] = newId;
		}

		/*public void EquipItem(uint itemID, int slot, bool save = true)
		{
			EquipItem(_owner.Inventory.GetItem(itemID), slot, save);
		}*/

		/// <summary>
		/// Removes an item from the equipment slot it uses
		/// returns the used equipmentSlot
		/// </summary>
		public int UnequipItem(Item item)
		{
			if (!Items.ContainsKey(item.GlobalID))
				return 0;
			Items.Remove(item.GlobalID);

			var slot = item.EquipmentSlot;
			if (_equipment[slot] == item.GlobalID)
			{
				_equipment[slot] = 0;
				item.Attributes[GameAttribute.Item_Equipped] = false; // Probaly should be handled by Equipable class /fasbat
				item.Unreveal(_owner);
				item.Reveal(_owner);
				EquipmentChanged = true;
				return slot;
			}

			return 0;
		}

		/// <summary>
		/// Returns whether an item is equipped
		/// </summary>
		public bool IsItemEquipped(uint itemID)
		{
			return Items.ContainsKey(itemID);
		}

		public bool IsItemEquipped(Item item)
		{
			return IsItemEquipped(item.GlobalID);
		}

		private VisualItem GetEquipmentItem(EquipmentSlotId equipSlot)
		{
			if (_equipment[(int)equipSlot] == 0)
			{
				return new VisualItem()
				{
					GbId = -1, // 0 causes error logs on the client  - angerwin
					DyeType = 0,
					ItemEffectType = 0,
					EffectLevel = 0,
				};
			}

			return Items[(_equipment[(int)equipSlot])].CreateVisualItem();
		}

		private D3.Hero.VisualItem GetEquipmentItemForToon(EquipmentSlotId equipSlot)
		{
			if (_equipment[(int)equipSlot] == 0)
			{
				return D3.Hero.VisualItem.CreateBuilder()
					.SetGbid(-1)
					.SetDyeType(0)
					.SetEffectLevel(0)
					.SetItemEffectType(-1)
					.Build();
			}

			return Items[(_equipment[(int)equipSlot])].GetVisualItem();
		}
		public VisuaCosmeticItem[] GetVisualCosmeticEquipment()
		{
			return new VisuaCosmeticItem[4]
					{
						new VisuaCosmeticItem()
						{
							GbId = _owner.Toon.Cosmetic1
						},
						new VisuaCosmeticItem()
						{
							GbId = _owner.Toon.Cosmetic2
						},
						new VisuaCosmeticItem()
						{
							GbId = _owner.Toon.Cosmetic3
						},
						new VisuaCosmeticItem()
						{
							GbId = _owner.Toon.Cosmetic4
						}
					};
		}
		public VisualItem[] GetVisualEquipment()
		{
			return new VisualItem[8]
			{
				GetEquipmentItem(EquipmentSlotId.Helm),
				GetEquipmentItem(EquipmentSlotId.Chest),
				GetEquipmentItem(EquipmentSlotId.Feet),
				GetEquipmentItem(EquipmentSlotId.Hands),
				GetEquipmentItem(EquipmentSlotId.Main_Hand),
				GetEquipmentItem(EquipmentSlotId.Off_Hand),
				GetEquipmentItem(EquipmentSlotId.Shoulders),
				GetEquipmentItem(EquipmentSlotId.Legs),
			};
		}

		public D3.Hero.VisualEquipment GetVisualEquipmentForToon()
		{
			var visualItems = new[]
			{
					GetEquipmentItemForToon(EquipmentSlotId.Helm),
					GetEquipmentItemForToon(EquipmentSlotId.Chest),
					GetEquipmentItemForToon(EquipmentSlotId.Feet),
					GetEquipmentItemForToon(EquipmentSlotId.Hands),
					GetEquipmentItemForToon(EquipmentSlotId.Main_Hand),
					GetEquipmentItemForToon(EquipmentSlotId.Off_Hand),
					GetEquipmentItemForToon(EquipmentSlotId.Shoulders),
					GetEquipmentItemForToon(EquipmentSlotId.Legs)
			};
			return D3.Hero.VisualEquipment.CreateBuilder().AddRangeVisualItem(visualItems).Build();
		}

		internal Item GetEquipment(int targetEquipSlot)
		{
			//Logger.Debug("GetEquipment Slot: {0}", targetEquipSlot);
			return GetItem(_equipment[targetEquipSlot]);
		}

		internal Item GetEquipment(EquipmentSlotId targetEquipSlot)
		{
			return GetEquipment((int)targetEquipSlot);
		}

		public Item GetWeapon()
		{
			return GetEquipment(4);
		}

		public Item GetOffHand()
		{
			return GetEquipment(3);
		}

		public bool Reveal(Player player)
		{
			foreach (var item in Items.Values)
			{
				item.Reveal(player);
			}
			return true;
		}

		public bool Unreveal(Player player)
		{
			foreach (var item in Items.Values)
			{
				item.Unreveal(player);
			}
			return true;
		}

		public Item GetItem(uint itemId)
		{
			Item item;
			if (!Items.TryGetValue(itemId, out item))
				return null;
			return item;
		}

		public Item GetItemByDynId(Player plr, uint dynId)
		{
			if (Items.Values.Any(it => it.IsRevealedToPlayer(plr) && it.DynamicID(plr) == dynId))
				return Items.Values.Single(it => it.IsRevealedToPlayer(plr) && it.DynamicID(plr) == dynId);
			else
				return null;
		}
	}
}
