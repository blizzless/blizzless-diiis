
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
	[HandledSNO(ActorSno._playerheadstone /* PlayerHeadstone.acr */)]
	class Headstone : Gizmo
	{
		public int playerIndex { get; set; }

		public Headstone(World world, ActorSno sno, TagMap tags, int playerIndex = -1)
			: base(world, sno, tags)
		{
			this.playerIndex = playerIndex;
			this.Attributes[GameAttribute.MinimapActive] = true;
			this.Attributes[GameAttribute.Headstone_Player_ANN] = 1;
			this.Attributes[GameAttribute.TeamID] = 1;
			if (this.World.Game.PvP) this.Attributes[GameAttribute.Disabled] = true;
		}


		public override bool Reveal(PlayerSystem.Player player)
		{
			if (!base.Reveal(player))
				return false;
			return true;
		}

		public override void OnTargeted(PlayerSystem.Player player, TargetMessage message)
		{
			base.OnTargeted(player, message);
			if (this.playerIndex > -1)
				this.GetPlayersInRange(100f).Where(p => p.PlayerIndex == this.playerIndex).First().Resurrect();
			//this.Destroy();
		}
	}
}
