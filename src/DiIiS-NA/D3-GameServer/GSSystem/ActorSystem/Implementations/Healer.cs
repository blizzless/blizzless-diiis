//Blizzless Project 2022 
using System.Linq;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
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
	[HandledSNO(
		ActorSno._priest_male_b_nolook,
		ActorSno._priest_caldeum,
		ActorSno._priest_bastionskeep_healer,
		ActorSno._x1_a5_westmhub_healer
	)]
	public sealed class Healer : InteractiveNPC
	{
		public Healer(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
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
