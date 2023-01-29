using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
using DiIiS_NA.Core.MPQ;
using DiIiS_NA.Core.MPQ.FileFormats;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Inventory;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Artisan;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
using DiIiS_NA.GameServer.Core;
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Effect;
using DiIiS_NA.GameServer.ClientSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;
using static DiIiS_NA.Core.MPQ.FileFormats.GameBalance;
using DiIiS_NA.Core.Extensions;

namespace DiIiS_NA.GameServer.GSSystem.PlayerSystem
{
	public class Inventory : IMessageConsumer, IRevealable
	{

		static readonly Logger Logger = LogManager.CreateLogger();

		// Access by ID
		private readonly Player _owner; // Used, because most information is not in the item class but Actors managed by the world

		//Values for buying new slots on stash
		private readonly int[] _stashBuyValue = { 100000, 200000, 300000 };

		public bool Loaded { get; private set; }
		public bool StashLoaded { get; private set; }
		public bool StashRevealed { get; set; }
		private Equipment _equipment;
		private InventoryGrid _inventoryGrid;
		private InventoryGrid _stashGrid;
		private InventoryGrid _buybackGrid;
		private Item _inventoryGold;
		private Item inventoryPotion;
		public int BloodShards;
		public int Platinum;
		// backpack for spellRunes, their Items are kept in equipment
		private uint[] _skillSocketRunes;


		public Inventory(Player owner)
		{
			_owner = owner;
			_equipment = new Equipment(owner);
			_inventoryGrid = new InventoryGrid(owner, owner.Attributes[GameAttribute.Backpack_Slots] / 10, 10);
			_stashGrid = new InventoryGrid(owner, owner.Attributes[GameAttribute.Shared_Stash_Slots] / 7, 7, (int)EquipmentSlotId.Stash);
			_buybackGrid = new InventoryGrid(owner, 1, 20, (int)EquipmentSlotId.VendorBuyback);
			_skillSocketRunes = new uint[6];
			StashRevealed = false;
		}

		private void AcceptMoveRequest(Item item)
		{
			UpdateCurrencies();
		}

		public List<Item> GetBackPackItems()
		{
			return new List<Item>(_inventoryGrid.Items.Values);
		}

		public InventoryGrid GetBag()
		{
			return _inventoryGrid;
		}

		public List<Item> GetStashItems()
		{
			return new List<Item>(_stashGrid.Items.Values);
		}

		public List<Item> GetEquippedItems()
		{
			return _equipment.Items.Values.ToList();
		}

		public List<Item> GetBuybackItems()
		{
			return new List<Item>(_buybackGrid.Items.Values);
		}

		public InventoryGrid GetBuybackGrid()
		{
			return _buybackGrid;
		}

		public bool HaveEnough(int GBid, int count)
		{
			return (_inventoryGrid.TotalItemCount(GBid) + _stashGrid.TotalItemCount(GBid)) >= count;
		}

		public void GrabSomeItems(int GBid, int count)
		{
			if (_inventoryGrid.HaveEnough(GBid, count))
				_inventoryGrid.GrabSomeItems(GBid, count);
			else
			{
				int inBag = _inventoryGrid.TotalItemCount(GBid);
				_inventoryGrid.GrabSomeItems(GBid, inBag);
				count -= inBag;
				_stashGrid.GrabSomeItems(GBid, count);
			}
		}

		public int GetGearScore()
		{
			return GetEquippedItems().Where(item => item.Attributes[GameAttribute.Item_Binding_Level_Override] == 0).Select(i => i.Rating).Sum();
		}

		public int GetAvgLevel()
		{
			if (GetEquippedItems().Count == 0) return 0;
			return (int)GetEquippedItems().Select(item => item.ItemDefinition.ItemLevel).Average();
		}


		/// <summary>
		/// Refreshes the visual appearance of the hero
		/// </summary>
		public void SendVisualInventory(Player player)
		{
			//player.InGameClient.SendMessage(message);
			player.World.BroadcastIfRevealed(plr => new VisualInventoryMessage()
			{
				ActorID = _owner.DynamicID(plr),
				EquipmentList = new VisualEquipment()
				{
					Equipment = _equipment.GetVisualEquipment(),
					CosmeticEquipment = _equipment.GetVisualCosmeticEquipment()
				},
			}, _owner);
		}

		public D3.Hero.VisualEquipment GetVisualEquipment()
		{
			return _equipment.GetVisualEquipmentForToon();
		}

		public bool HasInventorySpace(Item item)
		{
			return _inventoryGrid.HasFreeSpace(item);
		}

		/// <summary>
		/// Picks an item up after client request
		/// </summary>
		/// <returns>true if the item was picked up, or false if the player could not pick up the item.</returns>
		public bool PickUp(Item item)
		{

			if (_inventoryGrid.Contains(item) || _equipment.IsItemEquipped(item))
				return false;
			// TODO: Autoequip when equipment slot is empty

			// If Item is Stackable try to add the amount
			if (item.IsStackable())
			{
				// Find items of same type (GBID) and try to add it to one of them
				List<Item> baseItems = FindSameItems(item.GBHandle.GBID);
				foreach (Item baseItem in baseItems)
				{
					if (baseItem.Attributes[GameAttribute.ItemStackQuantityLo] + item.Attributes[GameAttribute.ItemStackQuantityLo] <= baseItem.ItemDefinition.MaxStackSize)
					{
						baseItem.UpdateStackCount(baseItem.Attributes[GameAttribute.ItemStackQuantityLo] + item.Attributes[GameAttribute.ItemStackQuantityLo]);
						baseItem.Attributes.SendChangedMessage(_owner.InGameClient);

						// Item amount successful added. Don't place item in inventory instead destroy it.
						item.Destroy();
						return true;
					}
				}
			}

			bool success = false;
			if (!_inventoryGrid.HasFreeSpace(item))
			{
				// Inventory full
				_owner.InGameClient.SendMessage(new ACDPickupFailedMessage()
				{
					IsCurrency = false,
					Reason = ACDPickupFailedMessage.Reasons.InventoryFull,
					ItemID = unchecked((uint)-1)
				});
				_owner.PlayEffect(Effect.PickupFailOverburden1, null, false);
			}
			else
			{
				item.CurrentState = ItemState.PickingUp;
				if (item.HasWorldLocation && item.World != null)
				{
					item.Owner = _owner;
					item.World.Leave(item);
				}


				_inventoryGrid.AddItem(item);

				if (_owner.GroundItems.ContainsKey(item.GlobalID))
					_owner.GroundItems.Remove(item.GlobalID);
				success = true;
				item.CurrentState = ItemState.Normal;

				foreach (var plr in _owner.World.Players.Values)
					if (plr != _owner)
						item.Unreveal(plr);
				AcceptMoveRequest(item);
			}

			//System.Threading.Thread.Sleep(10000);
			if (item.ItemType.Name.Contains("TemplarSpecial"))
				_owner.GrantCriteria(74987243307208);
			if (item.ItemType.Name.Contains("EnchantressSpecial"))
				_owner.GrantCriteria(74987243308421);
			if (item.ItemType.Name.Contains("ScoundrelSpecial"))
				_owner.GrantCriteria(74987243308422);

			_owner.PlayEffect(Effect.Sound, 196576);
			return success;
		}

		/// <summary>
		/// Used for equiping item
		/// </summary>
		/// <param name="item"></param>
		/// <param name="slot"></param>
		public void EquipItem(Item item, int slot, bool save = true)
		{
			_equipment.EquipItem(item, slot, save);
			if (save) ChangeItemSlotDB(slot, item);
		}

		public List<Item> PublicFindSameItems(int gbid)
		{
			return FindSameItems(gbid);
		}

		private List<Item> FindSameItems(int gbid)
		{
			return _inventoryGrid.Items.Values.Where(i => i.GBHandle.GBID == gbid).ToList();
		}

		private static readonly List<string> OneHandedWeapons = new List<string>{
			"Axe",
			"Sword",
			"Mace",
			"FistWeapon",
			"MightyWeapon1H",
			"Flail1H",
			"CeremonialDagger",
			"Dagger",
			"Spear",
			"HandXbow",
			"Wand"
		};

		private static readonly List<string> TwoHandedWeapons = new List<string>{
			"Axe2H",
			"Sword2H",
			"Mace2H",
			"CombatStaff",
			"MightyWeapon2H",
			"Staff",
			"Flail2H",
			"Polearm",
			"Bow",
			"Crossbow"
		};

		public void BuyItem(ActorSystem.Actor vendor, Item originalItem, bool buyback)
		{
			if (originalItem.ItemDefinition.Name.StartsWith("PH_"))
			{
				string itemType = originalItem.ItemDefinition.Name.Substring(3);
				if (itemType.Contains("1HWeapon"))
					itemType = OneHandedWeapons[FastRandom.Instance.Next(OneHandedWeapons.Count())];
				if (itemType.Contains("2HWeapon"))
					itemType = TwoHandedWeapons[FastRandom.Instance.Next(TwoHandedWeapons.Count())];
				if (itemType.Contains("Pants"))
					itemType = "Legs";
				_inventoryGrid.AddItem(ItemGenerator.GetRandomItemOfType(_owner, ItemGroup.FromString(itemType)));
			}
			else
			{
				if (Item.IsDye(originalItem.ItemType) || Item.IsPotion(originalItem.ItemType))
				{
					Item newItem = ItemGenerator.CloneItem(originalItem);
					newItem.DBInventory = null;
					_inventoryGrid.AddItem(newItem);
				}
				else
				{
					if (buyback)
						_buybackGrid.DeleteItem(originalItem);
					else
						(vendor as Vendor).DeleteItem(originalItem);
					_inventoryGrid.AddItem(originalItem);
				}
			}
		}

		public void SellItem(ActorSystem.Actor vendor, int itemId)
		{
			Item item = _inventoryGrid.GetItemByDynId(_owner, (uint)itemId);
			if (item == null) return;
			int cost = (int)Math.Floor(item.GetPrice() / 25f) * Math.Max(1, item.Attributes[GameAttribute.Gold]);
			_inventoryGrid.RemoveItem(item);
			item.Unreveal(_owner);
			AddGoldAmount(cost);
			(vendor as Vendor).AddBuybackItem(item, _owner);
			_owner.PlayEffect(Effect.Sound, 36744);
		}

		public bool CheckItemSlots(Item target_item, int destination_slot)
		{
			if (destination_slot == 0 || destination_slot == 15) return true; //bag and stash

			if (Item.IsGem(target_item.ItemType)) return false; //can't equip gem

			if (target_item.Attributes[GameAttribute.Requirement, 67] > (_owner.Level + 5)) return false; //can't equip too high level
			if (destination_slot == 14 || destination_slot == 16 || destination_slot == 17)
				return false; //can't equip in utility slots

			if (!(destination_slot == 3 || destination_slot == 4 || destination_slot == 21 || destination_slot == 22) && Item.IsWeapon(target_item.ItemType)) return false; //can't equip weapon in another slots
			if (destination_slot != 1 && destination_slot != 27 && Item.IsHelm(target_item.ItemType)) return false; //can't equip helm in another slots
			if (destination_slot != 2 && destination_slot != 28 && Item.IsChestArmor(target_item.ItemType)) return false; //can't equip chest in another slots
			if (!(destination_slot == 13 || destination_slot == 24) && Item.IsAmulet(target_item.ItemType)) return false; //can't equip amulets in another slots
			if (!(destination_slot == 11 || destination_slot == 12 || destination_slot == 25 || destination_slot == 26) && Item.IsRing(target_item.ItemType)) return false; //can't equip rings in another slots

			return true;
		}

