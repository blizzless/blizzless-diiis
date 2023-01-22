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
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
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
	public class MonsterBrain : Brain
	{
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
			PresetPowers = new Dictionary<int, Cooldown>();

			// build list of powers defined in monster mpq data
			if (body.ActorData.MonsterSNO > 0)
			{
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

				if (!monsterData.SkillDeclarations.Any(s => s.SNOPower == 30592))
					PresetPowers.Add(30592, new Cooldown { CooldownTimer = null, CooldownTime = 0f }); //hack for dummy mobs without powers
			}
		}

		public override void Think(int tickCounter)
		{
			if (Body.SNO == ActorSno._uber_siegebreakerdemon ||
				Body.SNO == ActorSno._a4dun_garden_corruption_monster ||
				Body.SNO == ActorSno._a4dun_garden_hellportal_pillar)
				return;
			if (Body.SNO == ActorSno._belialvoiceover) //BelialVoiceover
				return;
			if (Body.Hidden == true)
				return;

			if (CurrentAction != null && PriorityTarget != null && PriorityTarget.Attributes[GameAttribute.Is_Helper] == true)
			{
				PriorityTarget = null;
				CurrentAction.Cancel(tickCounter);
				CurrentAction = null;
				return;
			}

			if (!(tickCounter % 60 == 0)) return;
			
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
				else return;
			}
			else
				Feared = false;

			if (CurrentAction == null) 
			{

				if (_powerDelay == null)
					_powerDelay = new SecondsTickTimer(Body.World.Game, 1.0f);
				if (AttackedBy != null || Body.GetObjectsInRange<Player>(50f).Count != 0)
				{
					if (_powerDelay.TimedOut)
					{
						_powerDelay = new SecondsTickTimer(Body.World.Game, 1.0f);

						if (AttackedBy != null)
							PriorityTarget = AttackedBy;

						if (PriorityTarget == null)
						{
							List<Actor> targets = new List<Actor>();

							if (Body.Attributes[GameAttribute.Team_Override] == 1)
								targets = Body.GetObjectsInRange<Monster>(60f)
									.Where(p => !p.Dead)
									.OrderBy((monster) => PowerMath.Distance2D(monster.Position, Body.Position))
									.Cast<Actor>()
									.ToList();
							else
								targets = Body.GetActorsInRange(50f)
									.Where(p => ((p is Player) && !p.Dead && p.Attributes[GameAttribute.Loading] == false && p.Attributes[GameAttribute.Is_Helper] == false && p.World.BuffManager.GetFirstBuff<ActorGhostedBuff>(p) == null)
										|| ((p is Minion) && !p.Dead && p.Attributes[GameAttribute.Is_Helper] == false)
										|| (p is DesctructibleLootContainer && p.SNO.IsDoorOrBarricade())
										|| ((p is Hireling) && !p.Dead)
										)
									.OrderBy((actor) => PowerMath.Distance2D(actor.Position, Body.Position))
									.Cast<Actor>()
									.ToList();

							if (targets.Count == 0) return;
							
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

								if (power is MonsterAffixSkill)
									PresetPowers[powerToUse] = new Cooldown { CooldownTimer = null, CooldownTime = (power as MonsterAffixSkill).CooldownTime };

								if (PresetPowers[powerToUse].CooldownTime > 0f)
									PresetPowers[powerToUse] = new Cooldown { CooldownTimer = new SecondsTickTimer(Body.World.Game, PresetPowers[powerToUse].CooldownTime), CooldownTime = PresetPowers[powerToUse].CooldownTime };

								if (powerToUse == 96925 ||
								   powerToUse == 223284)
									PresetPowers[powerToUse] = new Cooldown { CooldownTimer = new SecondsTickTimer(Body.World.Game, 10f), CooldownTime = 10f };
							}
							else if (Body.WalkSpeed != 0)
							{
								if (Body.SNO.IsWoodwraithOrWasp())
								{
									Logger.Trace("MoveToPointAction to target");
									CurrentAction = new MoveToPointAction(
										Body, _target.Position
									);
								}
								else
								{
									Logger.Trace("MoveToTargetWithPathfindAction to target");
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
								switch (Body.SNO)
								{
									case ActorSno._a1dun_leor_firewall2:
										powerToUse = 223284;
										break;
								}
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
							List<Actor> targets = new List<Actor>();

							targets = Body.GetActorsInRange(50f)
							.Where(p => ((p is LorathNahr_NPC) && !p.Dead)
								|| ((p is CaptainRumford) && !p.Dead)
								|| (p is DesctructibleLootContainer && p.SNO.IsDoorOrBarricade())
								|| ((p is Cain) && !p.Dead))
							.OrderBy((actor) => PowerMath.Distance2D(actor.Position, Body.Position))
							.Cast<Actor>()
							.ToList();

							if (targets.Count == 0)
							{
								targets = Body.GetActorsInRange(20f)
									.Where(p => ((p is Monster) && !p.Dead)
										|| ((p is CaptainRumford) && !p.Dead)
										)
									.OrderBy((actor) => PowerMath.Distance2D(actor.Position, Body.Position))
									.Cast<Actor>()
									.ToList();


								if (targets.Count == 0)
									return;

								foreach (var tar in targets)
									if (_target == null)
										if (tar is Monster && tar != Body)
											if (((tar as Monster).Brain as MonsterBrain).AttackedBy != null)
												_target = ((tar as Monster).Brain as MonsterBrain).AttackedBy;
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

								//Logger.Trace("PowerAction to target");
								CurrentAction = new PowerAction(Body, powerToUse, _target);

								if (power is SummoningSkill)
									PresetPowers[powerToUse] = new Cooldown { CooldownTimer = null, CooldownTime = (Body is Boss ? 15f : 7f) };

								if (power is MonsterAffixSkill)
									PresetPowers[powerToUse] = new Cooldown { CooldownTimer = null, CooldownTime = (power as MonsterAffixSkill).CooldownTime };

								if (PresetPowers[powerToUse].CooldownTime > 0f)
									PresetPowers[powerToUse] = new Cooldown { CooldownTimer = new SecondsTickTimer(Body.World.Game, PresetPowers[powerToUse].CooldownTime), CooldownTime = PresetPowers[powerToUse].CooldownTime };
							}
							else if (Body.WalkSpeed != 0)
							{
								if (Body.SNO.IsWoodwraithOrWasp())
								{
									Logger.Trace("MoveToPointAction to target");
									CurrentAction = new MoveToPointAction(
										Body, _target.Position
									);
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
		public static Core.Types.Math.Vector3D RandomPosibleDirection(Core.Types.Math.Vector3D position, float minRadius, float maxRadius, MapSystem.World wrld)
		{
			float angle = (float)(FastRandom.Instance.NextDouble() * Math.PI * 2);
			float radius = minRadius + (float)FastRandom.Instance.NextDouble() * (maxRadius - minRadius);
			Core.Types.Math.Vector3D SP = null;
			int tryC = 0;
			while (tryC < 50)
			{
				//break;
				SP = new Core.Types.Math.Vector3D(position.X + (float)Math.Cos(angle) * radius,
							  position.Y + (float)Math.Sin(angle) * radius,
							  position.Z);
				if (wrld.CheckLocationForFlag(SP, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
					break;
				tryC++;
			}
			return SP;
		}

		public void FastAttack(Actor target, int skillSNO)
		{
			PowerScript power = PowerLoader.CreateImplementationForPowerSNO(skillSNO);
			power.User = Body;
			if (Body.WalkSpeed != 0)
				Body.TranslateFacing(target.Position, false); //columns and other non-walkable shit can't turn

			//Logger.Trace("Fast PowerAction to target");
			CurrentAction = new PowerAction(Body, skillSNO, target);

			if (power is SummoningSkill)
				PresetPowers[skillSNO] = new Cooldown { CooldownTimer = null, CooldownTime = (Body is Boss ? 15f : 7f) };

			if (power is MonsterAffixSkill)
				PresetPowers[skillSNO] = new Cooldown { CooldownTimer = null, CooldownTime = (power as MonsterAffixSkill).CooldownTime };

			if (PresetPowers[skillSNO].CooldownTime > 0f)
				PresetPowers[skillSNO] = new Cooldown { CooldownTimer = new SecondsTickTimer(Body.World.Game, PresetPowers[skillSNO].CooldownTime), CooldownTime = PresetPowers[skillSNO].CooldownTime };
		}

		protected virtual int PickPowerToUse()
		{
			if (!_warnedNoPowers && PresetPowers.Count == 0)
			{
				Logger.Info("Monster \"{0}\" has no usable powers. {1} are defined in mpq data.", Body.Name, _mpqPowerCount);
				_warnedNoPowers = true;
			}

			// randomly used an implemented power
			if (PresetPowers.Count > 0)
			{
				//int power = this.PresetPowers[RandomHelper.Next(this.PresetPowers.Count)].Key;
				List<int> availablePowers = Enumerable.ToList(PresetPowers.Where(p => (p.Value.CooldownTimer == null || p.Value.CooldownTimer.TimedOut) && PowerLoader.HasImplementationForPowerSNO(p.Key)).Select(p => p.Key));
				if (availablePowers.Where(p => p != 30592).Count() > 0)
				{
					int SelectedPower = availablePowers.Where(p => p != 30592).ToList()[RandomHelper.Next(availablePowers.Where(p => p != 30592).ToList().Count())];
					//if(SelectedPower == 73824)
						//if(SkeletonKingWhirlwind)
					return SelectedPower;
				}
				else
					if (availablePowers.Contains(30592))
					return 30592; //melee attack
			}

			// no usable power
			return -1;
		}

		public void AddPresetPower(int powerSNO)
		{
			if (PresetPowers.ContainsKey(powerSNO))
			{
				// Logger.Debug("AddPresetPower(): power sno {0} already defined for monster \"{1}\"",
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
