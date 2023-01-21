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
			this.PresetPowers = new Dictionary<int, Cooldown>();

			// build list of powers defined in monster mpq data
			if (body.ActorData.MonsterSNO > 0)
			{
				var monsterData = (DiIiS_NA.Core.MPQ.FileFormats.Monster)MPQStorage.Data.Assets[SNOGroup.Monster][body.ActorData.MonsterSNO].Data;
				_mpqPowerCount = monsterData.SkillDeclarations.Count(e => e.SNOPower != -1);
				for (int i = 0; i < monsterData.SkillDeclarations.Count(); i++)
				{
					if (monsterData.SkillDeclarations[i].SNOPower == -1) continue;
					if (PowerSystem.PowerLoader.HasImplementationForPowerSNO(monsterData.SkillDeclarations[i].SNOPower))
					{
						var cooldownTime = monsterData.MonsterSkillDeclarations[i].Timer / 10f;
						this.PresetPowers.Add(monsterData.SkillDeclarations[i].SNOPower, new Cooldown { CooldownTimer = null, CooldownTime = cooldownTime });
					}
				}

				if (!monsterData.SkillDeclarations.Any(s => s.SNOPower == 30592))
					this.PresetPowers.Add(30592, new Cooldown { CooldownTimer = null, CooldownTime = 0f }); //hack for dummy mobs without powers
			}
		}

		public override void Think(int tickCounter)
		{
			if (this.Body.SNO == ActorSno._uber_siegebreakerdemon ||
				this.Body.SNO == ActorSno._a4dun_garden_corruption_monster ||
				this.Body.SNO == ActorSno._a4dun_garden_hellportal_pillar)
				return;
			if (this.Body.SNO == ActorSno._belialvoiceover) //BelialVoiceover
				return;
			if (this.Body.Hidden == true)
				return;

			if (this.CurrentAction != null && this.PriorityTarget != null && this.PriorityTarget.Attributes[GameAttribute.Is_Helper] == true)
			{
				this.PriorityTarget = null;
				this.CurrentAction.Cancel(tickCounter);
				this.CurrentAction = null;
				return;
			}

			if (!(tickCounter % 60 == 0)) return;
			
			if (this.Body is NPC) return;

			if (!this.Body.Visible || this.Body.Dead) return;

			if (this.Body.World.Game.Paused) return;
			if (this.Body.Attributes[GameAttribute.Disabled]) return;

			if (this.Body.Attributes[GameAttribute.Frozen] ||
			this.Body.Attributes[GameAttribute.Stunned] ||
			this.Body.Attributes[GameAttribute.Blind] ||
			this.Body.Attributes[GameAttribute.Webbed] ||
			this.Body.Disable ||
			this.Body.World.BuffManager.GetFirstBuff<KnockbackBuff>(this.Body) != null ||
			this.Body.World.BuffManager.GetFirstBuff<SummonedBuff>(this.Body) != null)
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

			if (this.CurrentAction == null) 
			{

				if (_powerDelay == null)
					_powerDelay = new SecondsTickTimer(this.Body.World.Game, 1.0f);
				if (AttackedBy != null || this.Body.GetObjectsInRange<Player>(50f).Count != 0)
				{
					if (_powerDelay.TimedOut)
					{
						_powerDelay = new SecondsTickTimer(this.Body.World.Game, 1.0f);

						if (AttackedBy != null)
							this.PriorityTarget = AttackedBy;

						if (this.PriorityTarget == null)
						{
							List<Actor> targets = new List<Actor>();

							if (this.Body.Attributes[GameAttribute.Team_Override] == 1)
								targets = this.Body.GetObjectsInRange<Monster>(60f)
									.Where(p => !p.Dead)
									.OrderBy((monster) => PowerMath.Distance2D(monster.Position, this.Body.Position))
									.Cast<Actor>()
									.ToList();
							else
								targets = this.Body.GetActorsInRange(50f)
									.Where(p => ((p is Player) && !p.Dead && p.Attributes[GameAttribute.Loading] == false && p.Attributes[GameAttribute.Is_Helper] == false && p.World.BuffManager.GetFirstBuff<ActorGhostedBuff>(p) == null)
										|| ((p is Minion) && !p.Dead && p.Attributes[GameAttribute.Is_Helper] == false)
										|| (p is DesctructibleLootContainer && p.SNO.IsDoorOrBarricade())
										|| ((p is Hireling) && !p.Dead)
										)
									.OrderBy((actor) => PowerMath.Distance2D(actor.Position, this.Body.Position))
									.Cast<Actor>()
									.ToList();

							if (targets.Count == 0) return;
							
							_target = targets.First();
						}
						else
							_target = this.PriorityTarget;

						int powerToUse = PickPowerToUse();
						if (powerToUse > 0)
						{
							PowerScript power = PowerLoader.CreateImplementationForPowerSNO(powerToUse);
							power.User = this.Body;
							float attackRange = this.Body.ActorData.Cylinder.Ax2 + (power.EvalTag(PowerKeys.AttackRadius) > 0f ? (powerToUse == 30592 ? 10f : Math.Min((float)power.EvalTag(PowerKeys.AttackRadius), 35f)) : 35f);
							float targetDistance = PowerMath.Distance2D(_target.Position, this.Body.Position);
							if (targetDistance < attackRange + _target.ActorData.Cylinder.Ax2)
							{
								if (this.Body.WalkSpeed != 0)
									this.Body.TranslateFacing(_target.Position, false);

								this.CurrentAction = new PowerAction(this.Body, powerToUse, _target);

								if (power is SummoningSkill)
									this.PresetPowers[powerToUse] = new Cooldown { CooldownTimer = null, CooldownTime = (this.Body is Boss ? 15f : 7f) };

								if (power is MonsterAffixSkill)
									this.PresetPowers[powerToUse] = new Cooldown { CooldownTimer = null, CooldownTime = (power as MonsterAffixSkill).CooldownTime };

								if (this.PresetPowers[powerToUse].CooldownTime > 0f)
									this.PresetPowers[powerToUse] = new Cooldown { CooldownTimer = new SecondsTickTimer(this.Body.World.Game, this.PresetPowers[powerToUse].CooldownTime), CooldownTime = this.PresetPowers[powerToUse].CooldownTime };

								if (powerToUse == 96925 ||
								   powerToUse == 223284)
									this.PresetPowers[powerToUse] = new Cooldown { CooldownTimer = new SecondsTickTimer(this.Body.World.Game, 10f), CooldownTime = 10f };
							}
							else if (this.Body.WalkSpeed != 0)
							{
								if (this.Body.SNO.IsWoodwraithOrWasp())
								{
									Logger.Trace("MoveToPointAction to target");
									this.CurrentAction = new MoveToPointAction(
										this.Body, _target.Position
									);
								}
								else
								{
									Logger.Trace("MoveToTargetWithPathfindAction to target");
									this.CurrentAction = new MoveToTargetWithPathfindAction(
										this.Body,
										_target,
										attackRange + _target.ActorData.Cylinder.Ax2,
										powerToUse
									);
								}
							}
							else
							{
								switch (this.Body.SNO)
								{
									case ActorSno._a1dun_leor_firewall2:
										powerToUse = 223284;
										break;
								}
								this.CurrentAction = new PowerAction(this.Body, powerToUse, _target);

								if (power is SummoningSkill)
									this.PresetPowers[powerToUse] = new Cooldown { CooldownTimer = null, CooldownTime = (this.Body is Boss ? 15f : 7f) };

								if (power is MonsterAffixSkill)
									this.PresetPowers[powerToUse] = new Cooldown { CooldownTimer = null, CooldownTime = (power as MonsterAffixSkill).CooldownTime };

								if (this.PresetPowers[powerToUse].CooldownTime > 0f)
									this.PresetPowers[powerToUse] = new Cooldown { CooldownTimer = new SecondsTickTimer(this.Body.World.Game, this.PresetPowers[powerToUse].CooldownTime), CooldownTime = this.PresetPowers[powerToUse].CooldownTime };

								if (powerToUse == 96925 ||
								   powerToUse == 223284)
									this.PresetPowers[powerToUse] = new Cooldown { CooldownTimer = new SecondsTickTimer(this.Body.World.Game, 10f), CooldownTime = 10f };
							}
						}
					}
				}

				else if (this.Body.GetObjectsInRange<Living>(50f).Count != 0)
				{
					if (_powerDelay.TimedOut)
					{
						_powerDelay = new SecondsTickTimer(this.Body.World.Game, 1.0f);

						if (AttackedBy != null)
							this.PriorityTarget = AttackedBy;

						if (this.PriorityTarget == null)
						{
							List<Actor> targets = new List<Actor>();

							targets = this.Body.GetActorsInRange(50f)
							.Where(p => ((p is LorathNahr_NPC) && !p.Dead)
								|| ((p is CaptainRumford) && !p.Dead)
								|| (p is DesctructibleLootContainer && p.SNO.IsDoorOrBarricade())
								|| ((p is Cain) && !p.Dead))
							.OrderBy((actor) => PowerMath.Distance2D(actor.Position, this.Body.Position))
							.Cast<Actor>()
							.ToList();

							if (targets.Count == 0)
							{
								targets = this.Body.GetActorsInRange(20f)
									.Where(p => ((p is Monster) && !p.Dead)
										|| ((p is CaptainRumford) && !p.Dead)
										)
									.OrderBy((actor) => PowerMath.Distance2D(actor.Position, this.Body.Position))
									.Cast<Actor>()
									.ToList();


								if (targets.Count == 0)
									return;

								foreach (var tar in targets)
									if (_target == null)
										if (tar is Monster && tar != this.Body)
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
							_target = this.PriorityTarget;

						int powerToUse = PickPowerToUse();
						if (powerToUse > 0)
						{
							PowerScript power = PowerLoader.CreateImplementationForPowerSNO(powerToUse);
							power.User = this.Body;
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
							float attackRange = this.Body.ActorData.Cylinder.Ax2 + (power.EvalTag(PowerKeys.AttackRadius) > 0f ? (powerToUse == 30592 ? 10f : Math.Min((float)power.EvalTag(PowerKeys.AttackRadius), 35f)) : 35f);
							float targetDistance = PowerMath.Distance2D(_target.Position, this.Body.Position);
							if (targetDistance < attackRange + _target.ActorData.Cylinder.Ax2)
							{
								if (this.Body.WalkSpeed != 0)
									this.Body.TranslateFacing(_target.Position, false); //columns and other non-walkable shit can't turn

								//Logger.Trace("PowerAction to target");
								this.CurrentAction = new PowerAction(this.Body, powerToUse, _target);

								if (power is SummoningSkill)
									this.PresetPowers[powerToUse] = new Cooldown { CooldownTimer = null, CooldownTime = (this.Body is Boss ? 15f : 7f) };

								if (power is MonsterAffixSkill)
									this.PresetPowers[powerToUse] = new Cooldown { CooldownTimer = null, CooldownTime = (power as MonsterAffixSkill).CooldownTime };

								if (this.PresetPowers[powerToUse].CooldownTime > 0f)
									this.PresetPowers[powerToUse] = new Cooldown { CooldownTimer = new SecondsTickTimer(this.Body.World.Game, this.PresetPowers[powerToUse].CooldownTime), CooldownTime = this.PresetPowers[powerToUse].CooldownTime };
							}
							else if (this.Body.WalkSpeed != 0)
							{
								if (this.Body.SNO.IsWoodwraithOrWasp())
								{
									Logger.Trace("MoveToPointAction to target");
									this.CurrentAction = new MoveToPointAction(
										this.Body, _target.Position
									);
								}
								else
								{
									Logger.Trace("MoveToTargetWithPathfindAction to target");
									this.CurrentAction = new MoveToTargetWithPathfindAction(
										this.Body,
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
					if (this.Body.Position != this.Body.CheckPointPosition)
						this.CurrentAction = new MoveToPointWithPathfindAction(this.Body, this.Body.CheckPointPosition);
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
			power.User = this.Body;
			if (this.Body.WalkSpeed != 0)
				this.Body.TranslateFacing(target.Position, false); //columns and other non-walkable shit can't turn

			//Logger.Trace("Fast PowerAction to target");
			this.CurrentAction = new PowerAction(this.Body, skillSNO, target);

			if (power is SummoningSkill)
				this.PresetPowers[skillSNO] = new Cooldown { CooldownTimer = null, CooldownTime = (this.Body is Boss ? 15f : 7f) };

			if (power is MonsterAffixSkill)
				this.PresetPowers[skillSNO] = new Cooldown { CooldownTimer = null, CooldownTime = (power as MonsterAffixSkill).CooldownTime };

			if (this.PresetPowers[skillSNO].CooldownTime > 0f)
				this.PresetPowers[skillSNO] = new Cooldown { CooldownTimer = new SecondsTickTimer(this.Body.World.Game, this.PresetPowers[skillSNO].CooldownTime), CooldownTime = this.PresetPowers[skillSNO].CooldownTime };
		}

		protected virtual int PickPowerToUse()
		{
			if (!_warnedNoPowers && this.PresetPowers.Count == 0)
			{
				Logger.Info("Monster \"{0}\" has no usable powers. {1} are defined in mpq data.", this.Body.Name, _mpqPowerCount);
				_warnedNoPowers = true;
			}

			// randomly used an implemented power
			if (this.PresetPowers.Count > 0)
			{
				//int power = this.PresetPowers[RandomHelper.Next(this.PresetPowers.Count)].Key;
				List<int> availablePowers = Enumerable.ToList(this.PresetPowers.Where(p => (p.Value.CooldownTimer == null || p.Value.CooldownTimer.TimedOut) && PowerSystem.PowerLoader.HasImplementationForPowerSNO(p.Key)).Select(p => p.Key));
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
			if (this.PresetPowers.ContainsKey(powerSNO))
			{
				// Logger.Debug("AddPresetPower(): power sno {0} already defined for monster \"{1}\"",
				//powerSNO, this.Body.ActorSNO.Name);
				return;
			}
			if (this.PresetPowers.ContainsKey(30592)) //if can cast melee
				this.PresetPowers.Add(powerSNO, new Cooldown { CooldownTimer = null, CooldownTime = 5f });
			else
				this.PresetPowers.Add(powerSNO, new Cooldown { CooldownTimer = null, CooldownTime = 1f + (float)DiIiS_NA.Core.Helpers.Math.FastRandom.Instance.NextDouble() });
		}

		public void RemovePresetPower(int powerSNO)
		{
			if (this.PresetPowers.ContainsKey(powerSNO))
			{
				this.PresetPowers.Remove(powerSNO);
			}
		}
	}
}
