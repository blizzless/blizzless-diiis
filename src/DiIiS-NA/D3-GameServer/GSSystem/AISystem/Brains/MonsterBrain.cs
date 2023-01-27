using System;
using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.Core.MPQ;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
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
	public class MonsterBrain : Brain
	{
		private new readonly Logger Logger;
		// list of power SNOs that are defined for the monster
		public Dictionary<int, Cooldown> PresetPowers { get; private set; }

		private TickTimer _powerDelay;

		public struct Cooldown
		{
			public TickTimer CooldownTimer;
			public float CooldownTime;
		}

		private bool _warnedNoPowers;
		private Actor _target { get; set; }
		private int _mpqPowerCount;
		private bool Feared = false;

		public Actor AttackedBy = null;
		public TickTimer TimeoutAttacked = null;

		public Actor PriorityTarget = null;

		public MonsterBrain(Actor body)
		: base(body)
		{
			Logger = LogManager.CreateLogger(GetType().Name);
			
			PresetPowers = new Dictionary<int, Cooldown>();

			// build list of powers defined in monster mpq data
			if (body.ActorData.MonsterSNO <= 0)
			{
				Logger.Warn($"$[red]${GetType().Name}$[/]$ - Monster \"{body.SNO}\" has no monster SNO");
				return;
			}
			var monsterData = (DiIiS_NA.Core.MPQ.FileFormats.Monster)MPQStorage.Data.Assets[SNOGroup.Monster][body.ActorData.MonsterSNO].Data;
			_mpqPowerCount = monsterData.SkillDeclarations.Count(e => e.SNOPower != -1);
			for (int i = 0; i < monsterData.SkillDeclarations.Count(); i++)
			{
				if (monsterData.SkillDeclarations[i].SNOPower == -1) continue;
				if (PowerLoader.HasImplementationForPowerSNO(monsterData.SkillDeclarations[i].SNOPower))
				{
					var cooldownTime = monsterData.MonsterSkillDeclarations[i].Timer / 10f;
					PresetPowers.Add(monsterData.SkillDeclarations[i].SNOPower, new Cooldown { CooldownTimer = null, CooldownTime = cooldownTime });
				}
			}

			if (monsterData.SkillDeclarations.All(s => s.SNOPower != 30592))
				PresetPowers.Add(30592, new Cooldown { CooldownTimer = null, CooldownTime = 0f }); //hack for dummy mobs without powers
		}

		public override void Think(int tickCounter)
		{
			switch (Body.SNO)
			{
				case ActorSno._uber_siegebreakerdemon:
				case ActorSno._a4dun_garden_corruption_monster:
				case ActorSno._a4dun_garden_hellportal_pillar:
				case ActorSno._belialvoiceover:
					return;
			}

			if (Body.Hidden)
				return;

			if (CurrentAction != null && PriorityTarget != null && PriorityTarget.Attributes[GameAttribute.Is_Helper] == true)
			{
				PriorityTarget = null;
				CurrentAction.Cancel(tickCounter);
				CurrentAction = null;
				return;
			}

			if (tickCounter % 60 != 0) return;
			
			if (Body is NPC) return;

			if (!Body.Visible || Body.Dead) return;

			if (Body.World.Game.Paused) return;
			if (Body.Attributes[GameAttribute.Disabled]) return;

			if (Body.Attributes[GameAttribute.Frozen] ||
			Body.Attributes[GameAttribute.Stunned] ||
			Body.Attributes[GameAttribute.Blind] ||
			Body.Attributes[GameAttribute.Webbed] ||
			Body.Disable ||
			Body.World.BuffManager.GetFirstBuff<KnockbackBuff>(Body) != null ||
			Body.World.BuffManager.GetFirstBuff<SummonedBuff>(Body) != null)
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

				return;
			}

			Feared = false;

			if (CurrentAction == null)
			{
				_powerDelay ??= new SecondsTickTimer(Body.World.Game, 1.0f);
				if (AttackedBy != null || Body.GetObjectsInRange<Player>(50f).Count != 0)
				{
					if (_powerDelay.TimedOut)
					{
						_powerDelay = new SecondsTickTimer(Body.World.Game, 1.0f);

						if (AttackedBy != null)
							PriorityTarget = AttackedBy;

						if (PriorityTarget == null)
						{
							Actor[] targets;

							if (Body.Attributes[GameAttribute.Team_Override] == 1)
								targets = Body.GetObjectsInRange<Monster>(60f)
									.Where(p => !p.Dead)
									.OrderBy((monster) => PowerMath.Distance2D(monster.Position, Body.Position))
									.ToArray();
							else
								targets = Body.GetActorsInRange(50f)
									.Where(p => ((p is Player) && !p.Dead && p.Attributes[GameAttribute.Loading] == false && p.Attributes[GameAttribute.Is_Helper] == false && p.World.BuffManager.GetFirstBuff<ActorGhostedBuff>(p) == null)
										|| ((p is Minion) && !p.Dead && p.Attributes[GameAttribute.Is_Helper] == false)
										|| (p is DesctructibleLootContainer && p.SNO.IsDoorOrBarricade())
										|| ((p is Hireling) && !p.Dead)
										)
									.OrderBy((actor) => PowerMath.Distance2D(actor.Position, Body.Position))
									.ToArray();

							if (targets.Length == 0) return;
							
							_target = targets.First();
						}
						else
							_target = PriorityTarget;

						int powerToUse = PickPowerToUse();
						if (powerToUse > 0)
						{
							PowerScript power = PowerLoader.CreateImplementationForPowerSNO(powerToUse);
							power.User = Body;
							float attackRange = Body.ActorData.Cylinder.Ax2 + (power.EvalTag(PowerKeys.AttackRadius) > 0f ? (powerToUse == 30592 ? 10f : Math.Min((float)power.EvalTag(PowerKeys.AttackRadius), 35f)) : 35f);
							float targetDistance = PowerMath.Distance2D(_target.Position, Body.Position);
							if (targetDistance < attackRange + _target.ActorData.Cylinder.Ax2)
							{
								if (Body.WalkSpeed != 0)
									Body.TranslateFacing(_target.Position, false);

								CurrentAction = new PowerAction(Body, powerToUse, _target);

								if (power is SummoningSkill)
									PresetPowers[powerToUse] = new Cooldown { CooldownTimer = null, CooldownTime = (Body is Boss ? 15f : 7f) };

								if (power is MonsterAffixSkill monsterAffixSkill)
									PresetPowers[powerToUse] = new Cooldown { CooldownTimer = null, CooldownTime = monsterAffixSkill.CooldownTime };

								if (PresetPowers[powerToUse].CooldownTime > 0f)
									PresetPowers[powerToUse] = new Cooldown { CooldownTimer = new SecondsTickTimer(Body.World.Game, PresetPowers[powerToUse].CooldownTime), CooldownTime = PresetPowers[powerToUse].CooldownTime };

								if (powerToUse is 96925 or 223284)
									PresetPowers[powerToUse] = new Cooldown { CooldownTimer = new SecondsTickTimer(Body.World.Game, 10f), CooldownTime = 10f };
							}
							else if (Body.WalkSpeed != 0)
							{
								if (Body.SNO.IsWoodwraithOrWasp())
								{
									Logger.Trace($"{GetType().Name} {nameof(MoveToPointAction)} to target [{_target.Position}]");
									CurrentAction = new MoveToPointAction(
										Body, _target.Position
									);
								}
								else
								{
									Logger.Trace($"{GetType().Name} {nameof(MoveToTargetWithPathfindAction)} to target [{_target.ActorType}] {_target.SNO.ToString()}");
									CurrentAction = new MoveToTargetWithPathfindAction(
										Body,
										_target,
										attackRange + _target.ActorData.Cylinder.Ax2,
										powerToUse
									);
								}
							}
							else
							{
								powerToUse = Body.SNO switch
								{
									ActorSno._a1dun_leor_firewall2 => 223284,
									_ => powerToUse
								};
								CurrentAction = new PowerAction(Body, powerToUse, _target);

								if (power is SummoningSkill)
									PresetPowers[powerToUse] = new Cooldown { CooldownTimer = null, CooldownTime = (Body is Boss ? 15f : 7f) };

								if (power is MonsterAffixSkill)
									PresetPowers[powerToUse] = new Cooldown { CooldownTimer = null, CooldownTime = (power as MonsterAffixSkill).CooldownTime };

								if (PresetPowers[powerToUse].CooldownTime > 0f)
									PresetPowers[powerToUse] = new Cooldown { CooldownTimer = new SecondsTickTimer(Body.World.Game, PresetPowers[powerToUse].CooldownTime), CooldownTime = PresetPowers[powerToUse].CooldownTime };

								if (powerToUse == 96925 ||
								   powerToUse == 223284)
									PresetPowers[powerToUse] = new Cooldown { CooldownTimer = new SecondsTickTimer(Body.World.Game, 10f), CooldownTime = 10f };
							}
						}
					}
				}

				else if (Body.GetObjectsInRange<Living>(50f).Count != 0)
				{
					if (_powerDelay.TimedOut)
					{
						_powerDelay = new SecondsTickTimer(Body.World.Game, 1.0f);

						if (AttackedBy != null)
							PriorityTarget = AttackedBy;

						if (PriorityTarget == null)
						{
							var targets = Body.GetActorsInRange(50f)
							.Where(p => ((p is LorathNahr_NPC) && !p.Dead)
								|| ((p is CaptainRumford) && !p.Dead)
								|| (p is DesctructibleLootContainer && p.SNO.IsDoorOrBarricade())
								|| ((p is Cain) && !p.Dead))
							.OrderBy((actor) => PowerMath.Distance2D(actor.Position, Body.Position))
							.ToArray();

							if (targets.Length == 0)
							{
								targets = Body.GetActorsInRange(20f)
									.Where(p => ((p is Monster) && !p.Dead)
										|| ((p is CaptainRumford) && !p.Dead)
										)
									.OrderBy((actor) => PowerMath.Distance2D(actor.Position, Body.Position))
									.ToArray();


								if (targets.Length == 0)
									return;

								foreach (var monsterActor in targets.Where(tar => _target == null))
									if (monsterActor is Monster { Brain: MonsterBrain brain } monster && monsterActor != Body)
										if (brain.AttackedBy != null)
											_target = brain.AttackedBy;
							}
							else
							{
								_target = targets.First();
							}
							foreach (var tar in targets)
								if (tar is DesctructibleLootContainer && tar.SNO.IsDoorOrBarricade() && tar.SNO != ActorSno._trout_wagon_barricade)
								{ _target = tar; break; }
						}
						else
							_target = PriorityTarget;

						int powerToUse = PickPowerToUse();
						if (powerToUse > 0)
						{
							PowerScript power = PowerLoader.CreateImplementationForPowerSNO(powerToUse);
							power.User = Body;
							if (_target == null)
							{
								/*
								if (!this.Body.ActorSNO.Name.ToLower().Contains("woodwraith") && 
									!this.Body.ActorSNO.Name.ToLower().Contains("wasp"))
									if (this.Body.Quality < 2)
									{
										this.CurrentAction = new MoveToPointWithPathfindAction(this.Body, RandomPosibleDirection(this.Body.CheckPointPosition, 3f, 8f, this.Body.World));
										return;
									}
									else
										//*/
										return;
							}
							float attackRange = Body.ActorData.Cylinder.Ax2 + (power.EvalTag(PowerKeys.AttackRadius) > 0f ? (powerToUse == 30592 ? 10f : Math.Min((float)power.EvalTag(PowerKeys.AttackRadius), 35f)) : 35f);
							float targetDistance = PowerMath.Distance2D(_target.Position, Body.Position);
							if (targetDistance < attackRange + _target.ActorData.Cylinder.Ax2)
							{
								if (Body.WalkSpeed != 0)
									Body.TranslateFacing(_target.Position, false); //columns and other non-walkable shit can't turn

								
								Logger.Trace($"{GetType().Name} {nameof(PowerAction)} to target [{_target.ActorType}] {_target.SNO.ToString()}");
								// Logger.Trace("PowerAction to target");
								CurrentAction = new PowerAction(Body, powerToUse, _target);

								if (power is SummoningSkill)
									PresetPowers[powerToUse] = new Cooldown { CooldownTimer = null, CooldownTime = (Body is Boss ? 15f : 7f) };

								if (power is MonsterAffixSkill monsterSkill)
									PresetPowers[powerToUse] = new Cooldown { CooldownTimer = null, CooldownTime = monsterSkill.CooldownTime };

								if (PresetPowers[powerToUse].CooldownTime > 0f)
									PresetPowers[powerToUse] = new Cooldown { CooldownTimer = new SecondsTickTimer(Body.World.Game, PresetPowers[powerToUse].CooldownTime), CooldownTime = PresetPowers[powerToUse].CooldownTime };
							}
							else if (Body.WalkSpeed != 0)
							{
								if (Body.SNO.IsWoodwraithOrWasp())
								{									
									Logger.Trace($"{GetType().Name} {nameof(MoveToPointAction)} to target [{_target.Position}]");

									CurrentAction = new MoveToPointAction(
										Body, _target.Position
									);
								}
								else
								{
									Logger.Trace($"{GetType().Name} {nameof(MoveToTargetWithPathfindAction)} to target [{_target.ActorType}] {_target.SNO.ToString()}");

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
										attackRange + _target.ActorData.Cylinder.Ax2,
										powerToUse
									);
								}
							}
						}
					}						
				}

				else
				{
					//Logger.Trace("No enemies in range, return to master");
					if (Body.Position != Body.CheckPointPosition)
						CurrentAction = new MoveToPointWithPathfindAction(Body, Body.CheckPointPosition);
				}
			}
		}
		public static Core.Types.Math.Vector3D RandomPossibleDirection(Core.Types.Math.Vector3D position, float minRadius, float maxRadius, MapSystem.World world)
		{
			float angle = (float)(FastRandom.Instance.NextDouble() * Math.PI * 2);
			float radius = minRadius + (float)FastRandom.Instance.NextDouble() * (maxRadius - minRadius);
			Core.Types.Math.Vector3D point = null;
			int tryC = 0;
			while (tryC < 100)
			{
				//break;
				point = new Core.Types.Math.Vector3D(position.X + (float)Math.Cos(angle) * radius,
							  position.Y + (float)Math.Sin(angle) * radius,
							  position.Z);
				if (world.CheckLocationForFlag(point, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
					break;
				tryC++;
			}
			return point;
		}

		public void FastAttack(Actor target, int skillSNO)
		{
			PowerScript power = PowerLoader.CreateImplementationForPowerSNO(skillSNO);
			power.User = Body;
			if (Body.WalkSpeed != 0)
				Body.TranslateFacing(target.Position, false); //columns and other non-walkable shit can't turn
			Logger.Trace($"{GetType().Name} {nameof(FastAttack)} {nameof(PowerAction)} to target [{_target.ActorType}] {_target.SNO.ToString()}");
			CurrentAction = new PowerAction(Body, skillSNO, target);

			if (power is SummoningSkill)
				PresetPowers[skillSNO] = new Cooldown { CooldownTimer = null, CooldownTime = (Body is Boss ? 15f : 7f) };

			if (power is MonsterAffixSkill monsterAffixSkill)
				PresetPowers[skillSNO] = new Cooldown { CooldownTimer = null, CooldownTime = monsterAffixSkill.CooldownTime };

			if (PresetPowers[skillSNO].CooldownTime > 0f)
				PresetPowers[skillSNO] = new Cooldown { CooldownTimer = new SecondsTickTimer(Body.World.Game, PresetPowers[skillSNO].CooldownTime), CooldownTime = PresetPowers[skillSNO].CooldownTime };
		}

		protected virtual int PickPowerToUse()
		{
			if (!_warnedNoPowers && PresetPowers.Count == 0)
			{
				Logger.Warn($"Monster $[red]$\"{Body.Name}\"$[/]$ has no usable powers. {_mpqPowerCount} are defined in mpq data.");
				_warnedNoPowers = true;
				return -1;
			}

			// randomly used an implemented power
			if (PresetPowers.Count <= 0) return -1;
			
			//int power = this.PresetPowers[RandomHelper.Next(this.PresetPowers.Count)].Key;
			var availablePowers = PresetPowers.Where(p => (p.Value.CooldownTimer == null || p.Value.CooldownTimer.TimedOut) && PowerLoader.HasImplementationForPowerSNO(p.Key)).Select(p => p.Key).ToList();
			if (availablePowers.Count(p => p != 30592) > 0)
			{
				int SelectedPower = availablePowers.Where(p => p != 30592).ToList()[RandomHelper.Next(availablePowers.Where(p => p != 30592).ToList().Count())];
				//if(SelectedPower == 73824)
				//if(SkeletonKingWhirlwind)
				return SelectedPower;
			}

			if (availablePowers.Contains(30592))
				return 30592; //melee attack

			// no usable power
			return -1;
		}

		public void AddPresetPower(int powerSNO)
		{
			if (PresetPowers.ContainsKey(powerSNO))
			{
				Logger.Warn($"Monster $[red]$\"{Body.Name}\"$[/]$ already has power {powerSNO}.");
				// Logger.Debug("AddPresetPower(): power sno {0} already defined for monster \"{1}\"",
				//powerSNO, this.Body.ActorSNO.Name);
				return;
			}

			PresetPowers.Add(powerSNO,
				PresetPowers.ContainsKey(30592) //if can cast melee
					? new Cooldown { CooldownTimer = null, CooldownTime = 5f }
					: new Cooldown
						{ CooldownTimer = null, CooldownTime = 1f + (float)FastRandom.Instance.NextDouble() });
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
