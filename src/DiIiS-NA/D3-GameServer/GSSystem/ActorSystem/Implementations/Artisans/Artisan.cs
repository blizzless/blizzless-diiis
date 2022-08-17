//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Interactions;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Artisans
{
	public class Artisan : InteractiveNPC
	{
		public Artisan(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.Attributes[GameAttribute.MinimapActive] = true;
			Interactions.Add(new CraftInteraction());
			//Interactions.Add(new IdentifyAllInteraction());
		}

		public override void OnCraft(Player player)
		{
			player.InGameClient.SendMessage(new ANNDataMessage(Opcodes.OpenArtisanWindowMessage) { ActorID = this.DynamicID(player) });
			//player.InGameClient.SendMessage(new OpenArtisanWindowMessage() { ArtisanID = this.DynamicID(player) });
		}

	}
}
