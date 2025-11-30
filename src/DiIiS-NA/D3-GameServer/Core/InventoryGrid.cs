using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
using Item = DiIiS_NA.GameServer.GSSystem.ItemsSystem.Item;

namespace DiIiS_NA.GameServer.Core
{
	public class InventoryGrid : IRevealable
	{
		static readonly Logger Logger = LogManager.CreateLogger(nameof(InventoryGrid));

		public int EquipmentSlot { get; private set; }
		public int Rows => _backpack.GetLength(0);
		public int Columns => _backpack.GetLength(1);
		public Dictionary<uint, Item> Items { get; private set; }
		private uint[,] _backpack;

		private readonly Actor _owner; // Used, because most information is not in the item class but Actors managed by the world

		public struct InventorySize
		{
			public int Width;
			public int Height;
		}

		public struct InventorySlot
		{
			public int Row;
			public int Column;
		}

		public InventoryGrid(Actor owner, int rows, int columns, int slot = 0)
		{
			_backpack = new uint[rows, columns];
			_owner = owner;
			Items = new Dictionary<uint, Item>();
			EquipmentSlot = slot;
		}

		public void ResizeGrid(int rows, int columns)
		{
			var newBackpack = new uint[rows, columns];
			Array.Copy(_backpack, newBackpack, _backpack.Length);
			_backpack = newBackpack;
		}

		public void Clear()
		{
			Items.Clear();
			int r = Rows;
			int c = Columns;
			_backpack = new uint[r, c];
		}

		// This should be in the database#
		// Do all items need a rectangual space in diablo 3?
		public InventorySize GetItemInventorySize(Item item)
		{
			if (EquipmentSlot == (int)EquipmentSlotId.Vendor || EquipmentSlot == (int)EquipmentSlotId.VendorBuyback)
				return new InventorySize() { Width = 1, Height = 1 };

			if(item.ItemType.Name == "TreasureBag")
				return new InventorySize() { Width = 1, Height = 2 };

			if (Item.IsWeapon(item.ItemType) || Item.IsArmor(item.ItemType) || Item.IsOffhand(item.ItemType))
			{
				if (!Item.IsBelt(item.ItemType))
					return new InventorySize() { Width = 1, Height = 2 };
			}

			return new InventorySize() { Width = 1, Height = 1 };
		}


		public bool FreeSpace(Item item, int row, int column)
		{
			bool result = true;
			InventorySize size = GetItemInventorySize(item);

			for (int r = row; r < Math.Min(row + size.Height, Rows); r++)
				for (int c = column; c < Math.Min(column + size.Width, Columns); c++)
					if ((_backpack[r, c] != 0) && (_backpack[r, c] != item.GlobalID))
						result = false;
			return result;
		}

		/// <summary>
		/// Collects (counts) the items overlapping with the item about to be dropped.
		/// If there are none, drop item
		/// If there are more, item cannot be dropped
		/// </summary>
		private int CollectOverlappingItems(Item item, int row, int column)
		{
			InventorySize dropSize = GetItemInventorySize(item);
			var overlapping = new List<uint>();

			// For every slot...
			for (int r = row; r < _backpack.GetLength(0) && r < row + dropSize.Height; r++)
				for (int c = column; c < _backpack.GetLength(1) && c < column + dropSize.Width; c++)

					// that contains an item other than the one we want to drop
					if (_backpack[r, c] != 0 && _backpack[r, c] != item.GlobalID) //TODO this would break for an item with id 0

						// add it to the list if if dropping the item in <row, column> would need the same slot
						//if (r >= row && r <= row + dropSize.Height)
						//	if (c >= column && c <= column + dropSize.Width)
						if (!overlapping.Contains(_backpack[r, c]))
							overlapping.Add(_backpack[r, c]);

			return overlapping.Count;
		}

		/// <summary>
		/// Removes an item from the backpack
		/// </summary>
		public void RemoveItem(Item item)
		{
			if (Items.ContainsKey(item.GlobalID)) Items.Remove(item.GlobalID);

			for (int r = 0; r < Rows; r++)
			{
				for (int c = 0; c < Columns; c++)
				{
					if (_backpack[r, c] == item.GlobalID)
					{
						_backpack[r, c] = 0;
					}
				}
			}
			if (_owner is Player ownerPlayer)
			{
				ownerPlayer.Inventory.RemoveItemFromDB(item);
			}
		}

		//NOTE: only for vendor grids
		public void DeleteItem(Item item)
		{
			if (Items.ContainsKey(item.GlobalID)) Items.Remove(item.GlobalID);

			for (int r = 0; r < Rows; r++)
			{
				for (int c = 0; c < Columns; c++)
				{
					if (_backpack[r, c] == item.GlobalID)
					{
						_backpack[r, c] = 0;
					}
				}
			}

			foreach (var plr in _owner.World.Players.Values)
				item.Unreveal(plr);
		}

