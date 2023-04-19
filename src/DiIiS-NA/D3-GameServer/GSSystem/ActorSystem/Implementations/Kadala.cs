using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.GameSystem;
using System.Collections.Generic;
using System.Linq;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	[HandledSNO(ActorSno._x1_randomitemnpc)]
	public class Kadala : Vendor
	{
		private static readonly int[] itemGbIds = new int[]
		{
			-1492848355,
			-594428401,
			2050033703,
			-2026108002,
			-537237168,
			-1493063970,
			-2010009315,
			1281756953,
			-1492484569,
			1816611999,
			-767866790,
			-1099096773,
			-1780286480,
			215071258,
			-1492657844,
			-1843121997
		};
		public Kadala(MapSystem.World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}

		protected override List<ItemsSystem.Item> GetVendorItems()
		{
			return itemGbIds.Select(x => ItemsSystem.ItemGenerator.CookFromDefinition(World, ItemsSystem.ItemGenerator.GetItemDefinition(x), 1, false)).ToList();
		}

		public override bool Reveal(PlayerSystem.Player player)
		{
			if (World.Game.CurrentAct != ActEnum.OpenWorld) return false;
			return base.Reveal(player);
		}

		public override void OnRequestBuyItem(PlayerSystem.Player player, uint itemId)
		{
			int currentShards = player.Inventory.GetBloodShardsAmount();
			ItemsSystem.Item item = _vendorGrid.GetItemByDynId(player, itemId);
			if (item == null)
				return;
			if (player.Toon.IsSeasoned)
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