		/// <summary>
		/// Handles a request to move an item within the inventory.
		/// This covers moving items within the backpack, from equipment
		/// slot to backpack and from backpack to equipment slot
		/// </summary>
		public void HandleInventoryRequestMoveMessage(InventoryRequestMoveMessage request)
		{
			// TODO Normal inventory movement does not require setting of inv loc msg! Just Tick. /fasbat
			Item item = GetItemByDynId(_owner, request.ItemID);

			if (item == null)
			{
				if (_owner.ActiveHireling == null)
					return;
				else
					item = _owner.ActiveHireling.GetItemByDynId(_owner, request.ItemID);
			}

			if (!CheckItemSlots(item, request.Location.EquipmentSlot)) return;

			if (item.InvLoc(_owner).EquipmentSlot > 20)
			{
				if (_owner.ActiveHireling != null)
					_owner.ActiveHireling.UnequipItem(_owner, item.InvLoc(_owner).EquipmentSlot, item);
				return;
			}

			if (request.Location.EquipmentSlot > 20)
			{
				var sourceGrid = (item.InvLoc(_owner).EquipmentSlot == 0 ? _inventoryGrid : item.InvLoc(_owner).EquipmentSlot == (int)EquipmentSlotId.Stash ? _stashGrid : null);
				sourceGrid.UnplaceItem(item);
				if (_owner.ActiveHireling != null)
				{
					_owner.ActiveHireling.UnequipItemFromSlot(_owner, request.Location.EquipmentSlot);
					_owner.ActiveHireling.EquipItem(_owner, request.Location.EquipmentSlot, item);
				}
				return;
			}
			// Request to equip item from backpack
			if (request.Location.EquipmentSlot != 0 && request.Location.EquipmentSlot != (int)EquipmentSlotId.Stash)
			{
				var sourceGrid = (item.InvLoc(_owner).EquipmentSlot == 0 ? _inventoryGrid :
					item.InvLoc(_owner).EquipmentSlot == (int)EquipmentSlotId.Stash ? _stashGrid : null);

				//System.Diagnostics.Debug.Assert((sourceGrid != null && sourceGrid.Contains(request.ItemID)) || _equipment.IsItemEquipped(request.ItemID), "Request to equip unknown item");

				int targetEquipSlot = request.Location.EquipmentSlot;
				if (Item.Is2H(item.ItemType)) targetEquipSlot = (int)EquipmentSlotId.Main_Hand;

				if (IsValidEquipmentRequest(item, targetEquipSlot))
				{
					Item oldEquipItem = _equipment.GetEquipment(targetEquipSlot);

					// check if equipment slot is empty
					if (oldEquipItem == null)
					{
						// determine if item is in backpack or switching item from position with target originally empty
						if (sourceGrid == null)
							_equipment.UnequipItem(item);
						else
							sourceGrid.UnplaceItem(item);
						_equipment.EquipItem(item, targetEquipSlot);
						AcceptMoveRequest(item);
					}
					else
					{
						// check if item is already equipped at another equipmentslot
						if (_equipment.IsItemEquipped(item))
						{
							// switch both items
							if (!IsValidEquipmentRequest(oldEquipItem, item.EquipmentSlot))
								return;

							int oldEquipmentSlot = _equipment.UnequipItem(item);
							_equipment.EquipItem(item, targetEquipSlot);
							_equipment.EquipItem(oldEquipItem, oldEquipmentSlot);

						}
						else
						{
							// Get original location
							int x = item.InventoryLocation.X;
							int y = item.InventoryLocation.Y;
							// equip item and place other item in the backpack
							sourceGrid.UnplaceItem(item);
							int oldEquipmentSlot = item.EquipmentSlot;
							_equipment.UnequipItem(oldEquipItem);
							ChangeItemSlotDB(oldEquipmentSlot, oldEquipItem);
							ChangeItemLocationDB(x, y, oldEquipItem);
							Logger.Trace("HandleInventoryRequestMoveMessage, Slot: {0}, x: {1}, y: {2}", targetEquipSlot, request.Location.Column, request.Location.Row);
							oldEquipItem.SetInventoryLocation(oldEquipmentSlot, x, y);
							sourceGrid.PlaceItem(oldEquipItem, y, x);
							_equipment.EquipItem(item, targetEquipSlot);
						}
						AcceptMoveRequest(item);
						AcceptMoveRequest(oldEquipItem);
					}

					SendVisualInventory(_owner);
				}
			}
			// Request to move an item (from backpack or equipmentslot)
			else
			{
				var destGrid = (request.Location.EquipmentSlot == 0 ? _inventoryGrid : _stashGrid);
				var sourceGrid = (item.EquipmentSlot == 0 ? _inventoryGrid : _stashGrid);

				if (destGrid.HasFreeSpace(item, request.Location.Row, request.Location.Column)) //if cell is free
				{
					if (sourceGrid.GetItemInventorySize(item).Height == 2)
					{
						//Item crossitem = sourceGrid.GetItem(request.Location.Column, request.Location.Row + 1);
						try
						{
							Item crossitem = sourceGrid.GetItem(request.Location.Row + 1, request.Location.Column);
							if (crossitem != null)
							{
								sourceGrid.GetItem(request.Location.Row + 1, request.Location.Column);
								ChangeItemSlotDB(item.EquipmentSlot, crossitem);
								ChangeItemLocationDB(item.InventoryLocation.X, item.InventoryLocation.Y + 1, crossitem);
								sourceGrid.PlaceItem(crossitem, item.InventoryLocation.Y + 1, item.InventoryLocation.X);
							}
						}
						catch
						{
							Item crossitem = sourceGrid.GetItem(request.Location.Column, request.Location.Row + 1);
							if (crossitem != null)
							{
								sourceGrid.GetItem(request.Location.Row + 1, request.Location.Column);
								ChangeItemSlotDB(item.EquipmentSlot, crossitem);
								ChangeItemLocationDB(item.InventoryLocation.X, item.InventoryLocation.Y + 1, crossitem);
								sourceGrid.PlaceItem(crossitem, item.InventoryLocation.Y + 1, item.InventoryLocation.X);
							}
						}
					}

					if (_equipment.IsItemEquipped(item))
					{
						_equipment.UnequipItem(item); // Unequip the item
						SendVisualInventory(_owner);
					}
					else
					{
						sourceGrid.UnplaceItem(item);
					}
					ChangeItemSlotDB(request.Location.EquipmentSlot, item);
					ChangeItemLocationDB(request.Location.Column, request.Location.Row, item);
					if (item.InvLoc(_owner).EquipmentSlot != request.Location.EquipmentSlot)
						AcceptMoveRequest(item);
					destGrid.PlaceItem(item, request.Location.Row, request.Location.Column);
					Logger.Trace("HandleInventoryRequestMoveMessage, Slot: {0}, x: {1}, y: {2}", request.Location.EquipmentSlot, request.Location.Column, request.Location.Row);
					//item.SetInventoryLocation(request.Location.EquipmentSlot, request.Location.Column, request.Location.Row);
				}
				else
				{//if item in cell already exists (swaps them)
					if (_equipment.IsItemEquipped(item))
					{
						SendVisualInventory(_owner);
						return; //don't allow to swap item from equipped
					}

					Item oldItem = destGrid.GetItem(request.Location.Row, request.Location.Column);

					if (item.IsStackable() && item.GBHandle.GBID == oldItem.GBHandle.GBID && oldItem.Attributes[GameAttribute.ItemStackQuantityLo] < oldItem.ItemDefinition.MaxStackSize) //if it's stackable and same item (merge stacks)
					{
						if (item.Attributes[GameAttribute.ItemStackQuantityLo] + oldItem.Attributes[GameAttribute.ItemStackQuantityLo] <= oldItem.ItemDefinition.MaxStackSize)
						{
							oldItem.UpdateStackCount(oldItem.Attributes[GameAttribute.ItemStackQuantityLo] + item.Attributes[GameAttribute.ItemStackQuantityLo]);
							DestroyInventoryItem(item);
							oldItem.Attributes.SendChangedMessage((_owner as Player).InGameClient);
						}
						else
						{
							item.UpdateStackCount(item.Attributes[GameAttribute.ItemStackQuantityLo] - (oldItem.ItemDefinition.MaxStackSize - oldItem.Attributes[GameAttribute.ItemStackQuantityLo]));
							item.Attributes.SendChangedMessage((_owner as Player).InGameClient);
							oldItem.UpdateStackCount(oldItem.ItemDefinition.MaxStackSize);
							oldItem.Attributes.SendChangedMessage((_owner as Player).InGameClient);
						}
						Logger.Trace("HandleInventoryRequestMoveMessage(StackMerge) Slot: {0}, x: {1}, y: {2}", request.Location.EquipmentSlot, request.Location.Row, request.Location.Column);
					}
					else
					{
						bool Replaced = false;
						bool ReplacedNew = false;
						if (oldItem == null) oldItem = destGrid.GetItem(request.Location.Row + 1, request.Location.Column);
						if (oldItem == null) oldItem = destGrid.GetItem(request.Location.Row + 2, request.Location.Column);
						// Get original location
						int oldEquipmentSlot = item.EquipmentSlot;
						int new_x = item.InventoryLocation.X;
						int new_y = item.InventoryLocation.Y;
						int old_x = oldItem.InventoryLocation.X;
						int old_y = oldItem.InventoryLocation.Y;
						//grab from current positions
						sourceGrid.UnplaceItem(item);
						destGrid.UnplaceItem(oldItem);
						//save to db
						//InventorySize size = sourceGrid.GetItemInventorySize(item)
						if (sourceGrid.GetItemInventorySize(item).Height == 2 && destGrid.GetItemInventorySize(oldItem).Height == 1 && new_x == old_x)
						{
							if (new_y - old_y < 2 && new_y - old_y > -2)
								new_y++;
						}
						else if (sourceGrid.GetItemInventorySize(item).Height == 1 && destGrid.GetItemInventorySize(oldItem).Height == 2 && new_x == old_x)
						{
							if (sourceGrid.HasFreeSpace(oldItem, new_y, new_x))
							{
								if (new_y - old_y <= 2 && new_y - old_y >= -2)
									new_y--;
								if (item.InventoryLocation.Y > 0)
								{
									var addedItem = destGrid.GetItem(item.InvLoc(_owner).Row - 1, item.InvLoc(_owner).Column);
									if (addedItem != null)
									{
										ChangeItemSlotDB(request.Location.EquipmentSlot, addedItem);
										ChangeItemLocationDB(old_x, old_y++, addedItem);
										destGrid.PlaceItem(addedItem, old_y++, old_x);
									}
								}
								else
								{
									new_y++;
									old_y++;
								}


							}
							else
							{
								if (new_y < 5)
								{
									var addedItem = destGrid.GetItem(item.InvLoc(_owner).Row + 1, item.InvLoc(_owner).Column);
									if (addedItem != null)
									{
										if (sourceGrid.GetItemInventorySize(addedItem).Height != 2)
										{
											ChangeItemSlotDB(request.Location.EquipmentSlot, addedItem);
											ChangeItemLocationDB(old_x, old_y + 1, addedItem);
											destGrid.PlaceItem(addedItem, old_y + 1, old_x);
										}
									};
								}
								else
								{
									new_y--;
								}
							}
						}
						else if (sourceGrid.GetItemInventorySize(item).Height == 1 && destGrid.GetItemInventorySize(oldItem).Height == 2 && new_x != old_x)
						{
							if (!sourceGrid.HasFreeSpace(oldItem, item.InvLoc(_owner).Row + 1, item.InvLoc(_owner).Column))
							{
								if (new_y < 5)
								{
									var addedItem = destGrid.GetItem(item.InventoryLocation.Y + 1, item.InventoryLocation.X);
									if (addedItem != null)
									{
										if (sourceGrid.GetItemInventorySize(addedItem).Height == 2)
										{
											if (!sourceGrid.HasFreeSpace(oldItem))
											{
												// Inventory full
												_owner.InGameClient.SendMessage(new ACDPickupFailedMessage()
												{
													IsCurrency = false,
													Reason = ACDPickupFailedMessage.Reasons.InventoryFull,
													ItemID = unchecked((uint)-1)
												});
												_owner.PlayEffect(Effect.PickupFailOverburden1, null, false);
											}
											else
											{
												sourceGrid.RemoveItem(oldItem);
												sourceGrid.AddItem(oldItem);
												ChangeItemSlotDB(oldEquipmentSlot, oldItem);
												ChangeItemLocationDB(oldItem.InventoryLocation.X, oldItem.InventoryLocation.Y, oldItem);
												Replaced = true;
											}
										}
										else
										{
											ChangeItemSlotDB(request.Location.EquipmentSlot, addedItem);
											ChangeItemLocationDB(old_x, old_y + 1, addedItem);
											destGrid.PlaceItem(addedItem, old_y + 1, old_x);
										}
									}
								}
								else
								{
									new_y--;
								}
							}
							/*
							if (!sourceGrid.HasFreeSpace(oldItem, item.InvLoc(_owner).Row - 1, item.InvLoc(_owner).Column))
							{
								var addedItem = destGrid.GetItem(item.InvLoc(_owner).Row - 1, item.InvLoc(_owner).Column);
								if (sourceGrid.GetItemInventorySize(addedItem).Height == 2)
									return;
								else
								{
									if (old_y != 0)
										old_y--;

									ChangeItemSlotDB(request.Location.EquipmentSlot, addedItem);
									ChangeItemLocationDB(old_x, old_y, addedItem);
									destGrid.PlaceItem(addedItem, old_y, old_x);
								}
							}
							//else if (sourceGrid.HasFreeSpace(oldItem, item.InvLoc(_owner).Row + 1, item.InvLoc(_owner).Column))
							//	new_y--;
							else
							{
								var addedItem = destGrid.GetItem(item.InvLoc(_owner).Row + 1, item.InvLoc(_owner).Column);
								if (addedItem != null)
								{
									if (sourceGrid.GetItemInventorySize(addedItem).Height == 2)
										;
									else
									{
										ChangeItemSlotDB(request.Location.EquipmentSlot, addedItem);
										ChangeItemLocationDB(old_x, old_y + 1, addedItem);
										destGrid.PlaceItem(addedItem, old_y + 1, old_x);
									}
								}

							}//*/
						}
						else if (sourceGrid.GetItemInventorySize(item).Height == 2 && destGrid.GetItemInventorySize(oldItem).Height == 2)
						{
							old_x = request.Location.Column;
							old_y = request.Location.Row;
							if (!sourceGrid.HasFreeSpace(oldItem))
							{
								// Inventory full
								_owner.InGameClient.SendMessage(new ACDPickupFailedMessage()
								{
									IsCurrency = false,
									Reason = ACDPickupFailedMessage.Reasons.InventoryFull,
									ItemID = unchecked((uint)-1)
								});
								_owner.PlayEffect(Effect.PickupFailOverburden1, null, false);
								Replaced = true;
								ReplacedNew = true;
							}
							else
							{
								Replaced = true;
							}
						}

						if (!Replaced)
						{
							ChangeItemSlotDB(oldEquipmentSlot, oldItem);
							ChangeItemLocationDB(new_x, new_y, oldItem);
						}
						if (!ReplacedNew)
						{
							ChangeItemSlotDB(request.Location.EquipmentSlot, item);
							ChangeItemLocationDB(old_x, old_y, item);
						}
						//changing locations
						Logger.Trace("HandleInventoryRequestMoveMessage, Slot: {0}, x: {1}, y: {2}", request.Location.EquipmentSlot, request.Location.Row, request.Location.Column);
						//oldItem.SetInventoryLocation(oldEquipmentSlot, x, y);
						if (!ReplacedNew)
							destGrid.PlaceItem(item, old_y, old_x);
						if (!Replaced)
							sourceGrid.PlaceItem(oldItem, new_y, new_x);
						if (Replaced)
						{
							sourceGrid.RemoveItem(oldItem);
							sourceGrid.AddItem(oldItem);
							ChangeItemSlotDB(oldEquipmentSlot, oldItem);
							ChangeItemLocationDB(oldItem.InventoryLocation.X, oldItem.InventoryLocation.Y, oldItem);
						}
					}
				}
			}

			RefreshInventoryToClient();
			CheckAchievements();
		}

		private void Recheckall()
		{

		}

		private void CheckAchievements()
		{
			if (GetEquippedItems().Count == 0) return;

			if (GetAvgLevel() >= 25)
				_owner.GrantAchievement(74987243307124);
			if (GetAvgLevel() >= 60)
				_owner.GrantAchievement(74987243307126);

			var items = GetEquippedItems();
			if (items.Where(item => ItemGroup.IsSubType(item.ItemType, "Belt_Barbarian")).Count() > 0 && (items.Where(item => ItemGroup.IsSubType(item.ItemType, "MightyWeapon1H")).Count() > 0 || items.Where(item => ItemGroup.IsSubType(item.ItemType, "MightyWeapon2H")).Count() > 0)) //barb
				_owner.GrantAchievement(74987243307046);
			if (items.Where(item => ItemGroup.IsSubType(item.ItemType, "Cloak")).Count() > 0 && items.Where(item => ItemGroup.IsSubType(item.ItemType, "HandXbow")).Count() > 0) //dh
				_owner.GrantAchievement(74987243307058);
			if (items.Where(item => ItemGroup.IsSubType(item.ItemType, "SpiritStone_Monk")).Count() > 0 && (items.Where(item => ItemGroup.IsSubType(item.ItemType, "FistWeapon")).Count() > 0 || items.Where(item => ItemGroup.IsSubType(item.ItemType, "CombatStaff")).Count() > 0)) //monk
				_owner.GrantAchievement(74987243307544);
			if (items.Where(item => ItemGroup.IsSubType(item.ItemType, "VoodooMask")).Count() > 0 && items.Where(item => ItemGroup.IsSubType(item.ItemType, "CeremonialDagger")).Count() > 0 && items.Where(item => ItemGroup.IsSubType(item.ItemType, "Mojo")).Count() > 0) //wd
				_owner.GrantAchievement(74987243307561);
			if (items.Where(item => ItemGroup.IsSubType(item.ItemType, "WizardHat")).Count() > 0 && items.Where(item => ItemGroup.IsSubType(item.ItemType, "Wand")).Count() > 0 && items.Where(item => ItemGroup.IsSubType(item.ItemType, "Orb")).Count() > 0) //wiz
				_owner.GrantAchievement(74987243307582);
		}

