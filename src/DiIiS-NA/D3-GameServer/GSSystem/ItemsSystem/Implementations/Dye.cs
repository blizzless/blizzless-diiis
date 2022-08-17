//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using System.Collections.Generic;

namespace DiIiS_NA.GameServer.GSSystem.ItemsSystem.Implementations
{
	[HandledType("Dye")]
	public class Dye : Item
	{
		private static Dictionary<int, int> DyeColorMap = new Dictionary<int, int>();

		public Dye(World world, DiIiS_NA.Core.MPQ.FileFormats.GameBalance.ItemTable definition, int cork = -1, bool cork2 = false, int cork3 = -1)
			: base(world, definition)
		{
		}

		public override void OnRequestUse(Player player, Item target, int actionId, WorldPlace worldPlace)
		{
			//Debug.Assert(target != null);
			if (target == null) return;

			target.Attributes[GameAttribute.DyeType] = this.Attributes[GameAttribute.DyeType];
			target.DBInventory.DyeType = this.Attributes[GameAttribute.DyeType];

			player.World.Game.GameDBSession.SessionUpdate(target.DBInventory);

			player.Inventory.SendVisualInventory(player);

			if (this.GBHandle.GBID == 1866876233 || this.GBHandle.GBID == 1866876234) return;

			if (this.Attributes[GameAttribute.ItemStackQuantityLo] <= 1)
				player.Inventory.DestroyInventoryItem(this); // No more dyes!
			else
			{
				this.UpdateStackCount(--this.Attributes[GameAttribute.ItemStackQuantityLo]); // Just remove one
				this.Attributes.SendChangedMessage(player.InGameClient);
			}
		}
	}
}
