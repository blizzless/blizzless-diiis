using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Artisans
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
			player.InGameClient.SendMessage(new ANNDataMessage(Opcodes.OpenArtisanWindowMessage) { ActorID = DynamicID(player) });
			player.ArtisanInteraction = "Blacksmith";
		}

		public override bool Reveal(Player player)
		{
			if (!player.BlacksmithUnlocked && player.InGameClient.Game.CurrentAct != 3000)
				return false;

			return base.Reveal(player);
		}
	}
}