		public bool HaveEnough(int GBid, int count)
		{
			List<Item> baseItems = Items.Values.Where(i => i.GBHandle.GBID == GBid).ToList();
			int have = 0;
			foreach (var itm in baseItems)
				have += itm.Attributes[GameAttributes.ItemStackQuantityLo];

			Logger.MethodTrace($"gbid {GBid}, count {have}");

			return (have >= count);
		}

		public int TotalItemCount(int GBid)
		{
			List<Item> baseItems = Items.Values.Where(i => i.GBHandle.GBID == GBid).ToList();
			int have = 0;
			foreach (var itm in baseItems)
				have += itm.Attributes[GameAttributes.ItemStackQuantityLo];

			return have;
		}

		public void GrabSomeItems(int GBid, int count) //only for stackable!
		{
			List<Item> baseItems = Items.Values.Where(i => i.GBHandle.GBID == GBid).ToList();
			int estimate = count;
			List<Item> consumed = new List<Item>();
			foreach (var itm in baseItems)
			{
				if (itm.Attributes[GameAttributes.ItemStackQuantityLo] > estimate)
				{
					itm.UpdateStackCount(itm.Attributes[GameAttributes.ItemStackQuantityLo] - estimate);
					break;
				}

				estimate -= itm.Attributes[GameAttributes.ItemStackQuantityLo];
				consumed.Add(itm);
			}
			foreach (var itm in consumed)
			{
				RemoveItem(itm);
				itm.Unreveal(itm.Owner as Player);
				//itm.Destroy();
			}
		}

		public void UnplaceItem(Item item)
		{
			if (Items.ContainsKey(item.GlobalID))
				Items.Remove(item.GlobalID);

			for (int r = 0; r < Rows; r++)
			{
				for (int c = 0; c < Columns; c++)
				{
					if (_backpack[r, c] == item.GlobalID)
					{
						_backpack[r, c] = 0;
					}
				}
			}
		}

		public void PlaceItem(Item item, int row, int column)
		{
			InventorySize size = GetItemInventorySize(item);

			//check backpack boundaries
			if (row + size.Width > Rows || column + size.Width > Columns) return;

			if (!Items.ContainsKey(item.GlobalID)) Items.Add(item.GlobalID, item);

			for (int r = row; r < Math.Min(row + size.Height, Rows); r++)
				for (int c = column; c < Math.Min(column + size.Width, Columns); c++)
				{
					//System.Diagnostics.Debug.Assert(_backpack[r, c] == 0, "You need to remove an item from the backpack before placing another item there");
					_backpack[r, c] = item.GlobalID;
				}

			item.Owner = _owner;
			item.SetInventoryLocation(EquipmentSlot, column, row);
		}

		/// <summary>
		/// Adds an item to the backpack
		/// </summary>
		public void AddItem(Item item, int row, int column)
		{
			InventorySize size = GetItemInventorySize(item);

			//check backpack boundaries
			if (row + size.Width > Rows || column + size.Width > Columns) return;

			if (item.IsStackable() && _owner is Player)
			{
				// Find items of same type (GBID) and try to add it to one of them
				List<Item> baseItems = Items.Values.Where(i => i.GBHandle.GBID == item.GBHandle.GBID).ToList();
				foreach (var baseItem in baseItems.Where(baseItem => baseItem.Attributes[GameAttributes.ItemStackQuantityLo] + item.Attributes[GameAttributes.ItemStackQuantityLo] <= baseItem.ItemDefinition.MaxStackSize))
				{
					baseItem.UpdateStackCount(baseItem.Attributes[GameAttributes.ItemStackQuantityLo] + item.Attributes[GameAttributes.ItemStackQuantityLo]);
					baseItem.Attributes.SendChangedMessage((_owner as Player).InGameClient);

					return;
				}
			}

			if (!Items.ContainsKey(item.GlobalID)) Items.Add(item.GlobalID, item);

			for (int r = row; r < Math.Min(row + size.Height, Rows); r++)
				for (int c = column; c < Math.Min(column + size.Width, Columns); c++)
				{
					//System.Diagnostics.Debug.Assert(_backpack[r, c] == 0, "You need to remove an item from the backpack before placing another item there");
					_backpack[r, c] = item.GlobalID;
				}

			item.Owner = _owner;
			if (_owner is Player ownerPlayer)
			{
				item.SetInventoryLocation(EquipmentSlot, column, row);
				if (EquipmentSlot == 15)
					ownerPlayer.Inventory.SaveItemToDB(ownerPlayer.Toon.GameAccount.DBGameAccount, null, EquipmentSlotId.Stash, item);
				else
					ownerPlayer.Inventory.SaveItemToDB(ownerPlayer.Toon.GameAccount.DBGameAccount, ownerPlayer.Toon.DbToon, EquipmentSlotId.Inventory, item);
			}
		}

