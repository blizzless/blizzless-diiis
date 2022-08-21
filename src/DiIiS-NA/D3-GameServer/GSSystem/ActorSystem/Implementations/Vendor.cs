//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.NPC;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Toons;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	[HandledSNO(
		//act 1
		// Miner_InTown + variations
		177320, 178396, 178401, 178403, 229372, 229373, 229374, 229375, 229376,
		// Fence_InTown + variations
		177319, 178388, 178390, 178392, 229367, 229368, 229369, 229370, 229371,
		// Collector_InTown + variations
		107535, 178362, 178383, 178385, 229362, 229363, 229364, 229365, 229366,
		//act 2
		//the Peddler
		180593, 180783, 180784, 180785, 230573, 230574, 230575, 230576, 230577,
		// Miner_InTown + variations
		180275, 180800, 180801, 180802, 230476, 230477, 230478, 230479, 230480,
		// Fence_InTown + variations
		180274, 180817, 180818, 180819, 230471, 230472, 230473, 230474, 230475,
		// Collector_InTown + variations
		180272, 180807, 180808, 180809, 230466, 230467, 230468, 230469, 230470,
		//act 3
		// Collector_InTown + variations
		230481, 230482, 230483, 230484, 230485, 181466, 181583, 181584,
		// Fence_InTown + variations
		230486, 230487, 230488, 230489, 230490, 181468, 181585, 181586,
		// Miner_InTown + variations
		230491, 230492, 230493, 230494, 230495, 181467, 181588, 181590,
		//act 4
		// Collector_InTown + variations
		182388, 230496, 230497, 230498, 230499, 230500, 230501, 230502,
		// Fence_InTown + variations
		182389, 230503, 230504, 230505, 230506, 230507, 230508, 230509,
		// Miner_InTown + variations
		182390, 230510, 230511, 230512, 230513, 230514, 230515, 230516,
		//act 5
		309796, 309831, 309836, 337417
		)]
	public class Vendor : InteractiveNPC
	{
		protected InventoryGrid _vendorGrid;

		protected int level = 1;
		private bool _collapsed = false;
		public Vendor(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.Attributes[GameAttribute.TeamID] = 0;
			this.Attributes[GameAttribute.MinimapActive] = true;
			this.level = this.World.Game.InitialMonsterLevel;
			_vendorGrid = new InventoryGrid(this, 1, 20, (int)EquipmentSlotId.Vendor);
			PopulateItems();
		}

		private static List<int> _peddlers = new List<int> { 180593, 180783, 180784, 180785, 230573, 230574, 230575, 230576, 230577 };

		protected virtual List<Item> GetVendorItems()
		{
			var list = new List<Item>
			{
				ItemGenerator.GenerateRandomEquip(this, this.level, 3, 7),
				ItemGenerator.GenerateRandomEquip(this, this.level, 3, 7),
				ItemGenerator.GenerateRandomEquip(this, this.level, 3, 7),
				ItemGenerator.GenerateRandomEquip(this, this.level, 3, 6),
				ItemGenerator.GenerateRandomEquip(this, this.level, 3, 6),
				ItemGenerator.GenerateRandomEquip(this, this.level, 3, 6),
				ItemGenerator.GenerateRandomEquip(this, this.level, 3, 5),
				ItemGenerator.GenerateRandomEquip(this, this.level, 3, 5),
				ItemGenerator.GenerateRandomEquip(this, this.level, 3, 5),
				ItemGenerator.GenerateRandomEquip(this, this.level, 3, 5),
				ItemGenerator.GenerateRandomEquip(this, this.level, 3, 5),
				ItemGenerator.GenerateRandomEquip(this, this.level, 3, 5),

				ItemGenerator.GenerateRandomDye(this),
				ItemGenerator.GenerateRandomDye(this),
				ItemGenerator.GenerateRandomDye(this),
				ItemGenerator.GenerateRandomDye(this),
				ItemGenerator.GenerateRandomDye(this),
				ItemGenerator.GenerateRandomDye(this)
			};

			if (_peddlers.Contains(this.ActorSNO.Id))
			{
				list.Clear();
				list.Add(ItemGenerator.CreateItem(this, ItemGenerator.GetItemDefinition(-799868536)));
			}

			return list;
		}

		private void PopulateItems()
		{
			var items = GetVendorItems();
			if (items.Count > _vendorGrid.Columns)
			{
				_vendorGrid.ResizeGrid(1, items.Count);
			}

			foreach (var item in items)
			{
				if (item != null)
					_vendorGrid.AddVendorItem(-1, -1, item);
			}
		}

		public override bool Reveal(Player player)
		{
			if (!base.Reveal(player))
				return false;

			return true;
		}

		public override bool Unreveal(Player player)
		{
			if (!base.Unreveal(player))
				return false;

			_vendorGrid.Unreveal(player);
			return true;
		}

		public override void OnTargeted(Player player, TargetMessage message)
		{
			base.OnTargeted(player, message);
			//player.RefreshReveal();
			_vendorGrid.Reveal(player);
			player.InGameClient.SendMessage(new OpenTradeWindowMessage((int)this.DynamicID(player)));
		}

		public virtual void OnRequestBuyItem(PlayerSystem.Player player, uint itemId)
		{
			bool buyback = false;
			int currentGold = player.Inventory.GetGoldAmount();
			Item item = _vendorGrid.GetItemByDynId(player, itemId);
			if (item == null)
			{
				item = player.Inventory.GetBuybackGrid().GetItemByDynId(player, itemId);
				if (item == null)
					return;
				buyback = true;
			}

			if (!player.Inventory.HasInventorySpace(item))
			{
				return;
			}

			int cost = (int)((item.GetPrice() * (1f + item.Attributes[GameAttribute.Item_Cost_Percent_Bonus])) * Math.Max(1, item.Attributes[GameAttribute.ItemStackQuantityLo]));
			//Check gold here
			if (currentGold < cost)
				return;

			//Remove the gold
			player.Inventory.RemoveGoldAmount(cost);
			item.Unreveal(player);
			player.Inventory.BuyItem(this, item, buyback);
			player.Inventory.RefreshInventoryToClient();
			RefreshVendorWindow(player);

			if (buyback) return;
			if (item.GBHandle.GBID == -799868536)
				player.GrantAchievement(74987243307159);

			switch (item.ItemType.Name)
			{
				case "Sword":
				case "Sword2H":
					player.GrantCriteria(74987243307251);
					break;
				case "Mace":
				case "Mace2H":
					player.GrantCriteria(74987243309931);
					break;
				case "Mojo":
					player.GrantCriteria(74987243309932);
					break;
				case "Amulet":
					player.GrantCriteria(74987243309933);
					break;
				case "Belt":
					player.GrantCriteria(74987243309934);
					break;
				case "Boots":
					player.GrantCriteria(74987243309935);
					break;
				case "Bow":
					player.GrantCriteria(74987243309936);
					break;
				case "Bracers":
					player.GrantCriteria(74987243309937);
					break;
				case "CeremonialDagger":
					player.GrantCriteria(74987243309938);
					break;
				case "ChestArmor":
					player.GrantCriteria(74987243309939);
					break;
				case "CombatStaff":
					player.GrantCriteria(74987243309940);
					break;
				case "Crossbow":
					player.GrantCriteria(74987243309941);
					break;
				case "Dagger":
					player.GrantCriteria(74987243309942);
					break;
				case "Dye":
					player.GrantCriteria(74987243309943);
					break;
				case "FistWeapon":
					player.GrantCriteria(74987243309944);
					break;
				case "Gloves":
					player.GrantCriteria(74987243309945);
					break;
				case "HandXbow":
					player.GrantCriteria(74987243309946);
					break;
				case "Helm":
					player.GrantCriteria(74987243309947);
					break;
				case "Legs":
					player.GrantCriteria(74987243309948);
					break;
				case "MightyWeapon1H":
				case "MightyWeapon2H":
					player.GrantCriteria(74987243309948);
					break;
				case "Orb":
					player.GrantCriteria(74987243309950);
					break;
				case "Polearm":
					player.GrantCriteria(74987243309951);
					break;
				case "Quiver":
					player.GrantCriteria(74987243309952);
					break;
				case "Ring":
					player.GrantCriteria(74987243309953);
					break;
				case "Shield":
					player.GrantCriteria(74987243309954);
					break;
				case "Shoulders":
					player.GrantCriteria(74987243309955);
					break;
				case "Spear":
					player.GrantCriteria(74987243309956);
					break;
				case "SpiritStone_Monk":
					player.GrantCriteria(74987243309957);
					break;
				case "Staff":
					player.GrantCriteria(74987243309958);
					break;
				case "Wand":
					player.GrantCriteria(74987243309959);
					break;
			}
		}

		public virtual void OnRequestSellItem(PlayerSystem.Player player, int itemId)
		{
			player.Inventory.SellItem(this, itemId);
		}

		private void RefreshVendorWindow(Player player)
		{
			foreach (var item in _vendorGrid.Items.Values)
			{
				item.Unreveal(player);
				item.Reveal(player);
			}
			player.InGameClient.SendMessage(new OpenTradeWindowMessage((int)this.DynamicID(player)));
		}

		public void AddBuybackItem(Item item, Player player)
		{
			var buybackGrid = player.Inventory.GetBuybackGrid();
			if (player.Inventory.GetBuybackItems().Count >= 20)
				return;

			if (player.Inventory.GetBuybackItems().Contains(item)) return;

			if (buybackGrid.Items.Count == buybackGrid.Columns)
				buybackGrid.ResizeGrid(1, buybackGrid.Items.Count + 1);

			if (item != null)
			{
				buybackGrid.AddVendorItem(-1, -1, item);
				item.Reveal(player);
				player.Inventory.RefreshInventoryToClient();
				RefreshVendorWindow(player);
			}
		}

		public void DeleteItem(Item item)
		{
			_vendorGrid.DeleteItem(item);
		}

		public override void OnPlayerApproaching(PlayerSystem.Player player)
		{
			try
			{
				if (Visible)
					if (player.Position.DistanceSquared(ref _position) < ActorData.Sphere.Radius * ActorData.Sphere.Radius * this.Scale * this.Scale && !_collapsed)
					{
						_collapsed = true;

						int convId = -1;

						switch (this.ActorSNO.Id)
						{
							//A1
							//Miner
							case 177320: case 178396: case 178401: case 178403: case 229372: case 229373: case 229374: case 229375: case 229376:
								convId = 210879; break;
							
							//A2
							//Miner
							case 180275: case 180800: case 180801: case 180802: case 230476: case 230477: case 230478: case 230479: case 230480:
								convId = 211399; break;

							//A3
							//Miner
							case 230491: case 230492: case 230493: case 230494: case 230495:
								convId = 211432; break;
							case 181467: case 181588: case 181590:
								convId = 211267; break;

							//A4
							//Miner
							case 182390: case 230510: case 230511: case 230512: case 230513: case 230514: case 230515: case 230516:
								convId = 211447; break;

							//A5 
							//Miner
							case 309836: case 337417:
								convId = 309840; break;

						}
						if (convId != -1)
                        {
							player.Conversations.StartConversation(convId);
                        }
					}
			}
			catch { }
		}
	}
}
