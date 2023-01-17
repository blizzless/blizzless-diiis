//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
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
	[HandledSNO(
		ActorSno._a3dun_wall_lift_gategizmolong,
		ActorSno._a3dun_wall_lift_gategizmorightface,
		ActorSno._a3dun_wall_lift_gategizmo
	)]
	public class ActIIICatapult : Gizmo
	{
		public bool activated = false;

		public ActIIICatapult(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.Attributes[GameAttribute.MinimapActive] = true;
			this.Attributes[GameAttribute.MinimapDisableArrow] = true;
		}


		public override bool Reveal(Player player)
		{
			if (!base.Reveal(player))
				return false;

			if (this.activated)
			{
				player.InGameClient.SendMessage(new SetIdleAnimationMessage
				{
					ActorID = this.DynamicID(player),
					AnimationSNO = AnimationSetKeys.Open.ID
				});
			}

			return true;
		}

		public void Raise()
		{
			if (this.activated) return;

			World.BroadcastIfRevealed(plr => new PlayAnimationMessage
			{
				ActorID = this.DynamicID(plr),
				AnimReason = 5,
				UnitAniimStartTime = 0,
				tAnim = new PlayAnimationMessageSpec[]
				{
					new PlayAnimationMessageSpec()
					{
						Duration = 1000,
						AnimationSNO = AnimationSet.TagMapAnimDefault[AnimationSetKeys.Opening],
						PermutationIndex = 0,
						AnimationTag = 0,
						Speed = 1
					}
				}

			}, this);

			World.BroadcastIfRevealed(plr => new SetIdleAnimationMessage
			{
				ActorID = this.DynamicID(plr),
				AnimationSNO = AnimationSetKeys.Open.ID
			}, this);

			this.activated = true;
		}

		public override bool Unreveal(Player player)
		{
			if (!base.Unreveal(player))
				return false;

			return true;
		}
	}
}
