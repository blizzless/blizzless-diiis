//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD;
//Blizzless Project 2022 
using System.Linq;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Actions
{
	public class PowerAction : ActorAction
	{
		const float MaxTargetRange = 30f;
		const float PathUpdateDelay = 1f;

		private Actor _target;
		private PowerScript _power;
		private bool _powerRan;
		private TickTimer _powerFinishTimer;
		private float _baseAttackRadius;
		private ActorMover _ownerMover;

		public PowerAction(Actor owner, int powerSNO, Actor target = null)
			: base(owner)
		{
			_power = PowerLoader.CreateImplementationForPowerSNO(powerSNO);
			_power.World = owner.World;
			_power.User = owner;
			_powerRan = false;
			_baseAttackRadius = this.Owner.ActorData.Cylinder.Ax2 + (_power.EvalTag(PowerKeys.AttackRadius) > 0f ? (powerSNO == 30592 ? 10f : _power.EvalTag(PowerKeys.AttackRadius)) : 35f);
			_ownerMover = new ActorMover(owner);
			_target = target;
		}

		public override void Start(int tickCounter)
		{
			this.Started = true;
			this.Update(tickCounter);
		}

		public override void Update(int tickCounter)
		{
			// if power executed, wait for attack/cooldown to finish.
			if (_powerRan)
			{
				if (_powerFinishTimer.TimedOut)
					this.Done = true;

				return;
			}

			// try to get nearest target if no target yet acquired
			if (_target == null)
			{
				if (this.Owner is Minion || this.Owner is Hireling) // assume minions are player controlled and are targeting monsters
				{
					if ((this.Owner.World.Game.PvP || this.Owner.World.IsPvP) && (this.Owner as Minion).Master != null)
						_target = this.Owner.GetPlayersInRange(MaxTargetRange)
						   .Where(
						   p => p.GlobalID != (this.Owner as Minion).Master.GlobalID)
						   .OrderBy(
						   (player) => PowerMath.Distance2D(player.Position, this.Owner.Position))
						   .FirstOrDefault();
					else
						_target = this.Owner.GetMonstersInRange(MaxTargetRange).OrderBy(
							(monster) => PowerMath.Distance2D(monster.Position, this.Owner.Position))
							.FirstOrDefault();
				}
				else  // monsters targeting players
				{
					_target = this.Owner.GetPlayersInRange(MaxTargetRange).OrderBy(
						(player) => PowerMath.Distance2D(player.Position, this.Owner.Position))
						.FirstOrDefault();
				}
			}

			if (_target != null)
			{
				float targetDistance = PowerMath.Distance2D(_target.Position, this.Owner.Position);

				// if target has moved out of range, deselect it as the target
				if (targetDistance > MaxTargetRange)
				{
					_target = null;
				}
				else if (targetDistance < _baseAttackRadius + _target.ActorData.Cylinder.Ax2)  // run power if within range
				{
					// stop any movement
					_ownerMover.Move(this.Owner.Position, this.Owner.WalkSpeed);
					if (this.Owner is Monster)
					{
						/*(this.Owner as Monster).CorrectedPosition = new Core.Types.Math.Vector3D(this.Owner.Position.X, this.Owner.Position.Y, _target.Position.Z);
						//if()
						
						if (this.Owner.World.WorldSNO.Id == 1 ||
							this.Owner.World.WorldSNO.Id == 1)
							this.Owner.World.BroadcastIfRevealed(plr => new ACDTranslateSyncMessage() { ActorId = this.Owner.DynamicID(plr), Position = (this.Owner as Monster).CorrectedPosition, Snap = false }, this.Owner);
						else
							this.Owner.World.BroadcastIfRevealed(plr => new ACDTranslateSyncMessage() { ActorId = this.Owner.DynamicID(plr), Position = this.Owner.Position, Snap = false }, this.Owner);
						//*/
					}
					/*
					//this.Target.Position = new Core.Types.Math.Vector3D(this.Target.Position.X, this.Target.Position.Y, this.Context.User.Position.Z);

									plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.ACD. ACDTranslateSyncMessage()
									{
										ActorId = this.Target.DynamicID(plr),
										Position = new Core.Types.Math.Vector3D(this.Target.Position.X, this.Target.Position.Y, this.Context.User.Position.Z),
										Snap = false
									});
									//this.Target.World.BroadcastIfRevealed(plr => new MessageSystem.Message.Definitions.ACD.ACDTranslateSyncMessage() { ActorId = this.Target.DynamicID(plr), Position = this.Target.Position, Snap = false }, this.Target);
									
					//*/
					this.Owner.World.PowerManager.RunPower(this.Owner, _power, _target, _target.Position);
					_powerFinishTimer = new SecondsTickTimer(this.Owner.World.Game,
						_power.EvalTag(PowerKeys.AttackSpeed));// + _power.EvalTag(PowerKeys.CooldownTime));
					_powerRan = true;
				}
				else
				{
					this.Done = true;
				}
			}
		}

		public override void Cancel(int tickCounter)
		{
			// TODO: make this per-power instead?
			if (_powerRan)
				this.Owner.World.PowerManager.CancelAllPowers(this.Owner);

			this.Done = true;
		}
	}
}
