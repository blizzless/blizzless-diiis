using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Interactions;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;

namespace DiIiS_NA.D3_GameServer.GSSystem.ActorSystem.Implementations.Artisans
{
	public class Artisan : InteractiveNPC
	{
		public Artisan(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Interactions.Add(new CraftInteraction());
			//Interactions.Add(new IdentifyAllInteraction());
		}

		public override void OnCraft(Player player)
		{
			Logger.Trace("Artisan {0} opened by {1}", SNO, player?.Toon?.Name);
			player.InGameClient.SendMessage(new ANNDataMessage(Opcodes.OpenArtisanWindowMessage) { ActorID = DynamicID(player) });
		}

	}
}
