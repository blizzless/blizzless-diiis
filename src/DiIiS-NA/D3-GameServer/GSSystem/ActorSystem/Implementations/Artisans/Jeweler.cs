using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.GameSystem;
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.D3_GameServer.GSSystem.ActorSystem.Implementations.Artisans
{
	[HandledSNO(ActorSno._pt_jeweler /* PT_Jewler.acr */)]
	public class Jeweler : Artisan
	{
		public Jeweler(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
		}

		public void OnAddSocket(Player player, Item item)
		{
			// TODO: Animate Jeweler? Who knows. /fasbat
			item.Attributes[GameAttributes.Sockets] += 1;
			// Why this not work? :/
			item.Attributes.SendChangedMessage(player.InGameClient);
		}

		public override void OnCraft(Player player)
		{
			base.OnCraft(player);
			player.CurrentArtisan = ArtisanType.Jeweler;
		}

		public override bool Reveal(Player player)
		{
			if (!player.JewelerUnlocked && player.InGameClient.Game.CurrentAct != ActEnum.OpenWorld)
				return false;

			return base.Reveal(player);
		}
	}
}
