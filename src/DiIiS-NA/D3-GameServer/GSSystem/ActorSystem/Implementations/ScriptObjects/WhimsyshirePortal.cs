//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	[HandledSNO(ActorSno._g_portal_tentacle_trist)]
	public class WhimsyshirePortal : Portal
	{
		private bool Opened = false;

		public WhimsyshirePortal(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{

		}

		public override bool Reveal(Player player)
		{
			if (!Opened) return false;

			if (!base.Reveal(player))
				return false;

			return true;
		}

		public void Open()
		{
			Opened = true;
			foreach (var plr in World.Players.Values)
				Reveal(plr);
			World.GetActorBySNO(ActorSno._tentaclelord).Destroy();
		}
	}
}
