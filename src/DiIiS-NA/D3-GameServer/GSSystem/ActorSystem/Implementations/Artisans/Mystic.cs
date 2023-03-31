using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.GameSystem;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;

namespace DiIiS_NA.D3_GameServer.GSSystem.ActorSystem.Implementations.Artisans
{
	[HandledSNO(ActorSno._pt_mystic /* PT_Mystic.acr */)]
	public class Mystic : Artisan
	{
		public Mystic(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}

		public override void OnCraft(Player player)
		{
			base.OnCraft(player);
			player.CurrentArtisan = ArtisanType.Mystic;
		}

		public override bool Reveal(Player player)
		{
			if (!player.MysticUnlocked && player.InGameClient.Game.CurrentAct != ActEnum.OpenWorld)
				return false;

			return base.Reveal(player);
		}
	}
}
