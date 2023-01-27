using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
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

			target.Attributes[GameAttribute.DyeType] = Attributes[GameAttribute.DyeType];
			target.DBInventory.DyeType = Attributes[GameAttribute.DyeType];

			player.World.Game.GameDBSession.SessionUpdate(target.DBInventory);

			player.Inventory.SendVisualInventory(player);

			if (GBHandle.GBID == 1866876233 || GBHandle.GBID == 1866876234) return;

			if (Attributes[GameAttribute.ItemStackQuantityLo] <= 1)
				player.Inventory.DestroyInventoryItem(this); // No more dyes!
			else
			{
				UpdateStackCount(--Attributes[GameAttribute.ItemStackQuantityLo]); // Just remove one
				Attributes.SendChangedMessage(player.InGameClient);
			}
		}
	}
}