		/// <summary>
		/// Handles a request to move an item from stash the inventory and back
		/// </summary>
		public void HandleInventoryRequestQuickMoveMessage(InventoryRequestQuickMoveMessage request)
		{
			Item item = GetItemByDynId(_owner, request.ItemID);
			if (item == null || (request.DestEquipmentSlot != (int)EquipmentSlotId.Stash && request.DestEquipmentSlot != (int)EquipmentSlotId.Inventory))
				return;

			if (!CheckItemSlots(item, request.DestEquipmentSlot)) return;
			// Identify source and destination grids
			var destinationGrid = request.DestEquipmentSlot == 0 ? _inventoryGrid : _stashGrid;

			var sourceGrid = request.DestEquipmentSlot == 0 ? _stashGrid : _inventoryGrid;

			if (destinationGrid.HasFreeSpace(request.DestRowStart, request.DestRowEnd, item))
			{
				var slot = destinationGrid.FindSlotForItem(request.DestRowStart, request.DestRowEnd, item);
				if (!slot.HasValue) return;
				sourceGrid.UnplaceItem(item);
				ChangeItemSlotDB(request.DestEquipmentSlot, item);
				ChangeItemLocationDB(slot.Value.Column, slot.Value.Row, item);
				destinationGrid.PlaceItem(item, slot.Value.Row, slot.Value.Column);
				Logger.Trace("HandleInventoryRequestQuickMoveMessage, Slot: {0}, pos: {1}|{2}, DestRow: {3}|{4}", request.DestEquipmentSlot, slot.Value.Row, slot.Value.Column, request.DestRowStart, request.DestRowEnd);
				//item.SetInventoryLocation(request.DestEquipmentSlot, slot.Value.Column, slot.Value.Row);
			}
		}

		public void CheckWeapons()
		{
			Item itemMainHand = _equipment.GetEquipment(EquipmentSlotId.Main_Hand);
			Item itemOffHand = _equipment.GetEquipment(EquipmentSlotId.Off_Hand);
			bool bugged = false;
			if (itemOffHand == null) return;

			if (Item.Is2H(itemOffHand.ItemType)) bugged = true;

			if (itemMainHand != null)
			{
				if (Item.Is2H(itemMainHand.ItemType))
				{
					if (Item.IsShield(itemOffHand.ItemType) && !_owner.Attributes[GameAttribute.Allow_2H_And_Shield])
						bugged = true;      //Crusader - Heavenly Strength

					if (Item.IsBow(itemMainHand.ItemType) && !Item.IsQuiver(itemOffHand.ItemType))
						bugged = true;
				}
				else
				{
					if (Item.IsHandXbow(itemMainHand.ItemType) && !Item.IsHandXbow(itemOffHand.ItemType) && !Item.IsQuiver(itemOffHand.ItemType))
						bugged = true;

					if (!Item.IsHandXbow(itemMainHand.ItemType) && Item.IsHandXbow(itemOffHand.ItemType))
						bugged = true;
				}
			}

			if (bugged)
			{
				_equipment.UnequipItem(itemMainHand);
				RemoveItemFromDB(itemMainHand);
				_inventoryGrid.AddItem(itemMainHand);

				_equipment.UnequipItem(itemOffHand);
				RemoveItemFromDB(itemOffHand);
				_inventoryGrid.AddItem(itemOffHand);

				SendVisualInventory(_owner);
			}
		}

		public float AdjustDualWieldMin(PowerSystem.DamageType damageType)
		{
			float mainDmgMin = 0f;
			float offDmgMin = 0f;

			Item itemMainHand = _equipment.GetEquipment(EquipmentSlotId.Main_Hand);
			Item itemOffHand = _equipment.GetEquipment(EquipmentSlotId.Off_Hand);
			if (itemMainHand != null)
				if (itemOffHand != null && !Item.IsOffhand(itemOffHand.ItemType))
				{
					mainDmgMin = itemMainHand.Attributes[GameAttribute.Damage_Weapon_Min_Total, damageType.AttributeKey];
					offDmgMin = itemOffHand.Attributes[GameAttribute.Damage_Weapon_Min_Total, damageType.AttributeKey];
				}

			return (mainDmgMin + offDmgMin) * 0.5f;
		}

		public float AdjustDualWieldDelta(PowerSystem.DamageType damageType)
		{
			float mainDmgDelta = 0f;
			float offDmgDelta = 0f;

			Item itemMainHand = _equipment.GetEquipment(EquipmentSlotId.Main_Hand);
			Item itemOffHand = _equipment.GetEquipment(EquipmentSlotId.Off_Hand);
			if (itemMainHand != null)
				if (itemOffHand != null && !Item.IsOffhand(itemOffHand.ItemType))
				{
					mainDmgDelta = itemMainHand.Attributes[GameAttribute.Damage_Weapon_Delta_Total, damageType.AttributeKey];
					offDmgDelta = itemOffHand.Attributes[GameAttribute.Damage_Weapon_Delta_Total, damageType.AttributeKey];
				}

			return (mainDmgDelta + offDmgDelta) * 0.5f;
		}

		public float GetAPS()
		{
			float aps = 0f;

			Item itemMainHand = _equipment.GetEquipment(EquipmentSlotId.Main_Hand);
			Item itemOffHand = _equipment.GetEquipment(EquipmentSlotId.Off_Hand);
			if (itemMainHand != null)
			{
				float main_aps = itemMainHand.Attributes[GameAttribute.Attacks_Per_Second_Item];
				aps = main_aps;
				if (itemOffHand != null && !Item.IsOffhand(itemOffHand.ItemType))
				{
					float off_aps = itemOffHand.Attributes[GameAttribute.Attacks_Per_Second_Item];
					if (main_aps > 0f && off_aps > 0f)
						aps = 2f / ((1f / (main_aps * 1.15f)) + (1f / (off_aps * 1.15f)));
				}
			}
			else if (itemOffHand != null)
			{
				aps = itemOffHand.Attributes[GameAttribute.Attacks_Per_Second_Item];
			}

			if (aps < 0.5f)
				aps = 0.5f;

			return Math.Min(aps, 4f);
		}

		public float GetMagicFind()
		{
			if (_owner.World == null)
				return GetItemBonus(GameAttribute.Magic_Find);

			var difficulty = _owner.World.Game.Difficulty;
			var mult = 1f;

			switch (difficulty)
			{
				case 0: mult = 0.1f; break;     //Normal
				case 1: mult = 0.2f; break;     //Hard
				case 2: mult = 0.3f; break;     //Expert
				case 3: mult = 0.4f; break;     //Master
				case 4: mult = 0.5f; break;     //T1
				case 5: mult = 0.6f; break;     //T2
				case 6: mult = 0.7f; break;     //T3
				case 7: mult = 0.8f; break;     //T4
				case 8: mult = 0.9f; break;     //T5
				case 9: mult = 1f; break;       //T6
				default: mult = 1f; break;
			}

			return GetItemBonus(GameAttribute.Magic_Find) * mult;
		}

		public float GetGoldFind()
		{
			if (_owner.World == null)
				return GetItemBonus(GameAttribute.Gold_Find);

			var difficulty = _owner.World.Game.Difficulty;
			var mult = 1f;

			switch (difficulty)
			{
				case 0: mult = 0.1f; break;     //Normal
				case 1: mult = 0.2f; break;     //Hard
				case 2: mult = 0.3f; break;     //Expert
				case 3: mult = 0.4f; break;     //Master
				case 4: mult = 0.5f; break;     //T1
				case 5: mult = 0.6f; break;     //T2
				case 6: mult = 0.7f; break;     //T3
				case 7: mult = 0.8f; break;     //T4
				case 8: mult = 0.9f; break;     //T5
				case 9: mult = 1f; break;       //T6
				default: mult = 1f; break;
			}

			return GetItemBonus(GameAttribute.Gold_Find) * mult;
		}