		/// <summary>
		/// Adds an Item at a free spot to the backpack
		/// </summary>
		/// <param name="item"></param>
		public bool AddItem(Item item)
		{
			return AddItem(-1, -1, item);
		}
		/// <summary>
		/// Adds an Item at a free spot to the backpack
		/// </summary>
		/// <param name="minRow"></param>
		/// <param name="maxRow"></param>
		/// <param name="item"></param>
		public bool AddItem(int minRow, int maxRow, Item item)
		{
			InventorySlot? slot = FindSlotForItem(minRow, maxRow, item);
			if (slot.HasValue)
			{
				AddItem(item, slot.Value.Row, slot.Value.Column);
				return true;
			}
			else
			{
				Logger.Error("Can't find slot in backpack to add item {0}", item.SNO);
				if (_owner is Player)
					_owner.World.DropItem((_owner as Player), item);
				return false;
			}
		}

		public bool AddVendorItem(int minRow, int maxRow, Item item)
		{
			InventorySlot? slot = FindSlotForItem(minRow, maxRow, item);
			if (slot.HasValue)
			{
				PlaceItem(item, slot.Value.Row, slot.Value.Column);
				return true;
			}
			else
			{
				Logger.Error("Can't find slot in backpack to add item {0}", item.SNO);
				return false;
			}
		}

		public Boolean HasFreeSpace(Item item)
		{
			return (FindSlotForItem(-1, -1, item) != null);
		}

		public Boolean HasFreeSpace(int minRow, int maxRow, Item item)
		{
			return (FindSlotForItem(minRow, maxRow, item) != null);
		}

		public Boolean HasFreeSpace(Item item, int row, int column)
		{
			if (GetItemInventorySize(item).Height > 1)
			{
				if (EquipmentSlot == 0 && row > 4) return false;
				bool a = (_backpack[row, column] == 0 || _backpack[row, column] == item.GlobalID);
				bool b = (_backpack[row + 1, column] == 0 || _backpack[row + 1, column] == item.GlobalID);
				if (!((_backpack[row, column] == 0 || _backpack[row, column] == item.GlobalID) && (_backpack[row + 1, column] == 0 || _backpack[row + 1, column] == item.GlobalID)))
				 	return false;
				if (row + 2 < 6)
				{
					Item TryCheckItem2 = GetItem(row + 2, column);
					if (TryCheckItem2 != null)
						if (TryCheckItem2.InventoryLocation.X == column && TryCheckItem2.InventoryLocation.Y == row + 1) return false;
				}
				return ((_backpack[row, column] == 0 || _backpack[row, column] == item.GlobalID) && (_backpack[row + 1, column] == 0 || _backpack[row + 1, column] == item.GlobalID));
			}
			else
				return (_backpack[row, column] == 0 || _backpack[row, column] == item.GlobalID);
		}

		/// <summary>
		/// Checks whether the inventory contains an item
		/// </summary>
		public bool Contains(uint itemId) => Items.ContainsKey(itemId);

		public bool Contains(Item item) => Contains(item.GlobalID);

		/// <summary>
		/// Find an inventory slot with enough space for an item
		/// </summary>
		/// <returns>Slot or null if there is no space in the backpack</returns>
		public InventorySlot? FindSlotForItem(int minRow, int maxRow, Item item)
		{
			InventorySize size = GetItemInventorySize(item);
			// If we target a specific tab in stash, we need to specify min and max row to fill
			int nStartRow = minRow == -1 ? 0 : Math.Min(minRow, Rows); // maybe not needed, because Rows always > minRow
			int nEndRow = minRow == -1 ? Rows : Math.Min(maxRow, Rows);
			for (int r = nStartRow; r <= nEndRow - size.Height; r++)
				for (int c = 0; c <= Columns - size.Width; c++)
					if (CollectOverlappingItems(item, r, c) == 0)
						return new InventorySlot() { Row = r, Column = c };
			return null;
		}

		public Item GetItem(int row, int column)
		{
			return GetItem(_backpack[row, column]);
		}

		public bool Reveal(Player player)
		{
			if (_owner == null || _owner.World == null)
				return false;

			foreach (var item in Items.Values)
				item.Reveal(player);

			return true;
		}

		public bool Unreveal(Player player)
		{
			if (_owner == null || _owner.World == null)
				return false;

			foreach (var item in Items.Values)
				item.Unreveal(player);

			return true;
		}

		public Item GetItem(uint itemId)
		{
			if (!Items.TryGetValue(itemId, out var item))
				return null;
			return item;
		}

		public Item GetItemByDynId(Player plr, uint dynId)
		{
			return Items.Values.SingleOrDefault(it => it.IsRevealedToPlayer(plr) && it.DynamicID(plr) == dynId);
		}
	}
}
