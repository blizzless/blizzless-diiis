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
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
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
	[HandledSNO(ActorSno._a2dun_cald_belial_room_a_breakable_main)]
	public class BelialRoom : Gizmo
	{
		public BelialRoom(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			bool Activated = false;
			this.Attributes[GameAttribute.Team_Override] = (Activated ? -1 : 2);
			this.Attributes[GameAttribute.Untargetable] = !Activated;
			this.Attributes[GameAttribute.NPC_Is_Operatable] = Activated;
			this.Attributes[GameAttribute.Operatable] = Activated;
			this.Attributes[GameAttribute.Operatable_Story_Gizmo] = Activated;
			this.Attributes[GameAttribute.Disabled] = !Activated;
			this.Attributes[GameAttribute.Immunity] = !Activated;
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

		public void Break()
		{
			World.BroadcastIfRevealed(plr => new PlayAnimationMessage
			{
				ActorID = this.DynamicID(plr),
				AnimReason = 5,
				UnitAniimStartTime = 0,
				tAnim = new PlayAnimationMessageSpec[]
				{
					new PlayAnimationMessageSpec()
					{
						Duration = 600,
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
		}

		public void Rebuild()
		{
			World.BroadcastIfRevealed(plr => new SetIdleAnimationMessage
			{
				ActorID = this.DynamicID(plr),
				AnimationSNO = AnimationSetKeys.GizmoState1.ID
			}, this);
		}

		public override void OnTargeted(Player player, TargetMessage message)
		{
			base.OnTargeted(player, message);
		}
	}
}
