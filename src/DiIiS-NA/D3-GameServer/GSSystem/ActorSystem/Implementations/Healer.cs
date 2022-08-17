//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	[HandledSNO(141246, 226343, 226345, 309879)]
	public sealed class Healer : InteractiveNPC
	{
		public Healer(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.Attributes[GameAttribute.TeamID] = 0;
			this.Attributes[GameAttribute.MinimapActive] = true;
		}

		public override void OnTargeted(PlayerSystem.Player player, TargetMessage message)
		{
			player.AddPercentageHP(100);
			base.OnTargeted(player, message);
		}
	}
}
