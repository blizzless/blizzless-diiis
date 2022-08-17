//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Artisans
{
	[HandledSNO(56948 /* PT_Mystic.acr */)]
	public class Mystic : Artisan
	{
		public Mystic(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
		}

		public override void OnCraft(Player player)
		{
			player.InGameClient.SendMessage(new ANNDataMessage(Opcodes.OpenArtisanWindowMessage) { ActorID = this.DynamicID(player) });
			player.ArtisanInteraction = "Mystic";
		}

		public override bool Reveal(Player player)
		{
			if (!player.MysticUnlocked && player.InGameClient.Game.CurrentAct != 3000)
				return false;

			return base.Reveal(player);
		}
	}
}
