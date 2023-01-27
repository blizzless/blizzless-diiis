using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.ScriptObjects
{
	[HandledSNO(ActorSno._evacuation_refugee_cart)]
	public class RefugeeCart : Gizmo
	{
		public RefugeeCart(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			//this.Attributes[GameAttribute.MinimapActive] = true;
		}


		public override bool Reveal(Player player)
		{
			if (!base.Reveal(player))
				return false;

			return true;
		}

		public override bool Unreveal(Player player)
		{
			if (!base.Unreveal(player))
				return false;

			return true;
		}

		public override void OnTargeted(Player player, TargetMessage message)
		{
			if (World.Game.CurrentQuest == 121792 && World.Game.CurrentStep == 21)
			{
				base.OnTargeted(player, message);
				player.AddFollower(World.GetActorBySNO(ActorSno._caldeumpoor_male_f_ambient));
				Attributes[GameAttribute.Gizmo_Has_Been_Operated] = true;
				Attributes[GameAttribute.Disabled] = true;
				Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
}
