using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.GameSystem;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;

namespace DiIiS_NA.D3_GameServer.GSSystem.ActorSystem.Implementations.Artisans
{
	[HandledSNO(ActorSno._pt_blacksmith /* PT_Blacksmith.acr */)]
	public class Blacksmith : Artisan
	{
		public Blacksmith(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			// TODO add all blacksmith functionality? /fasbat
			//this.Attributes[GameAttribute.TeamID] = 0;
			//this.Attributes[GameAttribute.MinimapIconOverride] = 102320;
		}

		public override void OnCraft(Player player)
		{
			base.OnCraft(player);
			player.CurrentArtisan = ArtisanType.Blacksmith;
		}

		public override bool Reveal(Player player)
		{
			if (!player.BlacksmithUnlocked && player.InGameClient.Game.CurrentAct != ActEnum.OpenWorld)
				return false;

			return base.Reveal(player);
		}
	}
}
