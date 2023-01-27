using DiIiS_NA.Core.Logging;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;

namespace DiIiS_NA.GameServer.GSSystem.ItemsSystem.Implementations
{
	[HandledType("Book")]
	public class Book : Item
	{
		public static readonly Logger Logger = LogManager.CreateLogger();

		public int LoreSNOId { get; private set; }

		public Book(MapSystem.World world, DiIiS_NA.Core.MPQ.FileFormats.GameBalance.ItemTable definition, int cork = -1, bool cork2 = false, int cork3 = -1)
			: base(world, definition)
		{
			var actorData = ActorSNO.Target as DiIiS_NA.Core.MPQ.FileFormats.Actor;

			if (actorData.TagMap.ContainsKey(ActorKeys.Lore))
			{
				LoreSNOId = actorData.TagMap[ActorKeys.Lore].Id;
			}
		}

		public override void OnTargeted(PlayerSystem.Player player, TargetMessage message)
		{
			//Logger.Trace("OnTargeted");
			if (LoreSNOId != -1)
			{
				player.PlayLore(LoreSNOId, true);
			}
			if (player.GroundItems.ContainsKey(GlobalID))
				player.GroundItems.Remove(GlobalID);
			Destroy();
		}
	}
}
