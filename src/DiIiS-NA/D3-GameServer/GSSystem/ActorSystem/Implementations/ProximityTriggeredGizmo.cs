//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD;
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

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	class ProximityTriggeredGizmo : Gizmo
	{
		private bool _collapsed = false;

		public ProximityTriggeredGizmo(MapSystem.World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			//this.Field2 = 0x9;//16;
			//this.Field7 = 0x00000001;
			this.CollFlags = 0;

		}

		public override void OnPlayerApproaching(PlayerSystem.Player player)
		{
			try
			{
				if (player.Position.DistanceSquared(ref _position) < ActorData.Sphere.Radius * ActorData.Sphere.Radius * this.Scale * this.Scale && !_collapsed)
				{
					_collapsed = true;

					if (this.ActorSNO.Id == 59401) //caOut_Oasis_Attack_Plant
					{
						Task.Delay(1000).ContinueWith(delegate
						{
							this.World.PowerManager.RunPower(this, 102874);
						});
					}

					// TODO most of the fields here are unknown, find out about animation playing duration
					int duration = 500; // ticks
					if (AnimationSet != null)
						World.BroadcastIfRevealed(plr => new PlayAnimationMessage
						{
							ActorID = this.DynamicID(plr),
							AnimReason = 11,
							UnitAniimStartTime = 0,
							tAnim = new PlayAnimationMessageSpec[]
							{
							new PlayAnimationMessageSpec()
							{
								Duration = duration,
								AnimationSNO = ActorData.TagMap.ContainsKey(ActorKeys.DeathAnimationTag) ? AnimationSet.TagMapAnimDefault[ActorData.TagMap[ActorKeys.DeathAnimationTag]].Int : AnimationSet.TagMapAnimDefault[AnimationSetKeys.DeathDefault] ,
								PermutationIndex = 0,
								AnimationTag = 0,
								Speed = 1
							}
							}

						}, this);

					World.BroadcastIfRevealed(plr => new SetIdleAnimationMessage
					{
						ActorID = this.DynamicID(plr),
						AnimationSNO = AnimationSetKeys.DeadDefault.ID
					}, this);

					World.BroadcastIfRevealed(plr => new ACDCollFlagsMessage
					{
						ActorID = this.DynamicID(plr),
						CollFlags = 0
					}, this);

					this.Attributes[GameAttribute.Deleted_On_Server] = true;
					Attributes.BroadcastChangedIfRevealed();

					RelativeTickTimer destroy = new RelativeTickTimer(World.Game, duration, x => this.Destroy());
				}
			}
			catch { }
		}
	}
}
