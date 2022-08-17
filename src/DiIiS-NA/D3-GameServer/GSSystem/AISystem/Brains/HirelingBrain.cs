//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Actions;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.AISystem.Brains
{
	public class HirelingBrain : Brain
	{
		public Player Owner { get; private set; }

		public List<int> PresetPowers { get; private set; }
		private Actor _target { get; set; }
		private TickTimer _powerDelay;
		private bool Feared = false;

		public HirelingBrain(Actor body, Player master)
			: base(body)
		{
			this.Owner = master;

			this.PresetPowers = new List<int>();

			if (body is Templar && body is MalthaelHireling)
				this.PresetPowers.Add(30592); //melee instant
			if (body is Scoundrel)
				this.PresetPowers.Add(99902); //Scoundrel_ranged_Projectile
			if (body is Enchantress)
				this.PresetPowers.Add(30273); //HirelingMage_MagicMissile
			if (body is Leah)
				this.PresetPowers.Add(99902); //Scoundrel_ranged_Projectile

		}

		public override void Think(int tickCounter)
		{
			if (this.Owner == null) return;

			if (this.Body.World.Game.Paused) return;

			// check if in disabled state, if so cancel any action then do nothing
			if (this.Body.Attributes[GameAttribute.Frozen] ||
				this.Body.Attributes[GameAttribute.Stunned] ||
				this.Body.Attributes[GameAttribute.Blind] ||
				this.Body.Attributes[GameAttribute.Webbed] ||
				this.Body.Disable ||
				this.Body.World.BuffManager.GetFirstBuff<PowerSystem.Implementations.KnockbackBuff>(this.Body) != null)
			{
				if (this.CurrentAction != null)
				{
					this.CurrentAction.Cancel(tickCounter);
					this.CurrentAction = null;
				}
				_powerDelay = null;

				return;
			}

			if (this.Body.Attributes[GameAttribute.Feared])
			{
				if (!this.Feared || this.CurrentAction == null)
				{
					if (this.CurrentAction != null)
					{
						this.CurrentAction.Cancel(tickCounter);
						this.CurrentAction = null;
					}
					this.Feared = true;
					this.CurrentAction = new MoveToPointWithPathfindAction(
						this.Body,
						PowerContext.RandomDirection(this.Body.Position, 3f, 8f)
					);
					return;
				}
				else return;
			}
			else
				this.Feared = false;

			// select and start executing a power if no active action
			if (this.CurrentAction == null)
			{
				// do a little delay so groups of monsters don't all execute at once
				if (_powerDelay == null)
					_powerDelay = new SecondsTickTimer(this.Body.World.Game, 1f);

				var targets = this.Owner.GetObjectsInRange<Monster>(40f).Where(p => !p.Dead && p.Visible).OrderBy(m => PowerMath.Distance2D(m.Position, this.Body.Position)).ToList();
				if (targets.Count != 0 && PowerMath.Distance2D(this.Body.Position, this.Owner.Position) < 80f)
				{
					int powerToUse = PickPowerToUse();
					if (powerToUse > 0)
					{
						var elites = targets.Where(t => t is Champion || t is Rare || t is RareMinion);
						if (elites.Count() > 0)
							_target = elites.First();
						else
							_target = targets.First();

						PowerScript power = PowerLoader.CreateImplementationForPowerSNO(powerToUse);
						power.User = this.Body;
						float attackRange = this.Body.ActorData.Cylinder.Ax2 + (power.EvalTag(PowerKeys.AttackRadius) > 0f ? (powerToUse == 30592 ? 10f : power.EvalTag(PowerKeys.AttackRadius)) : 35f);
						float targetDistance = PowerMath.Distance2D(_target.Position, this.Body.Position);
						if (targetDistance < attackRange + _target.ActorData.Cylinder.Ax2)
						{
							if (_powerDelay.TimedOut)
							{
								_powerDelay = null;
								this.Body.TranslateFacing(_target.Position, false);

								this.CurrentAction = new PowerAction(this.Body, powerToUse, _target);
							}
						}
						else
						{
							this.CurrentAction = new MoveToTargetWithPathfindAction(
								this.Body,
								_target,
								attackRange + _target.ActorData.Cylinder.Ax2
							);
						}
					}
				}
				else
				{
					var distToMaster = PowerMath.Distance2D(this.Body.Position, this.Owner.Position);
					if ((distToMaster > 8f) || (distToMaster < 3f))
					{
						var Rand = FastRandom.Instance;
						var position = this.Owner.Position;
						float angle = (float)(Rand.NextDouble() * Math.PI * 2);
						float radius = 3f + (float)Rand.NextDouble() * (8f - 3f);
						var near = new Vector3D(position.X + (float)Math.Cos(angle) * radius, position.Y + (float)Math.Sin(angle) * radius, position.Z);
						this.CurrentAction = new MoveToPointAction(this.Body, near);
					}
				}
			}
		}

		protected virtual int PickPowerToUse()
		{
											  // randomly used an implemented power
			if (this.PresetPowers.Count > 0)
			{
				int powerIndex = RandomHelper.Next(this.PresetPowers.Count);
				if (PowerSystem.PowerLoader.HasImplementationForPowerSNO(this.PresetPowers[powerIndex]))
					return this.PresetPowers[powerIndex];
			}

			// no usable power
			return -1;
		}

		public void AddPresetPower(int powerSNO)
		{
			this.PresetPowers.Add(powerSNO);
		}
	}
}
