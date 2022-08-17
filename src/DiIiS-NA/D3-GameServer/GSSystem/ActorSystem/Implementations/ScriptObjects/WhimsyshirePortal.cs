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

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	[HandledSNO(209083)]
	public class WhimsyshirePortal : Portal
	{
		private bool Opened = false;

		public WhimsyshirePortal(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{

		}

		public override bool Reveal(Player player)
		{
			if (!this.Opened) return false;

			if (!base.Reveal(player))
				return false;

			return true;
		}

		public void Open()
		{
			this.Opened = true;
			foreach (var plr in this.World.Players.Values)
				this.Reveal(plr);
			this.World.GetActorBySNO(209133).Destroy();
		}
	}
}
