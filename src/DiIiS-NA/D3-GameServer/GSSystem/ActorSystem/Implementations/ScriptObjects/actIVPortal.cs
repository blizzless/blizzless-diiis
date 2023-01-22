//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
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

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.ScriptObjects
{
	[HandledSNO(ActorSno._event47_bigportal)]
	public class ActIVPortal : Gizmo
	{
		public ActIVPortal(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			//this.Attributes[GameAttribute.MinimapActive] = true;
		}


		public override bool Reveal(Player player)
		{
			if (!base.Reveal(player))
				return false;

			player.InGameClient.SendMessage(new SetIdleAnimationMessage
			{
				ActorID = DynamicID(player),
				AnimationSNO = AnimationSetKeys.Open.ID
			});

			return true;
		}

		public override bool Unreveal(Player player)
		{
			if (!base.Unreveal(player))
				return false;

			return true;
		}
	}
}
