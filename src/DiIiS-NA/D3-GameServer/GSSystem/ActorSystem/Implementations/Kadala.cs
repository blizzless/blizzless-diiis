//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
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
	[HandledSNO(361241)]
	public class Kadala : Vendor
	{
		public Kadala(MapSystem.World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}

		protected override List<ItemsSystem.Item> GetVendorItems()
		{
			var list = new List<ItemsSystem.Item>
			{
				ItemsSystem.ItemGenerator.CookFromDefinition(this.World, ItemsSystem.ItemGenerator.GetItemDefinition(-1492848355), 1, false),
				ItemsSystem.ItemGenerator.CookFromDefinition(this.World, ItemsSystem.ItemGenerator.GetItemDefinition(-594428401), 1, false),
				ItemsSystem.ItemGenerator.CookFromDefinition(this.World, ItemsSystem.ItemGenerator.GetItemDefinition(2050033703), 1, false),
				ItemsSystem.ItemGenerator.CookFromDefinition(this.World, ItemsSystem.ItemGenerator.GetItemDefinition(-2026108002), 1, false),
				ItemsSystem.ItemGenerator.CookFromDefinition(this.World, ItemsSystem.ItemGenerator.GetItemDefinition(-537237168), 1, false),
				ItemsSystem.ItemGenerator.CookFromDefinition(this.World, ItemsSystem.ItemGenerator.GetItemDefinition(-1493063970), 1, false),
				ItemsSystem.ItemGenerator.CookFromDefinition(this.World, ItemsSystem.ItemGenerator.GetItemDefinition(-2010009315), 1, false),
				ItemsSystem.ItemGenerator.CookFromDefinition(this.World, ItemsSystem.ItemGenerator.GetItemDefinition(1281756953), 1, false),
				ItemsSystem.ItemGenerator.CookFromDefinition(this.World, ItemsSystem.ItemGenerator.GetItemDefinition(-1492484569), 1, false),
				ItemsSystem.ItemGenerator.CookFromDefinition(this.World, ItemsSystem.ItemGenerator.GetItemDefinition(1816611999), 1, false),
				ItemsSystem.ItemGenerator.CookFromDefinition(this.World, ItemsSystem.ItemGenerator.GetItemDefinition(-767866790), 1, false),
				ItemsSystem.ItemGenerator.CookFromDefinition(this.World, ItemsSystem.ItemGenerator.GetItemDefinition(-1099096773), 1, false),
				ItemsSystem.ItemGenerator.CookFromDefinition(this.World, ItemsSystem.ItemGenerator.GetItemDefinition(-1780286480), 1, false),
				ItemsSystem.ItemGenerator.CookFromDefinition(this.World, ItemsSystem.ItemGenerator.GetItemDefinition(215071258), 1, false),
				ItemsSystem.ItemGenerator.CookFromDefinition(this.World, ItemsSystem.ItemGenerator.GetItemDefinition(-1492657844), 1, false),
				ItemsSystem.ItemGenerator.CookFromDefinition(this.World, ItemsSystem.ItemGenerator.GetItemDefinition(-1843121997), 1, false)
			};

			return list;
		}

		public override bool Reveal(PlayerSystem.Player player)
		{
			if (this.World.Game.CurrentAct != 3000) return false;
			return base.Reveal(player);
		}

		public override void OnRequestBuyItem(PlayerSystem.Player player, uint itemId)
		{
			int currentShards = player.Inventory.GetBloodShardsAmount();
			ItemsSystem.Item item = _vendorGrid.GetItemByDynId(player, itemId);
			if (item == null)
				return;
			if (player.Toon.isSeassoned)
			{
				player.GrantCriteria(74987248526596);
			}
			if (!player.Inventory.HasInventorySpace(item))
			{
				return;
			}

			int cost = item.ItemDefinition.Cost;
			//Check shards here
			if (currentShards < cost)
				return;

			//Remove the shards
			player.Inventory.RemoveBloodShardsAmount(cost);

			player.Inventory.BuyItem(this, item, false);
			player.Inventory.RefreshInventoryToClient();
		}
	}
}
