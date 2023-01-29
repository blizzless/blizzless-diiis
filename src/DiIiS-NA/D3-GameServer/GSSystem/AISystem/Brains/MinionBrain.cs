using System;
using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.Core.Extensions;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.Core.MPQ;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
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
	public class MinionBrain : Brain
	{
		// list of power SNOs that are defined for the monster
		public Dictionary<int, Cooldown> PresetPowers { get; private set; }

		private TickTimer _powerDelay;
		private bool Feared = false;
		private Actor _target { get; set; }
		private bool _warnedNoPowers;

		public struct Cooldown
		{
			public TickTimer CooldownTimer;
			public float CooldownTime;
		}

		public MinionBrain(Actor body)
			: base(body)
		{
			PresetPowers = new Dictionary<int, Cooldown>();

			// build list of powers defined in monster mpq data
			if (body.ActorData.MonsterSNO > 0)
			{
				var monsterData = (DiIiS_NA.Core.MPQ.FileFormats.Monster)MPQStorage.Data.Assets[SNOGroup.Monster][body.ActorData.MonsterSNO].Data;
				foreach (var monsterSkill in monsterData.SkillDeclarations)
				{
					if (monsterSkill.SNOPower > 0)
					{
						PresetPowers.Add(monsterSkill.SNOPower, new Cooldown { CooldownTimer = null, CooldownTime = 1f });
					}
				}
			}
		}

		public override void Think(int tickCounter)
		{
			// this needed? /mdz
			//if (this.Body is NPC) return;
			if ((Body as Minion).Master == null) return;

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
					_powerDelay = new SecondsTickTimer(Body.World.Game, (float)RandomHelper.NextDouble());

				if (_powerDelay.TimedOut)
				{
					List<Actor> targets = (Body as Minion).Master
						.GetObjectsInRange<Monster>(40f)
						.Where(m => !m.Dead && m.Visible && m.SNO.IsTargetable())
						.OrderBy(m => PowerMath.Distance2D(m.Position, Body.Position))
						.Cast<Actor>()
						.ToList();
					if (Body.World.Game.PvP)
						targets = (Body as Minion).Master.GetObjectsInRange<Player>(30f).Where(p => p.GlobalID != (Body as Minion).Master.GlobalID && p.Attributes[GameAttribute.TeamID] != (Body as Minion).Master.Attributes[GameAttribute.TeamID]).Cast<Actor>().ToList();
					if (Body.World.IsPvP)
						targets = (Body as Minion).Master.GetObjectsInRange<Player>(30f).Where(p => p.GlobalID != (Body as Minion).Master.GlobalID).Cast<Actor>().ToList();

					if (targets.Count != 0 && PowerMath.Distance2D(Body.Position, (Body as Minion).Master.Position) < 80f)
					{
						var elite = targets.FirstOrDefault(target => target is Champion or Rare or RareMinion);
						_target = elite ?? targets.First();

						int powerToUse = PickPowerToUse();
						if (powerToUse > 0)
						{
							PowerScript power = PowerLoader.CreateImplementationForPowerSNO(powerToUse);
							power.User = Body;
							float attackRange = Body.ActorData.Cylinder.Ax2 + (power.EvalTag(PowerKeys.AttackRadius) > 0f ? (powerToUse == 30592 ? 10f : power.EvalTag(PowerKeys.AttackRadius)) : 35f);
							float targetDistance = PowerMath.Distance2D(_target.Position, Body.Position);
							if (targetDistance < attackRange + _target.ActorData.Cylinder.Ax2)
							{
								if (Body.WalkSpeed != 0)
									Body.TranslateFacing(_target.Position, false); //columns and other non-walkable shit can't turn

								float cdReduction = (Body as Minion).CooldownReduction;

								//Logger.Trace("PowerAction to target");
								CurrentAction = new PowerAction(Body, powerToUse, _target);

								if (power is SummoningSkill)
									PresetPowers[powerToUse] = new Cooldown { CooldownTimer = null, CooldownTime = (7f * cdReduction) };

								if (PresetPowers[powerToUse].CooldownTime > 0f)
									PresetPowers[powerToUse] = new Cooldown { CooldownTimer = new SecondsTickTimer(Body.World.Game, PresetPowers[powerToUse].CooldownTime), CooldownTime = (PresetPowers[powerToUse].CooldownTime * cdReduction) };
							}
							else
							{
								Logger.Trace("MoveToTargetWithPathfindAction to target");
								CurrentAction = new MoveToTargetWithPathfindAction(
									Body,
									//(
									_target,// + MovementHelpers.GetMovementPosition(
											//new Vector3D(0, 0, 0), 
											//this.Body.WalkSpeed, 
											//MovementHelpers.GetFacingAngle(_target.Position, this.Body.Position),
											//6
											//)
											//)
									attackRange + _target.ActorData.Cylinder.Ax2
								);
							}
						}
					}
					else
					{
						var distToMaster = PowerMath.Distance2D(Body.Position, (Body as Minion).Master.Position);
						if ((distToMaster > 8f) || (distToMaster < 3f))
						{
							var Rand = FastRandom.Instance;
							var position = (Body as Minion).Master.Position;
							float angle = (float)(Rand.NextDouble() * Math.PI * 2);
							float radius = 3f + (float)Rand.NextDouble() * (8f - 3f);
							var near = new Vector3D(position.X + (float)Math.Cos(angle) * radius, position.Y + (float)Math.Sin(angle) * radius, position.Z);
							CurrentAction = new MoveToPointAction(Body, near);
						}
					}
				}
			}
		}

		protected virtual int PickPowerToUse()
		{
			if (!_warnedNoPowers && PresetPowers.Count == 0)
			{
				Logger.Debug("Minion \"{0}\" has no usable powers. ", Body.Name);
				_warnedNoPowers = true;
			}

			// randomly used an implemented power
			if (PresetPowers.Count > 0)
			{
				// int power = this.PresetPowers[RandomHelper.Next(this.PresetPowers.Count)].Key;
				List<int> availablePowers = PresetPowers.Where(p => (p.Value.CooldownTimer == null || p.Value.CooldownTimer.TimedOut) && PowerLoader.HasImplementationForPowerSNO(p.Key)).Select(p => p.Key).ToList();
				if (availablePowers.Where(p => p != 30592).TryPickRandom(out var randomItem))
					return randomItem;
				if (availablePowers.Contains(30592))
					return 30592; // melee attack
			}

			// no usable power
			return -1;
		}

		public void AddPresetPower(int powerSNO)
		{
			if (PresetPowers.ContainsKey(powerSNO))
			{
				// Logger.MethodTrace("power sno {0} already defined for monster \"{1}\"",
				//powerSNO, this.Body.ActorSNO.Name);
				return;
			}
			if (PresetPowers.ContainsKey(30592)) //if can cast melee
				PresetPowers.Add(powerSNO, new Cooldown { CooldownTimer = null, CooldownTime = 5f });
			else
				PresetPowers.Add(powerSNO, new Cooldown { CooldownTimer = null, CooldownTime = 1f + (float)FastRandom.Instance.NextDouble() });
		}

		public void RemovePresetPower(int powerSNO)
		{
			if (PresetPowers.ContainsKey(powerSNO))
			{
				PresetPowers.Remove(powerSNO);
			}
		}
	}
}