		/// <summary>
		/// Checks if Item can be equipped at that slot. Handels equipment for Two-Handed-Weapons.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="equipmentSlot"></param>
		/// <returns></returns>
		private bool IsValidEquipmentRequest(Item item, int equipmentSlot)
		{
			ItemTypeTable type = item.ItemType;

			if (equipmentSlot == (int)EquipmentSlotId.Main_Hand)
			{
				// useful for 1hand + shield switching, this is to avoid shield to be go to main hand
				if (!Item.IsWeapon(type))
					return false;

				if (Item.Is2H(type))
				{
					Item itemOffHand = _equipment.GetEquipment(EquipmentSlotId.Off_Hand);
					if (itemOffHand != null)
					{
						if (Item.IsQuiver(itemOffHand.ItemType) && (item.Attributes[GameAttribute.Bow] > 0 || _owner.Attributes[GameAttribute.Crossbow] > 0))
							return true;

						if (Item.IsShield(itemOffHand.ItemType) && _owner.Attributes[GameAttribute.Allow_2H_And_Shield])
							if (!Item.IsBow(type) && (_owner.Attributes[GameAttribute.Crossbow] <= 0))
								return true;

						_equipment.UnequipItem(itemOffHand);
						RemoveItemFromDB(itemOffHand);
						if (!_inventoryGrid.AddItem(itemOffHand))
						{
							// unequip failed, put back
							_equipment.EquipItem(itemOffHand, (int)EquipmentSlotId.Off_Hand);
							return false;
						}
						AcceptMoveRequest(itemOffHand);
					}
				}
			}
			else if (equipmentSlot == (int)EquipmentSlotId.Off_Hand)
			{
				Item itemMainHand = _equipment.GetEquipment(EquipmentSlotId.Main_Hand);
				if (Item.Is2H(type))
				{
					//remove object first to make room for possible unequiped item
					//_inventoryGrid.RemoveItem(item);

					if (itemMainHand != null)
					{
						_equipment.UnequipItem(itemMainHand);
						RemoveItemFromDB(itemMainHand);
						_inventoryGrid.AddItem(itemMainHand);
						AcceptMoveRequest(itemMainHand);
					}

					//_equipment.EquipItem(item, (int)EquipmentSlotId.Main_Hand); //why it's here? it's just a check or not?
					AcceptMoveRequest(item);

					SendVisualInventory(_owner);

					return true;
				}
				else if (Item.IsQuiver(type))
				{
					return (_owner.Attributes[GameAttribute.Bow] > 0 || _owner.Attributes[GameAttribute.Crossbow] > 0);
				}
				else if (Item.IsShield(type) && _owner.Attributes[GameAttribute.Allow_2H_And_Shield])
				{
					return true;
				}

				if (itemMainHand != null)
				{
					if (Item.Is2H(itemMainHand.ItemType))
					{
						_equipment.UnequipItem(itemMainHand);
						RemoveItemFromDB(itemMainHand);
						if (!_inventoryGrid.AddItem(itemMainHand))
						{
							// unequip failed, put back
							_equipment.EquipItem(itemMainHand, (int)EquipmentSlotId.Main_Hand);
							return false;
						}
						AcceptMoveRequest(itemMainHand);
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Transfers an amount from one stack to a free space
		/// </summary>
		public void OnInventorySplitStackMessage(InventorySplitStackMessage msg)
		{
			Logger.Trace("OnInventorySplitStackMessage()");
			Item itemFrom = GetItemByDynId(_owner, (uint)msg.FromID);
			int amount = Math.Min((int)msg.Amount, itemFrom.Attributes[GameAttribute.ItemStackQuantityLo] - 1);
			itemFrom.UpdateStackCount(itemFrom.Attributes[GameAttribute.ItemStackQuantityLo] - amount);
			itemFrom.Attributes.SendChangedMessage(_owner.InGameClient);

			Item item = ItemGenerator.CreateItem(_owner, itemFrom.ItemDefinition);
			item.Owner = _owner;

			InventoryGrid targetGrid = (msg.InvLoc.EquipmentSlot == (int)EquipmentSlotId.Stash) ? _stashGrid : _inventoryGrid;
			SaveItemToDB(_owner.Toon.GameAccount.DBGameAccount, _owner.Toon.DBToon, EquipmentSlotId.Inventory, item);
			ChangeItemLocationDB(msg.InvLoc.Column, msg.InvLoc.Row, item);
			item.UpdateStackCount(amount);
			targetGrid.PlaceItem(item, msg.InvLoc.Row, msg.InvLoc.Column);
		}

		/// <summary>
		/// Transfers an amount from one stack to another
		/// </summary>
		public void OnInventoryStackTransferMessage(InventoryStackTransferMessage msg)
		{
			Item itemFrom = GetItemByDynId(_owner, msg.FromID);
			Item itemTo = GetItemByDynId(_owner, msg.ToID);
			int amount = Math.Min((int)msg.Amount, itemFrom.Attributes[GameAttribute.ItemStackQuantityLo] - 1);

			itemFrom.UpdateStackCount(itemFrom.Attributes[GameAttribute.ItemStackQuantityLo] - amount);
			itemTo.UpdateStackCount(itemTo.Attributes[GameAttribute.ItemStackQuantityLo] + amount);

			itemFrom.Attributes.SendChangedMessage(_owner.InGameClient);
			itemTo.Attributes.SendChangedMessage(_owner.InGameClient);
		}

		private void OnInventoryDropItemMessage(InventoryDropItemMessage msg)
		{
			if (_owner.World != null && _owner.World.IsPvP) return;
			var item = GetItemByDynId(_owner, msg.ItemID);
			if (item == null)
				return; // TODO: Throw smthg? /fasbat

			if (_equipment.IsItemEquipped(item))
			{
				_equipment.UnequipItem(item);
				SendVisualInventory(_owner);
				RemoveItemFromDB(item);
			}
			else
			{
				var sourceGrid = (item.InvLoc(_owner).EquipmentSlot == 0 ? _inventoryGrid : _stashGrid);
				sourceGrid.RemoveItem(item);
			}

			item.CurrentState = ItemState.Dropping;
			item.Unreveal(_owner);
			item.SetNewWorld(_owner.World);
			_owner.World.DropItem(_owner, item);
			//if (item.DBInventory != null)
			//_dbInventoriesToDelete.Add(item.DBInventory);
			item.DBInventory = null;
			item.CurrentState = ItemState.Normal;
			AcceptMoveRequest(item);
		}

		public void Consume(GameClient client, GameMessage message)
		{
			if (client.Game.PvP) return;
			if (_owner.IsCasting) _owner.StopCasting();
			if (message is InventoryRequestMoveMessage moveMessage) HandleInventoryRequestMoveMessage(moveMessage);
			else if (message is InventoryRequestQuickMoveMessage quickMoveMessage) HandleInventoryRequestQuickMoveMessage(quickMoveMessage);
			else if (message is InventorySplitStackMessage stackMessage) OnInventorySplitStackMessage(stackMessage);
			else if (message is InventoryStackTransferMessage transferMessage) OnInventoryStackTransferMessage(transferMessage);
			else if (message is InventoryDropItemMessage dropItemMessage) OnInventoryDropItemMessage(dropItemMessage);
			else if (message is InventoryRequestUseMessage useMessage) OnInventoryRequestUseMessage(useMessage);
			else if (message is InventoryRequestSocketMessage socketMessage) OnSocketMessage(socketMessage);
			else if (message is InventoryGemsExtractMessage extractMessage) OnGemsExtractMessage(extractMessage);
			else if (message is RequestBuySharedStashSlotsMessage slotsMessage) OnBuySharedStashSlots(slotsMessage);
			else if (message is InventoryIdentifyItemMessage identifyItemMessage) OnInventoryIdentifyItemMessage(identifyItemMessage);
			else if (message is InventoryUseIdentifyItemMessage itemMessage) OnInventoryUseIdentifyItemMessage(itemMessage);
			else if (message is TrySalvageMessage salvageMessage) OnTrySalvageMessage(salvageMessage);
			else if (message is TrySalvageAllMessage allMessage) OnTrySalvageAllMessage(allMessage);
			else if (message is CraftItemsMessage itemsMessage) OnCraftItemMessage(client, itemsMessage);
			else if (message is EnchantAffixMessage affixMessage) OnEnchantAffixMessage(client, affixMessage);
			else if (message is TryTransmogItemMessage transmogItemMessage) OnTryTransmogItemMessage(client, transmogItemMessage);
			else if (message is DyeItemMessage dyeItemMessage) OnDyeItemMessage(client, dyeItemMessage);
			else if (message is InventoryRepairAllMessage) RepairAll();
			else if (message is InventoryRepairEquippedMessage) RepairEquipment();

			if (_equipment.EquipmentChanged)
			{
				_owner.World.BuffManager.RemoveAllBuffs(_owner, false);
				_owner.SetAttributesByItems();
				_owner.SetAttributesByItemProcs();
				_owner.SetAttributesByGems();
				_owner.SetAttributesByItemSets();
				_owner.SetAttributesByPassives();
				_owner.SetAttributesByParagon();
				_owner.SetAttributesSkillSets();
				CheckWeapons();
				_owner.Attributes.BroadcastChangedIfRevealed();
				_owner.SaveStats();
				_owner.UpdatePercentageHP(_owner.PercHPbeforeChange);
				_owner.Toon.PvERating = GetGearScore();
				_equipment.EquipmentChanged = false;
				_owner.ToonStateChanged();
			}
		}
		private void OnDyeItemMessage(GameClient client, DyeItemMessage msg)
		{
			_equipment.EquipmentChanged = true;
			var Item = GetItemByDynId(_owner, msg.ItemID);
			;
			Item.Attributes[GameAttribute.DyeType] = msg.DyeID;
			Item.Attributes.BroadcastChangedIfRevealed();
			RefreshInventoryToClient();
		}

		private void OnEnchantAffixMessage(GameClient client, EnchantAffixMessage msg)
		{
			_equipment.EquipmentChanged = true;
			List<Affix> ListWithoutNo = new List<Affix>();
			Affix ReloadAffix = null;

			var EnchData = //(DiIiS_NA.Core.MPQ.FileFormats.GameBalance.EnchantItemAffixUseCountCostScalarsTables)
				MPQStorage.Data.Assets[SNOGroup.GameBalance][346698].Data;

			var Item = GetItemByDynId(_owner, (uint)msg.Field0);
			foreach (var aff in Item.AffixList)
				if (aff.AffixGbid != msg.GBIDAffixToReroll)
					ListWithoutNo.Add(aff);
				else
					ReloadAffix = aff;

			if (ReloadAffix == null) return;
			#region Поиск аффикса
			bool IsUnique = Item.ItemDefinition.Name.Contains("Unique_");
			List<int> itemTypes = ItemGroup.HierarchyToHashList(Item.ItemType);
			//itemGroups.Add(ItemGroup.GetRootType(item.ItemType));
			int levelToFind = (Item.ItemLevel < 5 ? 0 : Math.Min(Item.ItemLevel, 70));
			if (Item.GBHandle.GBID == -4139386) levelToFind = 10;
			if (itemTypes[0] == 828360981)
				itemTypes.Add(-1028103400);
			else if (itemTypes[0] == -947867741)
				itemTypes.Add(3851110);
			else if (itemTypes[0] == 110504)
				itemTypes.Add(395678127);
			else if (itemTypes[0] == 151398954)
				itemTypes.Add(140775496);


			Class ItemPlayerClass = Class.None;
			if (Item.ItemType.Usable.HasFlag(ItemFlags.Barbarian)) ItemPlayerClass = Class.Barbarian;
			if (Item.ItemType.Usable.HasFlag(ItemFlags.Crusader)) ItemPlayerClass = Class.Crusader;
			if (Item.ItemType.Usable.HasFlag(ItemFlags.Necromancer)) ItemPlayerClass = Class.Necromancer;
			if (Item.ItemType.Usable.HasFlag(ItemFlags.DemonHunter)) ItemPlayerClass = Class.DemonHunter;
			if (Item.ItemType.Usable.HasFlag(ItemFlags.Wizard)) ItemPlayerClass = Class.Wizard;
			if (Item.ItemType.Usable.HasFlag(ItemFlags.WitchDoctor)) ItemPlayerClass = Class.Witchdoctor;
			if (Item.ItemType.Usable.HasFlag(ItemFlags.Monk)) ItemPlayerClass = Class.Monk;

			var filteredList = AffixGenerator.AllAffix.Where(a =>
				//(a.PlayerClass == ItemPlayerClass || a.PlayerClass == Class.None) &&//(a.PlayerClass == ItemPlayerClass || a.PlayerClass == Class.None) &&
				itemTypes.ContainsAtLeastOne(a.ItemGroup) &&
				(a.AffixLevelMax <= ReloadAffix.Definition.AffixLevelMax) &&
				(a.AffixLevelMin >= ReloadAffix.Definition.AffixLevelMin) &&
				(a.OverrideLevelReq <= Item.ItemDefinition.RequiredLevel)
				);

			if (!ReloadAffix.Definition.Name.Contains("Secondary")) filteredList = filteredList.Where( a => !a.Name.Contains("Secondary") );
			if (!ReloadAffix.Definition.Name.Contains("Experience")) filteredList = filteredList.Where(a => !a.Name.Contains("Experience"));
			if (!ReloadAffix.Definition.Name.Contains("Archon")) filteredList = filteredList.Where(a => !a.Name.Contains("Archon"));
			if (ReloadAffix.Definition.Hash == ReloadAffix.Definition.Hash) filteredList = filteredList.Where(a => a.Hash != ReloadAffix.Definition.Hash);
			if (Item.GBHandle.GBID == -4139386) filteredList = filteredList.Where( a => !a.Name.Contains("Str") && !a.Name.Contains("Dex") && !a.Name.Contains("Int") && !a.Name.Contains("Vit" ));

			Dictionary<int, AffixTable> bestDefinitions = new Dictionary<int, AffixTable>();

			foreach (var affix_group in filteredList.GroupBy(a => a.AffixFamily0))
			{
				if (Item.AffixFamilies.Contains(affix_group.First().AffixFamily0)) continue;
				int s = Item.ItemDefinition.RequiredLevel;

				bestDefinitions[affix_group.First().AffixFamily0] = affix_group.ToList()[FastRandom.Instance.Next(0, 1)];
			}

			var SocketsAffixs = AffixGenerator.AllAffix.Where(a => a.Name.ToLower().Contains("1xx_socket") && itemTypes.ContainsAtLeastOne(a.ItemGroup)).ToList();

			//if (bestDefinitions.Values.Where(a => a.Name.Contains("PoisonD")).Count() > 0) Logger.Debug("PoisonD in bestDefinitions");
			List<AffixTable> selectedGroups = bestDefinitions.Values
				.OrderBy(x => FastRandom.Instance.Next()) //random order
				.GroupBy(x => (x.AffixFamily1 == -1) ? x.AffixFamily0 : x.AffixFamily1)
				.Select(x => x.First()) //only one from group
				.Take(1) //take needed amount
				.ToList();
			if (selectedGroups.Count == 0)
				if (ReloadAffix.Definition.Name.ToLower().Contains("socket"))
					selectedGroups = SocketsAffixs.Where(x => x.OverrideLevelReq <= ReloadAffix.Definition.AffixLevelMax //&& x.AffixLevelMin == ReloadAffix.Definition.AffixLevelMin
					).OrderBy(x => FastRandom.Instance.Next()).Take(1).ToList();
				else
					return;

			#region Удаление действующего аффикса
			foreach (var effect in ReloadAffix.Definition.AttributeSpecifier)
			{
				//if (def.Name.Contains("Sockets")) Logger.Info("socket affix attribute: {0}, {1}", effect.AttributeId, effect.SNOParam);
				if (effect.AttributeId > 0)
				{
					float result;
					float minValue;
					float maxValue;

					if (Item.RandomGenerator == null)
						Item.RandomGenerator = new ItemRandomHelper(Item.Attributes[GameAttribute.Seed]);

					if (FormulaScript.Evaluate(effect.Formula.ToArray(), Item.RandomGenerator, out result, out minValue, out maxValue))
					{
						if (effect.AttributeId == 369) continue; //Durability_Max
						if (effect.AttributeId == 380) //Sockets
						{
							result = Math.Max(result, 1f);
							if (ReloadAffix.Definition.Name.Contains("Chest"))
								result = (float)FastRandom.Instance.Next(1, 4);
							if (ReloadAffix.Definition.Name.Contains("Bracer"))
								result = (float)FastRandom.Instance.Next(1, 3);
						}
						
						if (GameAttribute.Attributes[effect.AttributeId] is GameAttributeF)
						{
							var attr = GameAttribute.Attributes[effect.AttributeId] as GameAttributeF;
							if (effect.SNOParam != -1)
								Item.Attributes[attr, effect.SNOParam] -= result;
							else
								Item.Attributes[attr] -= result;
						}
						else if (GameAttribute.Attributes[effect.AttributeId] is GameAttributeI)
						{
							var attr = GameAttribute.Attributes[effect.AttributeId] as GameAttributeI;
							if (effect.SNOParam != -1)
								Item.Attributes[attr, effect.SNOParam] -= (int)result;
							else
								Item.Attributes[attr] -= (int)result;
						}
					}
				}
			}
			Item.Attributes.BroadcastChangedIfRevealed();
			#endregion


			foreach (var def in selectedGroups)
			{
				if (def != null) 
				{
					List<float> Scores = new List<float>();
					foreach (var effect in def.AttributeSpecifier)
					{
						//if (def.Name.Contains("Sockets")) Logger.Info("socket affix attribute: {0}, {1}", effect.AttributeId, effect.SNOParam);
						if (effect.AttributeId > 0)
						{
							float result;
							float minValue;
							float maxValue;

							if (Item.RandomGenerator == null)
								Item.RandomGenerator = new ItemRandomHelper(Item.Attributes[GameAttribute.Seed]);
							
							if (FormulaScript.Evaluate(effect.Formula.ToArray(), Item.RandomGenerator, out result, out minValue, out maxValue))
							{
								if (effect.AttributeId == 369) continue; //Durability_Max
								if (effect.AttributeId == 380) //Sockets
								{
									result = Math.Max(result, 1f);
									if (def.Name.Contains("Chest"))
										result = (float)FastRandom.Instance.Next(1, 4);
									if (def.Name.Contains("Bracer"))
										result = (float)FastRandom.Instance.Next(1, 3);
								}
								float score = (minValue == maxValue ? 0.5f : ((result - minValue) / (maxValue - minValue)));
								Scores.Add(score);
								//Logger.Debug("Randomized value for attribute " + GameAttribute.Attributes[effect.AttributeId].Name + "(" + minValue + "," + maxValue + ")" + " is " + result + ", score is " + score);
								//var tmpAttr = GameAttribute.Attributes[effect.AttributeId];
								//var attrName = tmpAttr.Name;
								//


								if (GameAttribute.Attributes[effect.AttributeId] is GameAttributeF)
								{
									var attr = GameAttribute.Attributes[effect.AttributeId] as GameAttributeF;
									if (effect.SNOParam != -1)
										Item.Attributes[attr, effect.SNOParam] += result;
									else
										Item.Attributes[attr] += result;
								}
								else if (GameAttribute.Attributes[effect.AttributeId] is GameAttributeI)
								{
									var attr = GameAttribute.Attributes[effect.AttributeId] as GameAttributeI;
									if (effect.SNOParam != -1)
										Item.Attributes[attr, effect.SNOParam] += (int)result;
									else
										Item.Attributes[attr] += (int)result;
								}
								//Logger.Debug("{0} - Str: {1} ({2})", n, result, item.Attributes[GameAttribute.Attributes[effect.AttributeId] as GameAttributeF]);
							}
						}
					}
					var affix = new Affix(def.Hash);
					affix.Score = (Scores.Count == 0 ? 0 : Scores.Average());
					//Logger.Debug("Affix  " + def.Hash + ", final score is" + affix.Score);
					Item.AffixList.Clear();
					foreach (var Aff in ListWithoutNo)
						Item.AffixList.Add(Aff);
					Item.AffixList.Add(affix);

					if (!IsUnique && !Item.ItemDefinition.Name.Contains("StaffOfCow"))
					{
						Item.RareItemName = AffixGenerator.GenerateItemName();
					}

					var affixGbis = new int[Item.AffixList.Count];
					for (int i = 0; i < Item.AffixList.Count; i++)
					{
						affixGbis[i] = Item.AffixList[i].AffixGbid;
					}
					_owner.InGameClient.SendMessage(new AffixMessage()
					{
						ActorID = Item.DynamicID(_owner),
						Field1 = (Item.Unidentified ? 0x00000002 : 0x00000001),
						aAffixGBIDs = affixGbis,
					});
					
					//*/
				}
			}
			#endregion
			_owner.GrantCriteria(74987255495718);
			
		}
		private void OnTrySalvageAllMessage(TrySalvageAllMessage msg)
		{
			int last_item_gbid = 0;
			int last_item_quality = 0;
			int count_reward = 0;
			switch (msg.SalvageType)
			{
				
				// Simple items
				case 0:
					foreach (var item in GetBackPackItems())
						if (!item.ItemDefinition.Name.ToLower().Contains("potion") &&
							!item.ItemDefinition.Name.ToLower().Contains("gem") &&
							!item.ItemType.Name.ToLower().Contains("gem") &&
							!item.ItemDefinition.Name.ToLower().Contains("book") &&
							!item.ItemDefinition.Name.ToLower().Contains("potion") &&
							!item.ItemDefinition.Name.ToLower().Contains("plan") &&
							!item.ItemDefinition.Name.ToLower().Contains("key") &&
							!item.ItemDefinition.Name.ToLower().Contains("horadric") &&
							!item.ItemDefinition.Name.ToLower().Contains("dye"))
							if (item.Attributes[GameAttribute.Item_Quality_Level] < 3)
							{
								last_item_gbid = item.GBHandle.GBID;
								last_item_quality = item.Attributes[GameAttribute.Item_Quality_Level];
								count_reward += SalvageItem(item);
							}
					break;
				// Magical items
				case 1:
					foreach (var item in GetBackPackItems())
						if (item.Attributes[GameAttribute.Item_Quality_Level] > 2 &
							item.Attributes[GameAttribute.Item_Quality_Level] < 6)
						{
							last_item_gbid = item.GBHandle.GBID;
							last_item_quality = item.Attributes[GameAttribute.Item_Quality_Level];
							count_reward += SalvageItem(item);
						}
					break;
				// Rare Items
				case 2:
					foreach (var item in GetBackPackItems())
						if (item.Attributes[GameAttribute.Item_Quality_Level] > 5 &
							item.Attributes[GameAttribute.Item_Quality_Level] < 9)
						{
							last_item_gbid = item.GBHandle.GBID;
							last_item_quality = item.Attributes[GameAttribute.Item_Quality_Level];
							count_reward += SalvageItem(item);
						}
					break;
			}
			//6-8 Rare
			//3-5 Magic
			//012
			_owner.InGameClient.SendMessage(new SalvageResultsMessage()
			{
				gbidOriginalItem = last_item_gbid,
				IQLOriginalItem = Math.Min(last_item_quality, 9),
				MaterialsResults = 1,
				gbidNewItems = new int[] { msg.SalvageType == 0 ? -363607620 : msg.SalvageType == 1 ? -1585802162 : -605947593, -1, -1, -1 },
				MaterialsCounts = new int[] { count_reward, 0, 0, 0 }
			});
		}
		private int SalvageItem(Item salvageitem)
		{
			var item = salvageitem;
			if (item == null)
				return 0;

			if (item.Attributes[GameAttribute.IsCrafted] == true && item.DBInventory.Version > 1)
				return 0;

			if (item.Attributes[GameAttribute.Item_Equipped] == true)
				return 0;

			string rewardName = "Crafting_";

			if (item.ItemLevel >= 60)
			{
				if (item.Attributes[GameAttribute.Item_Quality_Level] > 5)
					rewardName += "Rare_01";                            //Veiled Crystal
				else
					if (item.Attributes[GameAttribute.Item_Quality_Level] > 2)
					rewardName += "Magic_01";                       //Arcane Dust
				else
					rewardName += "AssortedParts_01";               //Reusable Parts
			}
			else
				if (item.Attributes[GameAttribute.Item_Quality_Level] > 5)
				rewardName += "Rare_01";                               //Iridescent Tear
			else
					if (item.Attributes[GameAttribute.Item_Quality_Level] > 2)
				rewardName += "Magic_01";                           //Exquisite Essence
			else
				rewardName += "AssortedParts_01";                           //Common Debris

			Item reward = ItemGenerator.Cook(_owner, rewardName);
			int count_reward = RandomHelper.Next(1, 5) * (10 - item.Attributes[GameAttribute.Item_Quality_Level]);
			var playerAcc = _owner.Toon.GameAccount;
			if (reward == null) return 0;
			for (int i = 0; i < count_reward; i++)
			{
				switch (rewardName)
				{
					case "Crafting_AssortedParts_01": playerAcc.CraftItem1++; break;
					case "Crafting_Magic_01": playerAcc.CraftItem2++; break;
					case "Crafting_Rare_01": playerAcc.CraftItem3++; break;
				}
				//Item reward1 = ItemGenerator.Cook(_owner, rewardName);
				//_inventoryGrid.AddItem(reward1);
			}
			//reward.Owner = _owner;

			if (item.DBInventory.FirstGem != -1)
			{
				_owner.InGameClient.SendMessage(new GemNotificationMessage());

				_inventoryGrid.AddItem(ItemGenerator.CookFromDefinition(_owner.World, ItemGenerator.GetItemDefinition(item.DBInventory.FirstGem)));

				if (item.DBInventory.SecondGem != -1)
					_inventoryGrid.AddItem(ItemGenerator.CookFromDefinition(_owner.World, ItemGenerator.GetItemDefinition(item.DBInventory.SecondGem)));
				if (item.DBInventory.ThirdGem != -1)
					_inventoryGrid.AddItem(ItemGenerator.CookFromDefinition(_owner.World, ItemGenerator.GetItemDefinition(item.DBInventory.ThirdGem)));

				foreach (var gem in item.Gems)
					gem.Unreveal(_owner);

				item.Gems.Clear();
				item.Attributes[GameAttribute.Sockets_Filled] = 0;
				item.Attributes.BroadcastChangedIfRevealed();
			}

			_inventoryGrid.RemoveItem(item);
			item.Unreveal(_owner);

			bool haveBrimstone = false;
			Item brimstone = null;
			if (item.Attributes[GameAttribute.Item_Quality_Level] > 8 || FastRandom.Instance.Next(1, 1000) == 1)
			{
				if (item.ItemLevel >= 60)
					rewardName = "Crafting_Legendary_01";       //Forgotten Soul
				else
					rewardName = "Crafting_Legendary_01";           //Fiery Brimstone
				brimstone = ItemGenerator.Cook(_owner, rewardName);
				if (brimstone != null)
				{
					_owner.Toon.GameAccount.CraftItem5++;
					//_inventoryGrid.AddItem(brimstone);
					haveBrimstone = true;
				}
			}

			UpdateCurrencies();

			_owner.CheckSalvageItemCriteria(reward.GBHandle.GBID);
			if (haveBrimstone)
				_owner.CheckSalvageItemCriteria(brimstone.GBHandle.GBID);

			if (Item.IsWeapon(item.ItemType))
				_owner.GrantCriteria(74987243307733);
			if (Item.IsArmor(item.ItemType))
				_owner.GrantCriteria(74987243307734);
			if (Item.IsAmulet(item.ItemType))
				_owner.GrantCriteria(74987243307735);
			if (Item.IsRing(item.ItemType))
				_owner.GrantCriteria(74987243309909);

			//DestroyInventoryItem(item);
			return count_reward;
		}

		private void OnInventoryIdentifyItemMessage(InventoryIdentifyItemMessage msg)
		{
			var item = GetItemByDynId(_owner, msg.ItemID);
			if (item == null)
				return;
			Logger.Warn("Identifying items not implemented yet");
		}
		
		private void OnInventoryUseIdentifyItemMessage(InventoryUseIdentifyItemMessage msg)
		{
			var item = GetItemByDynId(_owner, msg.ItemID);
			if (item == null)
				return;

			int idDuration = 60;
			_owner.StartCasting(idDuration, new Action(() => {
				item.Identify();
			}));
		}
		//*
		private void OnTrySalvageMessage(TrySalvageMessage msg)
		{
			
			var item = GetItemByDynId(_owner, msg.ActorID);
			if (item == null)
				return;

			//if (item.Attributes[GameAttribute.IsCrafted] == true)// && item.DBInventory.Version > 1)
			//	return;

			if (item.Attributes[GameAttribute.Item_Equipped] == true)
				return;

			string rewardName = "Crafting_";

			if (item.ItemLevel >= 60)
			{
				if (item.Attributes[GameAttribute.Item_Quality_Level] > 5)
					rewardName += "Rare_01";                            //Veiled Crystal
				else
					if (item.Attributes[GameAttribute.Item_Quality_Level] > 2)
					rewardName += "Magic_01";                       //Arcane Dust
				else
					rewardName += "AssortedParts_01";               //Reusable Parts
			}
			else
				if (item.Attributes[GameAttribute.Item_Quality_Level] > 5)
				rewardName += "Rare_01";                               //Iridescent Tear
			else
					if (item.Attributes[GameAttribute.Item_Quality_Level] > 2)
				rewardName += "Magic_01";                           //Exquisite Essence
			else
				rewardName += "AssortedParts_01";                           //Common Debris

			Item reward = ItemGenerator.Cook(_owner, rewardName);
			int count_reward = RandomHelper.Next(1, 5) * (10 - item.Attributes[GameAttribute.Item_Quality_Level]);
			if (item.Attributes[GameAttribute.IsCrafted] == true)
				count_reward /= 4;
			if (reward == null) return;
			for (int i = 0; i < count_reward; i++)
			{
				switch (rewardName)
				{
					case "Crafting_AssortedParts_01": _owner.Toon.GameAccount.CraftItem1++; break;
					case "Crafting_Magic_01": _owner.Toon.GameAccount.CraftItem2++; break;
					case "Crafting_Rare_01": _owner.Toon.GameAccount.CraftItem3++; break;
				}
			}
			//reward.Owner = _owner;

			if (item.DBInventory.FirstGem != -1)
			{
				_owner.InGameClient.SendMessage(new GemNotificationMessage());

				_inventoryGrid.AddItem(ItemGenerator.CookFromDefinition(_owner.World, ItemGenerator.GetItemDefinition(item.DBInventory.FirstGem)));

				if (item.DBInventory.SecondGem != -1)
					_inventoryGrid.AddItem(ItemGenerator.CookFromDefinition(_owner.World, ItemGenerator.GetItemDefinition(item.DBInventory.SecondGem)));
				if (item.DBInventory.ThirdGem != -1)
					_inventoryGrid.AddItem(ItemGenerator.CookFromDefinition(_owner.World, ItemGenerator.GetItemDefinition(item.DBInventory.ThirdGem)));

				foreach (var gem in item.Gems)
					gem.Unreveal(_owner);

				item.Gems.Clear();
				item.Attributes[GameAttribute.Sockets_Filled] = 0;
				item.Attributes.BroadcastChangedIfRevealed();
			}

			_inventoryGrid.RemoveItem(item);
			item.Unreveal(_owner);
			
			
			bool haveBrimstone = false;
			Item brimstone = null;
			if (item.Attributes[GameAttribute.Item_Quality_Level] > 8 || FastRandom.Instance.Next(1, 1000) == 1)
			{
				if (item.ItemLevel >= 60)
					rewardName = "Crafting_Legendary_01";       //Forgotten Soul
				else
					rewardName = "Crafting_Legendary_01";           //Fiery Brimstone
				brimstone = ItemGenerator.Cook(_owner, rewardName);
				if (brimstone != null)
				{
					_owner.Toon.GameAccount.CraftItem5++;
					haveBrimstone = true;
				}
			}

			_owner.InGameClient.SendMessage(new SalvageResultsMessage()
			{
				gbidOriginalItem = item.GBHandle.GBID,
				IQLOriginalItem = Math.Min(item.Attributes[GameAttribute.Item_Quality_Level], 9),
				MaterialsResults = haveBrimstone ? 2 : 1,
				gbidNewItems = new int[] { reward.GBHandle.GBID, haveBrimstone ? brimstone.GBHandle.GBID : -1, -1, -1 }, 
				MaterialsCounts = new int[] { count_reward, haveBrimstone ? 1 : 0, 0, 0 }
				
			});

			UpdateCurrencies();

			_owner.CheckSalvageItemCriteria(reward.GBHandle.GBID);
			if (haveBrimstone)
				_owner.CheckSalvageItemCriteria(brimstone.GBHandle.GBID);

			if (Item.IsWeapon(item.ItemType))
				_owner.GrantCriteria(74987243307733);
			if (Item.IsArmor(item.ItemType))
				_owner.GrantCriteria(74987243307734);
			if (Item.IsAmulet(item.ItemType))
				_owner.GrantCriteria(74987243307735);
			if (Item.IsRing(item.ItemType))
				_owner.GrantCriteria(74987243309909);

			//DestroyInventoryItem(item);

		}
		//*/
		private bool _achievementGranted = false;
		private bool _radiantAchievementGranted = false;
		//*
		private void OnCraftItemMessage(GameClient client, CraftItemsMessage msg)
		{
			var recipeGBId = msg.GBIDRecipe;
			var recipeDefinition = ItemGenerator.GetRecipeDefinition(recipeGBId);
			//if (!this._owner.RecipeAvailable(recipeDefinition)) return;
			var recipe = (Recipe)MPQStorage.Data.Assets[SNOGroup.Recipe][recipeDefinition.SNORecipe].Data;
			var extraAffixCount = recipe.ItemSpecifierData.AdditionalRandomAffixes + FastRandom.Instance.Next(0, recipe.ItemSpecifierData.AdditionalRandomAffixesDelta);

			Item reward = ItemGenerator.CookFromDefinition(_owner.World, ItemGenerator.GetItemDefinition(recipe.ItemSpecifierData.ItemGBId), Math.Min(extraAffixCount, 9), false, true);
			reward.Attributes[GameAttribute.ItemStackQuantityLo] = 0;
			if (!Item.IsAmulet(reward.ItemType) && !Item.IsRing(reward.ItemType))
			{
				if (reward.ItemLevel >= 70)
					_owner.GrantCriteria(74987254004798);
			}
			else
			{
				if (reward.ItemLevel >= 70)
					_owner.GrantCriteria(74987254245219);
			}

			if (GetGoldAmount() < recipeDefinition.Gold) return;
			bool haveEnoughIngredients = true;

			foreach (var ingr in recipeDefinition.Ingredients) 
			{
				if (ingr.ItemsGBID == -1 || ingr.ItemsGBID == 0) continue;
				switch (ingr.ItemsGBID)
				{
					case -363607620: // Common parts.
						if(_owner.Toon.GameAccount.CraftItem1 < ingr.Count)
							haveEnoughIngredients = false;
						break;
					case -1585802162: // Wizard Dust.
						if (_owner.Toon.GameAccount.CraftItem2 < ingr.Count)
							haveEnoughIngredients = false;
						break;
					case -605947593: // Blurred Crystal.
						if (_owner.Toon.GameAccount.CraftItem3 < ingr.Count)
							haveEnoughIngredients = false;
						break;
				}
				if (Item.IsGem(reward.ItemType))
				{
					Item FoundedItem = null;
					foreach (var item in GetBackPackItems())
						if (item.ItemDefinition.Hash == ingr.ItemsGBID)
						{
							FoundedItem = item;
							if (FoundedItem.Attributes[GameAttribute.ItemStackQuantityLo] == ingr.Count)
							{
								_inventoryGrid.RemoveItem(FoundedItem);
								FoundedItem.Unreveal(FoundedItem.Owner as Player);
							}
							else if (FoundedItem.Attributes[GameAttribute.ItemStackQuantityLo] > ingr.Count)
							{
								FoundedItem.Attributes[GameAttribute.ItemStackQuantityLo] -= ingr.Count;
								FoundedItem.Attributes.BroadcastChangedIfRevealed();
							}
							else
								return;
						}
					if (FoundedItem == null)
						foreach (var item in GetStashItems())
							if (item.ItemDefinition.Hash == ingr.ItemsGBID)
							{
								if (item.Attributes[GameAttribute.ItemStackQuantityLo] == ingr.Count)
								{
									_stashGrid.RemoveItem(item);
									item.Unreveal(item.Owner as Player);
								}
								else if(item.Attributes[GameAttribute.ItemStackQuantityLo] > ingr.Count)
								{
									item.Attributes[GameAttribute.ItemStackQuantityLo] -= ingr.Count;
									item.Attributes.BroadcastChangedIfRevealed();
								}
								else
									return;
							}

				}
			}
			if (!haveEnoughIngredients) return;

			foreach (var ingr in recipeDefinition.Ingredients)
			{
				if (ingr.ItemsGBID == -1 || ingr.ItemsGBID == 0) continue;
				switch (ingr.ItemsGBID)
				{
					case -363607620: // Common parts.
							_owner.Toon.GameAccount.CraftItem1 -= ingr.Count;
						break;
					case -1585802162: // Wizard Dust.
							_owner.Toon.GameAccount.CraftItem2 -= ingr.Count;
						break;
					case -605947593: // Blurred Crystal.
							_owner.Toon.GameAccount.CraftItem3 -= ingr.Count;
						break;
				}
			}
			RemoveGoldAmount(recipeDefinition.Gold);

			if (Item.IsGem(reward.ItemType))
				reward.Attributes[GameAttribute.Item_Quality_Level] = 1;
			else
			{
				foreach (int affixId in recipe.ItemSpecifierData.GBIdAffixes)
					if (affixId != -1)
						AffixGenerator.AddAffix(reward, affixId, true);

				if (reward.ItemDefinition.Name.StartsWith("Unique_"))
				{
					AffixGenerator.Generate(reward, recipe.ItemSpecifierData.AdditionalRandomAffixes, true);
					reward.Attributes[GameAttribute.Item_Quality_Level] = 9;
				}
				//else
				if (!(recipeDefinition.Name.StartsWith("T12_") || recipeDefinition.Name.StartsWith("T11_")))
					reward.Attributes[GameAttribute.Item_Quality_Level] = Math.Min(recipe.ItemSpecifierData.AdditionalRandomAffixes + 2 , 9);
				if (reward.Attributes[GameAttribute.Item_Quality_Level] < 9)
				{
					AffixGenerator.Generate(reward, recipe.ItemSpecifierData.AdditionalRandomAffixes, true);
				}

				//reward.Attributes[GameAttribute.IsCrafted] = true;
			}
			reward.Attributes[GameAttribute.IsCrafted] = true;
			reward.Attributes[GameAttribute.Attachment_Handled_By_Client] = true;
			reward.Attributes[GameAttribute.ItemStackQuantityLo]++;

			if (Item.IsGem(reward.ItemType))
			{
				if (!_achievementGranted)
				{
					_achievementGranted = true;
					_owner.GrantAchievement(74987243307784);
				}
				if (_owner.Toon.IsSeasoned)
					if (Int32.Parse(reward.ItemDefinition.Name.Split('_')[2]) >= 7)
					{
						_owner.GrantCriteria(74987245885431);
						//74987245885431
					}

				if (reward.ItemDefinition.Name.Contains("_06") && !_radiantAchievementGranted)
				{
					_radiantAchievementGranted = true;
					_owner.GrantAchievement(74987243307785);
				}

				if (reward.ItemDefinition.Name.Contains("Amethyst"))
					_owner.GrantCriteria(74987243308254);
				if (reward.ItemDefinition.Name.Contains("Emerald"))
					_owner.GrantCriteria(74987243309059);
				if (reward.ItemDefinition.Name.Contains("Ruby"))
					_owner.GrantCriteria(74987243309060);
				if (reward.ItemDefinition.Name.Contains("Topaz"))
					_owner.GrantCriteria(74987243309061);

				if (reward.ItemDefinition.Name.Contains("_14"))
				{
					_owner.GrantAchievement(74987243307787);

					if (reward.ItemDefinition.Name.Contains("Amethyst"))
						_owner.GrantCriteria(74987243309067);
					if (reward.ItemDefinition.Name.Contains("Emerald"))
						_owner.GrantCriteria(74987243308256);
					if (reward.ItemDefinition.Name.Contains("Ruby"))
						_owner.GrantCriteria(74987243309066);
					if (reward.ItemDefinition.Name.Contains("Topaz"))
						_owner.GrantCriteria(74987243309065);
				}
			}
			else
			{
				if (reward.Attributes[GameAttribute.Strength_Item] > 0)
					_owner.GrantCriteria(74987243308238);
				if (reward.Attributes[GameAttribute.Dexterity_Item] > 0)
					_owner.GrantCriteria(74987243308941);
				if (reward.Attributes[GameAttribute.Intelligence_Item] > 0)
					_owner.GrantCriteria(74987243308942);
				if (reward.Attributes[GameAttribute.Vitality_Item] > 0)
					_owner.GrantCriteria(74987243308943);

				if (reward.Attributes[GameAttribute.Block_Chance_Item_Total] > 0)
					_owner.GrantCriteria(74987243308239);
				if (reward.Attributes[GameAttribute.Thorns_Percent_Total] > 0 || reward.Attributes[GameAttribute.Thorns_Fixed, 0] > 0)
					_owner.GrantCriteria(74987243308945);
				if (reward.Attributes[GameAttribute.Sockets] > 0)
					_owner.GrantCriteria(74987243308946);
				if (reward.Attributes[GameAttribute.Experience_Bonus] > 0 || reward.Attributes[GameAttribute.Experience_Bonus_Percent] > 0)
					_owner.GrantCriteria(74987243308947);
				if (reward.Attributes[GameAttribute.Gold_Find_Total] > 0)
					_owner.GrantCriteria(74987243308948);
				if (reward.Attributes[GameAttribute.Attacks_Per_Second_Item_Bonus] > 0 || reward.Attributes[GameAttribute.Attacks_Per_Second_Item_Percent] > 0)
					_owner.GrantCriteria(74987243308949);
				if (reward.Attributes[GameAttribute.Hitpoints_Regen_Per_Second] > 0)
					_owner.GrantCriteria(74987243308950);
				if (reward.Attributes[GameAttribute.Magic_Find_Total] > 0)
					_owner.GrantCriteria(74987243308951);
				if (reward.Attributes[GameAttribute.Crit_Percent_Bonus_Capped] > 0 || reward.Attributes[GameAttribute.Crit_Damage_Percent] > 0)
					_owner.GrantCriteria(74987243308952);
				if (reward.Attributes[GameAttribute.Movement_Scalar] > 0)
					_owner.GrantCriteria(74987243312486);


				if (reward.Attributes[GameAttribute.Resistance, 3] > 0) //cold
					_owner.GrantCriteria(74987243308240);
				if (reward.Attributes[GameAttribute.Resource_Regen_Per_Second, 3] > 0) //spirit
					_owner.GrantCriteria(74987243308954);
				if (reward.Attributes[GameAttribute.Resource_Max_Bonus, 6] > 0) //discipline
					_owner.GrantCriteria(74987243308955);
				if (reward.Attributes[GameAttribute.Resistance, 5] > 0) //arcane
					_owner.GrantCriteria(74987243308956);
				if (reward.Attributes[GameAttribute.Resistance, 1] > 0) //fire
					_owner.GrantCriteria(74987243308957);
				if (reward.Attributes[GameAttribute.Resistance, 2] > 0) //lightning
					_owner.GrantCriteria(74987243308958);
				if (reward.Attributes[GameAttribute.Resistance, 4] > 0) //poison
					_owner.GrantCriteria(74987243308959);
				if (reward.Attributes[GameAttribute.Resource_Max_Bonus, 2] > 0) //fury
					_owner.GrantCriteria(74987243308960);
				if (reward.Attributes[GameAttribute.Resource_Max_Bonus, 1] > 0) //arcane
					_owner.GrantCriteria(74987243308961);
				if (reward.Attributes[GameAttribute.Resource_Max_Bonus, 0] > 0) //mana
					_owner.GrantCriteria(74987243308962);
				if (reward.Attributes[GameAttribute.Resistance_All] > 0)
					_owner.GrantCriteria(74987243308963);
				if (reward.Attributes[GameAttribute.Hitpoints_On_Hit] > 0 || reward.Attributes[GameAttribute.Steal_Health_Percent] > 0)
					_owner.GrantCriteria(74987243308966);
				if (reward.Attributes[GameAttribute.Health_Globe_Bonus_Health] > 0)
					_owner.GrantCriteria(74987243308967);
				if (reward.Attributes[GameAttribute.Armor_Bonus_Item] > 0 || reward.Attributes[GameAttribute.Armor_Item_Percent] > 0)
					_owner.GrantCriteria(74987243308968);
			}

			_owner.UpdateQuantity(74987243307366, 1);
			_owner.UpdateQuantity(74987243307371, 1);
			_owner.UpdateQuantity(74987243307374, 1);

			if (reward.Attributes[GameAttribute.ItemStackQuantityLo] > 0)
				_inventoryGrid.AddItem(reward);
			client.SendMessage(new CraftingResultsMessage { annItem = reward.GlobalID, GBIDItem = recipe.ItemSpecifierData.ItemGBId, IQL = reward.Attributes[GameAttribute.Item_Quality_Level] });
			

			UpdateCurrencies();
		}

		private void OnTryTransmogItemMessage(GameClient client, TryTransmogItemMessage msg)
		{
			Logger.Debug("OnTryTransmogItemMessage, itemID: {0}, GBIDTransmog: {1}", msg.annItem, msg.GBIDTransmog);
			var item = _inventoryGrid.GetItemByDynId(_owner, msg.annItem);
			if (item == null)
			{
				item = _equipment.GetItemByDynId(_owner, msg.annItem);
				if (item == null) return;
			}

			var transmogItem = ItemGenerator.GetItemDefinition(msg.GBIDTransmog);
			if (transmogItem == null) return;

			int amount = transmogItem.TransmogCost;
			if (GetGoldAmount() >= amount)
			{
				RemoveGoldAmount(amount);
				item.UpdateTransmog(msg.GBIDTransmog);
				item.Attributes.BroadcastChangedIfRevealed();
				SendVisualInventory(_owner);
			}
			_owner.GrantCriteria(74987253143400);
		}
		//*/
		public void OnBuySharedStashSlots(RequestBuySharedStashSlotsMessage requestBuySharedStashSlotsMessage)
		{
			int amount = 0;

			if (_stashGrid.Rows % 10 == 0)
			{
				if (_stashGrid.Rows / 10 - 1 >= _stashBuyValue.Length)
					return;
				amount = _stashBuyValue[_stashGrid.Rows / 10 - 1];
			}

			if (GetGoldAmount() >= amount)
			{
				RemoveGoldAmount(amount);
				_owner.Attributes[GameAttribute.Shared_Stash_Slots] += 70;
				_owner.Attributes.BroadcastChangedIfRevealed();
				_stashGrid.ResizeGrid(_owner.Attributes[GameAttribute.Shared_Stash_Slots] / 7, 7);
				var dbGAcc = _owner.Toon.GameAccount.DBGameAccount;
				dbGAcc.StashSize = _owner.Attributes[GameAttribute.Shared_Stash_Slots];
				_owner.World.Game.GameDBSession.SessionUpdate(dbGAcc);
			}

			if (_owner.Attributes[GameAttribute.Shared_Stash_Slots] >= 280)
				_owner.GrantAchievement(74987243307163);
		}

		private void OnSocketMessage(InventoryRequestSocketMessage requestSocketMessage)
		{
			Logger.Debug("requestSocketMessage, Field0: {0}, Field1: {1}", requestSocketMessage.annGem, requestSocketMessage.annItemToReceiveGem);
			var gem = _inventoryGrid.GetItemByDynId(_owner, requestSocketMessage.annGem);
			var item = _inventoryGrid.GetItemByDynId(_owner, requestSocketMessage.annItemToReceiveGem);
			if (item == null)
			{
				item = _equipment.GetItemByDynId(_owner, requestSocketMessage.annItemToReceiveGem);
				if (item == null) return;
			}
			if (item.Attributes[GameAttribute.Sockets_Filled] >= item.Attributes[GameAttribute.Sockets]) return;
			gem.Owner = item;
			gem.SetInventoryLocation(20, 0, item.Attributes[GameAttribute.Sockets_Filled]);
			(item.Owner as Player).InGameClient.SendMessage(gem.ACDInventoryPositionMessage(item.Owner as Player));
			item.Gems.Add(gem);

			switch (item.Gems.Count)
			{
				case 1:
					item.DBInventory.FirstGem = gem.GBHandle.GBID;
					_owner.World.Game.GameDBSession.SessionUpdate(item.DBInventory);
					break;
				case 2:
					item.DBInventory.SecondGem = gem.GBHandle.GBID;
					_owner.World.Game.GameDBSession.SessionUpdate(item.DBInventory);
					break;
				case 3:
					item.DBInventory.ThirdGem = gem.GBHandle.GBID;
					_owner.World.Game.GameDBSession.SessionUpdate(item.DBInventory);
					break;
			}

			if (gem.Attributes[GameAttribute.ItemStackQuantityLo] > 1)
			{
				var compensation = ItemGenerator.CookFromDefinition(_owner.World, ItemGenerator.GetItemDefinition(gem.GBHandle.GBID), 1);
				compensation.Attributes[GameAttribute.ItemStackQuantityLo] = gem.Attributes[GameAttribute.ItemStackQuantityLo] - 1;
				gem.Attributes[GameAttribute.ItemStackQuantityLo] = 1;
				_inventoryGrid.RemoveItem(gem);
				_inventoryGrid.AddItem(compensation);
			}
			else
			{
				_inventoryGrid.RemoveItem(gem);
			}
			//this.GrabSomeItems(gem.GBHandle.GBID, 1);

			_owner.SetAttributesByItems();
			_owner.SetAttributesByItemProcs();
			_owner.SetAttributesByGems();

			item.Attributes[GameAttribute.Sockets_Filled] = item.Gems.Count;
			item.Attributes.BroadcastChangedIfRevealed();

			_owner.GrantAchievement(74987243307166);

			if (gem.ItemDefinition.Name.Contains("Topaz"))
				_owner.GrantCriteria(74987243307541);
			if (gem.ItemDefinition.Name.Contains("Emerald"))
				_owner.GrantCriteria(74987243307549);
			if (gem.ItemDefinition.Name.Contains("Ruby"))
				_owner.GrantCriteria(74987243307550);
			if (gem.ItemDefinition.Name.Contains("Amethyst"))
				_owner.GrantCriteria(74987243307552);
			_owner.UpdatePercentageHP(_owner.PercHPbeforeChange);

		}
		//*
		private void OnGemsExtractMessage(InventoryGemsExtractMessage msg)
		{
			Logger.Debug("OnGemsExtractMessage");

			var item = _inventoryGrid.GetItemByDynId(_owner, msg.ItemID);
			if (item == null)
			{
				item = _equipment.GetItemByDynId(_owner, msg.ItemID);
				if (item == null) return;
			}

			if (item.DBInventory.FirstGem != -1)
			{
				PickUp(ItemGenerator.CookFromDefinition(_owner.World, ItemGenerator.GetItemDefinition(item.DBInventory.FirstGem)));
			}

			if (item.DBInventory.SecondGem != -1)
				PickUp(ItemGenerator.CookFromDefinition(_owner.World, ItemGenerator.GetItemDefinition(item.DBInventory.SecondGem)));

			if (item.DBInventory.ThirdGem != -1)
				PickUp(ItemGenerator.CookFromDefinition(_owner.World, ItemGenerator.GetItemDefinition(item.DBInventory.ThirdGem)));

			item.DBInventory.FirstGem = -1;
			item.DBInventory.SecondGem = -1;
			item.DBInventory.ThirdGem = -1;

			_owner.World.Game.GameDBSession.SessionUpdate(item.DBInventory);

			foreach (var gem in item.Gems)
				gem.Unreveal(_owner);

			item.Gems.Clear();
			item.Attributes[GameAttribute.Sockets_Filled] = 0;
			item.Attributes.BroadcastChangedIfRevealed();

			_owner.GrantAchievement(74987243307789);
		}
		//*/
		public void RepairAll()
		{
			RepairEquipment();
			int cost = 0;
			foreach (var item in GetBackPackItems())
				if (item.Attributes[GameAttribute.Durability_Cur] < item.Attributes[GameAttribute.Durability_Max])
				{
					cost += (int)((item.GetPrice() * (item.Attributes[GameAttribute.Durability_Max] - item.Attributes[GameAttribute.Durability_Cur])) / (item.Attributes[GameAttribute.Durability_Max] * 25));
					item.UpdateDurability(item.Attributes[GameAttribute.Durability_Max]);
				}
			RemoveGoldAmount(cost);
		}

		public void RepairEquipment()
		{
			int cost = 0;
			foreach (var item in GetEquippedItems())
				if (item.Attributes[GameAttribute.Durability_Cur] < item.Attributes[GameAttribute.Durability_Max])
				{
					cost += (int)((item.GetPrice() * (item.Attributes[GameAttribute.Durability_Max] - item.Attributes[GameAttribute.Durability_Cur])) / (item.Attributes[GameAttribute.Durability_Max] * 25));
					item.UpdateDurability(item.Attributes[GameAttribute.Durability_Max]);
				}
			RemoveGoldAmount(cost);
			_owner.GrantAchievement(74987243307773);
		}

		public void PickUpGold(Item item)
		{
			int amount = item.Attributes[GameAttribute.Gold];//item.Attributes[GameAttribute.ItemStackQuantityLo];
			AddGoldAmount(amount, false);
		}

		public void PickUpBloodShard(Item item)
		{
			int amount = item.Attributes[GameAttribute.ItemStackQuantityLo];
			AddBloodShardsAmount(amount, false);
		}

		public void PickUpPlatinum(Item item)
		{
			int amount = item.Attributes[GameAttribute.ItemStackQuantityLo];
			AddPlatinumAmount(amount);
		}

		private void OnInventoryRequestUseMessage(InventoryRequestUseMessage inventoryRequestUseMessage)
		{
			uint targetItemId = inventoryRequestUseMessage.UsedOnItem;
			uint usedItemId = inventoryRequestUseMessage.UsedItem;
			int actionId = inventoryRequestUseMessage.Type;
			Item usedItem = GetItemByDynId(_owner, usedItemId);
			Item targetItem = GetItemByDynId(_owner, targetItemId);

			if (usedItem != null)
				usedItem.OnRequestUse(_owner, targetItem, actionId, inventoryRequestUseMessage.Location);
		}

		public void DecreaseItemStack(Item item, int count = 1)
		{
			GrabSomeItems(item.GBHandle.GBID, count);
		}

		public void DestroyInventoryItem(Item item)
		{
			if (_equipment.IsItemEquipped(item))
			{
				_equipment.UnequipItem(item);
				SendVisualInventory(_owner);
			}
			else
			{
				_inventoryGrid.RemoveItem(item);
				_stashGrid.RemoveItem(item);
			}
			item.Unreveal(_owner);
			item.Destroy();
			//_destroyedItems.Add(item);
		}

		//private List<Item> _destroyedItems = new List<Item>();

		public bool Reveal(Player player)
		{
			_equipment.Reveal(player);
			if (player == _owner)
			{
				//_inventoryGrid.Reveal(player);
				//_stashGrid.Reveal(player);
			}
			return true;
		}

		public bool Unreveal(Player player)
		{
			_equipment.Unreveal(player);
			if (player == _owner)
			{
				//_inventoryGrid.Unreveal(player);
				//_stashGrid.Unreveal(player);
			}

			return true;
		}

		public Item GetItem(uint itemId)
		{
			Item result;
			if (!_inventoryGrid.Items.TryGetValue(itemId, out result) &&
				!_stashGrid.Items.TryGetValue(itemId, out result) &&
				!_equipment.Items.TryGetValue(itemId, out result))
			{
				return null;
			}
			return result;
		}

		public Item GetItemByDynId(Player plr, uint dynId)
		{
			if (_inventoryGrid.Items.Values.Union(_stashGrid.Items.Values).Union(_equipment.Items.Values).Where(it => it.IsRevealedToPlayer(plr) && it.DynamicID(plr) == dynId).Count() > 0)
				return _inventoryGrid.Items.Values.Union(_stashGrid.Items.Values).Union(_equipment.Items.Values).Single(it => it.IsRevealedToPlayer(plr) && it.DynamicID(plr) == dynId);
			else
				return null;
		}

		public bool HasItem(int GBid)
		{
			return _inventoryGrid.HaveEnough(GBid, 1);
		}

		public bool HasGold(int amount)
		{
			return _inventoryGold.Attributes[GameAttribute.Gold] >= amount;
		}

		public Item GetEquippedWeapon()
		{
			return _equipment.GetWeapon();
		}

		public Item GetEquippedOffHand()
		{
			return _equipment.GetOffHand();
		}

		

		public void AddGoldAmount(int amount, bool immediately = true)
		{
			_inventoryGold.Attributes[GameAttribute.Gold] += amount;
			_inventoryGold.Attributes[GameAttribute.ItemStackQuantityLo] = _inventoryGold.Attributes[GameAttribute.Gold];
			_inventoryGold.Attributes.SendChangedMessage(_owner.InGameClient);
			if (immediately)
			{
				_owner.Toon.GameAccount.Gold += (ulong)amount;
			}
			else
				_owner.GoldCollectedTempCount += amount;

			UpdateCurrencies();
		}

		public void RemoveGoldAmount(int amount)
		{
			_inventoryGold.Attributes[GameAttribute.Gold] -= amount;
			_inventoryGold.Attributes[GameAttribute.ItemStackQuantityLo] = _inventoryGold.Attributes[GameAttribute.Gold];
			_inventoryGold.Attributes.SendChangedMessage(_owner.InGameClient);
			_owner.Toon.GameAccount.Gold -= (ulong)amount;
			UpdateCurrencies();
		}

		public int GetGoldAmount()
		{

			if (_inventoryGold != null)
			{
				// Logger.Warn($"InventoryGold is $[bold red]$NOT$[/]$ null: {_inventoryGold.Attributes[GameAttribute.Gold]}");
				return _inventoryGold.Attributes[GameAttribute.Gold];
			}
			else
			{
				// Logger.Warn($"InventoryGold is $[bold red]$NULL$[/]$");

				return -1;
			}
		}

		public void AddBloodShardsAmount(int amount, bool immediately = true)
		{
			int C1 = 0; foreach (var item in FindSameItems(-363607620)) C1 += item.Attributes[GameAttribute.ItemStackQuantityLo];
			int C2 = 0; foreach (var item in FindSameItems(-1585802162)) C2 += item.Attributes[GameAttribute.ItemStackQuantityLo];
			int C3 = 0; foreach (var item in FindSameItems(-605947593)) C3 += item.Attributes[GameAttribute.ItemStackQuantityLo];

			var Moneys = D3.Items.CurrencySavedData.CreateBuilder();
			D3.Items.CurrencyData GoldData = D3.Items.CurrencyData.CreateBuilder().SetId(0).SetCount((long)GetGoldAmount()).Build();
			//D3.Items.CurrencyData.CreateBuilder().SetId(1).SetCount(_owner.InGameClient.BnetClient.Account.GameAccount.BloodShards).Build();
			var BloodShardsElement = D3.Items.CurrencyData.CreateBuilder().SetId(1);
			if (immediately)
			{
				BloodShardsElement.SetCount(_owner.Toon.GameAccount.BloodShards);
			}
			else
			{
				_owner.BloodShardsCollectedTempCount += amount;
				if (_owner.World.Game.IsHardcore)
				BloodShardsElement.SetCount(_owner.BloodShardsCollectedTempCount + _owner.Toon.GameAccount.BloodShards);
			}
		}

		public void UpdateCurrencies()
		{
			var Moneys = D3.Items.CurrencySavedData.CreateBuilder();
			var playerAcc = _owner.InGameClient.BnetClient.Account.GameAccount;
			D3.Items.CurrencyData GoldData = D3.Items.CurrencyData.CreateBuilder().SetId(0).SetCount((long)this.GetGoldAmount()).Build();
			D3.Items.CurrencyData BloodShardData = D3.Items.CurrencyData.CreateBuilder().SetId(1).SetCount(playerAcc.BloodShards).Build();
			D3.Items.CurrencyData PlatinumData = D3.Items.CurrencyData.CreateBuilder().SetId(2).SetCount(playerAcc.Platinum).Build();

			D3.Items.CurrencyData Craft1Data = D3.Items.CurrencyData.CreateBuilder().SetId(3).SetCount(playerAcc.CraftItem1).Build(); // Reusable Parts.
			D3.Items.CurrencyData Craft2Data = D3.Items.CurrencyData.CreateBuilder().SetId(4).SetCount(playerAcc.CraftItem2).Build(); // Arcanes Dust.
			D3.Items.CurrencyData Craft3Data = D3.Items.CurrencyData.CreateBuilder().SetId(5).SetCount(playerAcc.CraftItem3).Build(); // Veiled Crystal.
			D3.Items.CurrencyData Craft4Data = D3.Items.CurrencyData.CreateBuilder().SetId(6).SetCount(playerAcc.CraftItem4).Build(); // Death's Breath.
			D3.Items.CurrencyData Craft5Data = D3.Items.CurrencyData.CreateBuilder().SetId(7).SetCount(playerAcc.CraftItem5).Build(); // Forgotten Soul.

			D3.Items.CurrencyData Horadric1Data = D3.Items.CurrencyData.CreateBuilder().SetId(8).SetCount(playerAcc.HoradricA1Res).Build();  // Khanduran Rune Bounty itens Act I.
			D3.Items.CurrencyData Horadric2Data = D3.Items.CurrencyData.CreateBuilder().SetId(9).SetCount(playerAcc.HoradricA2Res).Build();  // Caldeum Nightshade Bounty itens Act II.
			D3.Items.CurrencyData Horadric3Data = D3.Items.CurrencyData.CreateBuilder().SetId(10).SetCount(playerAcc.HoradricA3Res).Build(); // Arreat War Tapestry Bounty itens Act III.
			D3.Items.CurrencyData Horadric4Data = D3.Items.CurrencyData.CreateBuilder().SetId(11).SetCount(playerAcc.HoradricA4Res).Build(); // Copputed Angel Flesh Bounty itens Act IV.
			D3.Items.CurrencyData Horadric5Data = D3.Items.CurrencyData.CreateBuilder().SetId(12).SetCount(playerAcc.HoradricA5Res).Build(); // Westmarch Holy Water Bounty itens Act V.

			D3.Items.CurrencyData Craft8Data = D3.Items.CurrencyData.CreateBuilder().SetId(13).SetCount(playerAcc.HeartofFright).Build();     // Heart of Fright.
			D3.Items.CurrencyData Craft9Data = D3.Items.CurrencyData.CreateBuilder().SetId(14).SetCount(playerAcc.VialofPutridness).Build();  // Idol of Terror.
			D3.Items.CurrencyData Craft10Data = D3.Items.CurrencyData.CreateBuilder().SetId(15).SetCount(playerAcc.IdolofTerror).Build();     // Vail of Putridiness.
			D3.Items.CurrencyData Craft11Data = D3.Items.CurrencyData.CreateBuilder().SetId(16).SetCount(playerAcc.LeorikKey).Build();        // Leorik Regret.

			D3.Items.CurrencyData Craft7Data = D3.Items.CurrencyData.CreateBuilder().SetId(20).SetCount(playerAcc.BigPortalKey).Build();      // KeyStone Greater Rift.

			D3.Items.CurrencyData[] consumables = {GoldData, BloodShardData, PlatinumData, Craft1Data, Craft2Data, Craft3Data, Craft4Data, Craft5Data, Craft7Data, Horadric1Data, Horadric2Data, Horadric3Data, Horadric4Data, Horadric5Data, Craft8Data, Craft9Data, Craft10Data, Craft11Data};

			foreach (var consumable in consumables)
			{
				Moneys.AddCurrency(consumable);
			}

			_owner.InGameClient.SendMessage(new GenericBlobMessage(Opcodes.CurrencyDataFull) { Data = Moneys.Build().ToByteArray() });
		}

		public void AddPlatinumAmount(int amount)
		{
			_owner.Toon.GameAccount.Platinum += amount;
			UpdateCurrencies();
		}

		public void RemoveBloodShardsAmount(int amount)
		{
			this.BloodShards -= amount;
			_owner.Toon.GameAccount.BloodShards -= amount;
			UpdateCurrencies();
		}

		public int GetBloodShardsAmount()
		{
			return BloodShards;
		}

		public void LoadFromDB()
		{
			//load everything and make a switch on slot_id

			Item item = null;
			int goldAmount = (int)_owner.World.Game.GameDBSession.SessionGet<DBGameAccount>(_owner.Toon.GameAccount.PersistentID).Gold;
			// Logger.Warn($"User {this._owner.Toon.PersistentID} has {goldAmount} gold.");
			this.BloodShards = (int)_owner.World.Game.GameDBSession.SessionGet<DBGameAccount>(_owner.Toon.GameAccount.PersistentID).BloodShards;
			// Clear already present items
			// LoadFromDB is called every time World is changed, even entering a dungeon
			_inventoryGrid.Clear();
			// first of all load stash size
			var slots = _owner.World.Game.GameDBSession.SessionGet<DBGameAccount>(_owner.Toon.GameAccount.PersistentID).StashSize;
			if (slots > 0)
			{
				_owner.Attributes[GameAttribute.Shared_Stash_Slots] = slots;
				_owner.Attributes.BroadcastChangedIfRevealed();
				// To be applied before loading items, to have all the space needed
				_stashGrid.ResizeGrid(_owner.Attributes[GameAttribute.Shared_Stash_Slots] / 7, 7);
			}


			// next read all items
			var allInventoryItems = _owner.World.Game.GameDBSession.SessionQueryWhere<DBInventory>(
					dbi =>
					dbi.DBToon != null &&
					dbi.HirelingId == 0 &&
					dbi.DBToon.Id == _owner.Toon.PersistentID).ToList();

			//Task.Run(() =>
			//{
			foreach (var inv in allInventoryItems)
			{
				//Thread.Sleep(31);
				var slot = inv.EquipmentSlot;
				if (slot >= (int)EquipmentSlotId.Inventory && slot <= (int)EquipmentSlotId.Neck)
				{
					item = ItemGenerator.LoadFromDB(_owner, inv);
					//item.DBInventory = inv;
					if (slot == (int)EquipmentSlotId.Inventory)
					{
						_inventoryGrid.PlaceItem(item, inv.LocationY, inv.LocationX);
					}
					else
					{
						_equipment.EquipItem(item, (int)slot, false);
					}
				}
			}
			SendVisualInventory(_owner);
			_owner.SetAttributesByItems();
			_owner.SetAttributesByItemProcs();
			_owner.SetAttributesByGems();
			_owner.SetAttributesByItemSets();
			_owner.SetAttributesByPassives();
			_owner.SetAttributesByParagon();
			CheckWeapons();
			_owner.Attributes.BroadcastChangedIfRevealed();
			Task.Delay(3000).ContinueWith((t) => {
				try
				{
					_owner.CheckBonusSets();
				}
				catch { }
			});

			//}).ContinueWith((a) =>{
			_inventoryGold = ItemGenerator.CreateGold(_owner, goldAmount);
			_inventoryGold.Attributes[GameAttribute.Gold] = goldAmount;
			// Logger.Warn($"User {this._owner.Toon.PersistentID} - inventory gold has {_inventoryGold.Attributes[GameAttribute.Gold]} gold.");

			_inventoryGold.Attributes[GameAttribute.ItemStackQuantityLo] = goldAmount; // This is the attribute that makes the gold visible in gamethe gold visible in game
			_inventoryGold.Owner = _owner;
			_inventoryGold.SetInventoryLocation((int)EquipmentSlotId.Gold, 0, 0);
			_inventoryGold.Attributes.SendChangedMessage(_owner.InGameClient);
			
			//this.inventoryPotion = ItemGenerator.CreateItem(this._owner, ItemGenerator.GetItemDefinition(DiIiS_NA.Core.Helpers.Hash.StringHashHelper.HashItemName("HealthPotionBottomless")));
			//this.inventoryPotion.Owner = _owner;
			//this.inventoryPotion.SetInventoryLocation((int)EquipmentSlotId.Inventory, 0, 0);
			//*/
			Loaded = true;
			UpdateCurrencies();
			//});
			_owner.UpdatePercentageHP(_owner.PercHPbeforeChange);

		}

		public void LoadStashFromDB()
		{
			Item item = null;
			_stashGrid.Clear();

			// next load all stash items
			var stashInventoryItems =
				_owner.World.Game.GameDBSession.SessionQueryWhere<DBInventory>(
					dbi =>
					dbi.DBGameAccount.Id == _owner.Toon.GameAccount.PersistentID &&
					dbi.HirelingId == 0 &&
					dbi.DBToon == null &&
					dbi.isHardcore == _owner.World.Game.IsHardcore).ToList();

			//Task.Run(() =>
			//{
			foreach (var inv in stashInventoryItems)
			{
				//Thread.Sleep(15);
				var slot = inv.EquipmentSlot;
				if (slot == (int)EquipmentSlotId.Stash)
				{
					// load stash
					item = ItemGenerator.LoadFromDB(_owner, inv);
					item.DBInventory = inv;
					_stashGrid.PlaceItem(item, inv.LocationY, inv.LocationX);
				}
			}
			//}).Wait();

			StashLoaded = true;
		}

		public void UnrevealStash()
		{
			_stashGrid.Unreveal(_owner);
		}

		public void RefreshInventoryToClient()
		{
			var itemsToUpdate = new List<Item>();
			itemsToUpdate.AddRange(_inventoryGrid.Items.Values);
			if (StashRevealed)
				itemsToUpdate.AddRange(_stashGrid.Items.Values);
			itemsToUpdate.AddRange(_buybackGrid.Items.Values);
			itemsToUpdate.Add(_inventoryGold);
			//Task.Run(() =>
			//{
			foreach (var itm in itemsToUpdate)
			{
				if (itm.Owner is Player)
				{
					var player = (itm.Owner as Player);
					//Thread.Sleep(30);
					if (!itm.Reveal(player))
					{
						player.InGameClient.SendMessage(itm.ACDInventoryPositionMessage(player));
					}
				}
			}
			SendVisualInventory(_owner);
			_owner.SetAttributesByItems();
			_owner.SetAttributesByItemProcs();
			_owner.SetAttributesByGems();
			_owner.SetAttributesByItemSets();
			_owner.SetAttributesByPassives();
			_owner.SetAttributesByParagon();
			_owner.Attributes.BroadcastChangedIfRevealed();
			//});
			UpdateCurrencies();
			_owner.UpdatePercentageHP(_owner.PercHPbeforeChange);

		}

		public void SaveItemToDB(DBGameAccount dbGameAccount, DBToon dbToon, EquipmentSlotId slotId, Item item) //public only for Grids
		{
			//Logger.Debug("SaveItemToDB");
			if (item == null)
				return;
			//bool newItem = false;
			item.DBInventory = new DBInventory();

			item.DBInventory.DBGameAccount = dbGameAccount;
			item.DBInventory.DBToon = dbToon;
			item.DBInventory.LocationX = item.InventoryLocation.X;
			item.DBInventory.LocationY = item.InventoryLocation.Y;
			item.DBInventory.EquipmentSlot = (int)slotId;
			item.DBInventory.isHardcore = item.Owner.World.Game.IsHardcore;
			item.DBInventory.FirstGem = -1;
			item.DBInventory.SecondGem = -1;
			item.DBInventory.ThirdGem = -1;
			item.DBInventory.Unidentified = item.Unidentified;
			item.DBInventory.TransmogGBID = -1;

			if (item.Gems.Count > 0)
			{
				item.DBInventory.FirstGem = item.Gems[0].GBHandle.GBID;

				if (item.Gems.Count > 1)
				{
					item.DBInventory.SecondGem = item.Gems[1].GBHandle.GBID;

					if (item.Gems.Count > 2)
					{
						item.DBInventory.ThirdGem = item.Gems[2].GBHandle.GBID;
					}
				}
			}

			ItemGenerator.SaveToDB(item);

			//Logger.Debug("SaveItemToDB, SessionSave");
			_owner.World.Game.GameDBSession.SessionSave(item.DBInventory);
			//Logger.Debug("SaveItemToDB success, item dbid: {0}", item.DBInventory.Id);
		}

		public void RemoveItemFromDB(Item item) //public only for Grids
		{
			//Logger.Debug("RemoveItemFromDB");
			if (item == null)
				return;
			if (item.DBInventory == null)
				return;
			//var inventory = item.Owner.World.Game.GameDBSession.SessionGet<DBInventory>(item.DBInventory.Id);

			_owner.World.Game.GameDBSession.SessionDelete(item.DBInventory);

			//Logger.Debug("RemoveItemFromDB success, item dbid: {0}", item.DBInventory.Id);
			item.DBInventory = null;
		}

		public void ChangeItemSlotDB(int slotId, Item item)
		{
			//Logger.Debug("ChangeItemSlotDB");
			if (item == null)
				return;
			if (item.DBInventory == null)
				return;

			item.DBInventory.EquipmentSlot = slotId;
			item.DBInventory.LocationX = 0;
			item.DBInventory.LocationY = 0;

			if (slotId == 15)
				item.DBInventory.DBToon = null;
			else
				item.DBInventory.DBToon = (_owner as Player).Toon.DBToon;

			item.Owner.World.Game.GameDBSession.SessionUpdate(item.DBInventory);
			//Logger.Debug("ChangeItemSlotDB success, item dbid: {0}", item.DBInventory.Id);
		}

		public void ChangeItemLocationDB(int locX, int locY, Item item)
		{
			//Logger.Trace("ChangeItemLocationDB");
			if (item == null)
				return;
			if (item.DBInventory == null)
				return;

			item.DBInventory.LocationX = locX;
			item.DBInventory.LocationY = locY;

			item.Owner.World.Game.GameDBSession.SessionUpdate(item.DBInventory);
			//Logger.Debug("ChangeItemLocationDB success, item dbid: {0}", item.DBInventory.Id);
		}

		#region EqupimentStats
		public float GetItemBonus(GameAttributeF attributeF)
		{
			if (!Loaded) return _owner.Attributes[attributeF];

			var stats = GetEquippedItems().Where(item => item.Attributes[GameAttribute.Durability_Cur] > 0 || item.Attributes[GameAttribute.Durability_Max] == 0);

			return stats.Sum(item => item.Attributes[attributeF]);
		}

		public int GetItemBonus(GameAttributeI attributeI)
		{
			return Loaded ? GetEquippedItems().Where(item => item.Attributes[GameAttribute.Durability_Cur] > 0 || item.Attributes[GameAttribute.Durability_Max] == 0).Sum(item => item.Attributes[attributeI]) : _owner.Attributes[attributeI];
		}

		public bool GetItemBonus(GameAttributeB attributeB)
		{
			return Loaded ? (GetEquippedItems().Where(item => item.Attributes[attributeB] == true).Count() > 0) : _owner.Attributes[attributeB];
		}

		public float GetItemBonus(GameAttributeF attributeF, int attributeKey)
		{
			return Loaded ? GetEquippedItems().Where(item => item.Attributes[GameAttribute.Durability_Cur] > 0 || item.Attributes[GameAttribute.Durability_Max] == 0).Sum(item => item.Attributes[attributeF, attributeKey]) : _owner.Attributes[attributeF];
		}

		public int GetItemBonus(GameAttributeI attributeI, int attributeKey)
		{
			return Loaded ? GetEquippedItems().Where(item => item.Attributes[GameAttribute.Durability_Cur] > 0 || item.Attributes[GameAttribute.Durability_Max] == 0).Sum(item => item.Attributes[attributeI, attributeKey]) : _owner.Attributes[attributeI];
		}

		public void SetGemBonuses()
		{
			uint countofGems = 0;
			foreach (var equip in GetEquippedItems())
			{
				foreach (var gem in equip.Gems)
				{
					var effect = ItemGenerator.GetGemEffectDefinition(gem.GBHandle.GBID, equip.EquipGemType);
					if (effect == null) continue;
					foreach (var attr in effect.Attribute)
					{
						float result;
						if (attr.AttributeId > 0)
							if (FormulaScript.Evaluate(attr.Formula.ToArray(), new ItemRandomHelper(35674658), out result))
							{
								//Logger.Info("attr: {0},	{1}	{2}", attr.AttributeId, attr.SNOParam, result);
								var attrib = GameAttribute.Attributes[attr.AttributeId] as GameAttributeF;
								//if (attrib == GameAttribute.Dexterity_Item || attrib == GameAttribute.Vitality_Item || attrib == GameAttribute.Strength_Item || attrib == GameAttribute.Intelligence_Item) result /= 1065353216; //dat shit is crazy
								if (attr.SNOParam != -1)
									_owner.Attributes[attrib, attr.SNOParam] += result;
								else
									_owner.Attributes[attrib] += result;
							}
					}
					_owner.Attributes.BroadcastChangedIfRevealed();
					//_owner.Attributes[effect.Attribute[0].]
					countofGems++;
					if (_owner.InGameClient.Game.IsSeasoned && countofGems >= 5)
						_owner.GrantCriteria(74987254401623);
				}
			}
		}

		public void SetItemSetBonuses()
		{
			foreach (var set in GetEquippedItems().Where(i => i.ItemDefinition.SNOSet != -1).GroupBy(i => i.ItemDefinition.SNOSet))
			{
				for (int c = 1; c <= set.Count(); c++)
				{
					var effect = ItemGenerator.GetItemSetEffectDefinition(set.First().ItemDefinition.SNOSet, c);
					if (effect == null) continue;
					foreach (var attr in effect.Attribute)
					{
						float result;
						if (attr.AttributeId > 0)
							if (FormulaScript.Evaluate(attr.Formula.ToArray(), new ItemRandomHelper(35674658), out result))
							{
								//Logger.Debug("attr: {0},	{1}	{2}", attr.AttributeId, attr.SNOParam, result);
								var attrib = GameAttribute.Attributes[attr.AttributeId] as GameAttributeF;
								if (attrib == GameAttribute.Dexterity_Item || attrib == GameAttribute.Vitality_Item || attrib == GameAttribute.Strength_Item || attrib == GameAttribute.Intelligence_Item) result /= 1065353216; //dat shit is crazy
								if (attr.SNOParam != -1)
									_owner.Attributes[attrib, attr.SNOParam] += result;
								else
									_owner.Attributes[attrib] += result;
							}
					}
				}
				_owner.Attributes[GameAttribute.Set_Item_Count, set.First().ItemDefinition.SNOSet] = set.Count();
				if (set.Count() >= 6)
					_owner.GrantAchievement(74987243307160);
			}
			_owner.Attributes.BroadcastChangedIfRevealed();
		}

		public void DecreaseDurability(float percent)
		{
			foreach (var equip in GetEquippedItems())
			{
				if (equip.Attributes[GameAttribute.Item_Indestructible] == false)
				{
					equip.UpdateDurability(Math.Max(0, (equip.Attributes[GameAttribute.Durability_Cur] - (int)((float)equip.Attributes[GameAttribute.Durability_Max] * percent))));
					equip.Attributes.SendChangedMessage(_owner.InGameClient);
				}
			}
		}

		#endregion
	}
}
