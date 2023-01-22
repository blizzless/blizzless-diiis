//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
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
			item.Attributes[GameAttribute.Sockets] += 1;
			// Why this not work? :/
			item.Attributes.SendChangedMessage(player.InGameClient);
		}

		public override void OnCraft(Player player)
		{
			player.InGameClient.SendMessage(new ANNDataMessage(Opcodes.OpenArtisanWindowMessage) { ActorID = DynamicID(player) });
			player.ArtisanInteraction = "Jeweler";
		}

		public override bool Reveal(Player player)
		{
			if (!player.JewelerUnlocked && player.InGameClient.Game.CurrentAct != 3000)
				return false;

			return base.Reveal(player);
		}
	}
}
