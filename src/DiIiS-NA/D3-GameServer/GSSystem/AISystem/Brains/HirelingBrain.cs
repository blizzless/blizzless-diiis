using System;
using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.Core.MPQ;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Actions;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
using DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
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
			Owner = master;

			PresetPowers = new List<int>();

			if (body is Templar && body is MalthaelHireling)
				PresetPowers.Add(30592); //melee instant
			if (body is Scoundrel)
				PresetPowers.Add(99902); //Scoundrel_ranged_Projectile
			if (body is Enchantress)
				PresetPowers.Add(30273); //HirelingMage_MagicMissile
			if (body is Leah)
				PresetPowers.Add(99902); //Scoundrel_ranged_Projectile

		}

		public override void Think(int tickCounter)
		{
			if (Owner == null) return;

			if (Body.World.Game.Paused) return;

			// check if in disabled state, if so cancel any action then do nothing
			if (Body.Attributes[GameAttribute.Frozen] ||
				Body.Attributes[GameAttribute.Stunned] ||
				Body.Attributes[GameAttribute.Blind] ||
				Body.Attributes[GameAttribute.Webbed] ||
				Body.Disable ||
				Body.World.BuffManager.GetFirstBuff<KnockbackBuff>(Body) != null)
			{
				if (CurrentAction != null)
				{
					CurrentAction.Cancel(tickCounter);
					CurrentAction = null;
				}
				_powerDelay = null;

				return;
			}

			if (Body.Attributes[GameAttribute.Feared])
			{
				if (!Feared || CurrentAction == null)
				{
					if (CurrentAction != null)
					{
						CurrentAction.Cancel(tickCounter);
						CurrentAction = null;
					}
					Feared = true;
					CurrentAction = new MoveToPointWithPathfindAction(
						Body,
						PowerContext.RandomDirection(Body.Position, 3f, 8f)
					);
					return;
				}
				else return;
			}
			else
				Feared = false;

			// select and start executing a power if no active action
			if (CurrentAction == null)
			{
				// do a little delay so groups of monsters don't all execute at once
				if (_powerDelay == null)
					_powerDelay = new SecondsTickTimer(Body.World.Game, 1f);

				var targets = Owner.GetObjectsInRange<Monster>(40f).Where(p => !p.Dead && p.Visible).OrderBy(m => PowerMath.Distance2D(m.Position, Body.Position)).ToList();
				if (targets.Count != 0 && PowerMath.Distance2D(Body.Position, Owner.Position) < 80f)
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
						power.User = Body;
						float attackRange = Body.ActorData.Cylinder.Ax2 + (power.EvalTag(PowerKeys.AttackRadius) > 0f ? (powerToUse == 30592 ? 10f : power.EvalTag(PowerKeys.AttackRadius)) : 35f);
						float targetDistance = PowerMath.Distance2D(_target.Position, Body.Position);
						if (targetDistance < attackRange + _target.ActorData.Cylinder.Ax2)
						{
							if (_powerDelay.TimedOut)
							{
								_powerDelay = null;
								Body.TranslateFacing(_target.Position, false);

								CurrentAction = new PowerAction(Body, powerToUse, _target);
							}
						}
						else
						{
							CurrentAction = new MoveToTargetWithPathfindAction(
								Body,
								_target,
								attackRange + _target.ActorData.Cylinder.Ax2
							);
						}
					}
				}
				else
				{
					var distToMaster = PowerMath.Distance2D(Body.Position, Owner.Position);
					if ((distToMaster > 8f) || (distToMaster < 3f))
					{
						var Rand = FastRandom.Instance;
						var position = Owner.Position;
						float angle = (float)(Rand.NextDouble() * Math.PI * 2);
						float radius = 3f + (float)Rand.NextDouble() * (8f - 3f);
						var near = new Vector3D(position.X + (float)Math.Cos(angle) * radius, position.Y + (float)Math.Sin(angle) * radius, position.Z);
						CurrentAction = new MoveToPointAction(Body, near);
					}
				}
			}
		}

		protected virtual int PickPowerToUse()
		{
											  // randomly used an implemented power
			if (PresetPowers.Count > 0)
			{
				int powerIndex = RandomHelper.Next(PresetPowers.Count);
				if (PowerLoader.HasImplementationForPowerSNO(PresetPowers[powerIndex]))
					return PresetPowers[powerIndex];
			}

			// no usable power
			return -1;
		}

		public void AddPresetPower(int powerSNO)
		{
			PresetPowers.Add(powerSNO);
		}
	}
}
