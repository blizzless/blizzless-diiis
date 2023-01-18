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
			this.PresetPowers = new Dictionary<int, Cooldown>();

			// build list of powers defined in monster mpq data
			if (body.ActorData.MonsterSNO > 0)
			{
				var monsterData = (DiIiS_NA.Core.MPQ.FileFormats.Monster)MPQStorage.Data.Assets[SNOGroup.Monster][body.ActorData.MonsterSNO].Data;
				foreach (var monsterSkill in monsterData.SkillDeclarations)
				{
					if (monsterSkill.SNOPower > 0)
					{
						this.PresetPowers.Add(monsterSkill.SNOPower, new Cooldown { CooldownTimer = null, CooldownTime = 1f });
					}
				}
			}
		}

		public override void Think(int tickCounter)
		{
			// this needed? /mdz
			//if (this.Body is NPC) return;
			if ((this.Body as Minion).Master == null) return;

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
					_powerDelay = new SecondsTickTimer(this.Body.World.Game, (float)RandomHelper.NextDouble());

				if (_powerDelay.TimedOut)
				{
					List<Actor> targets = (this.Body as Minion).Master
						.GetObjectsInRange<Monster>(40f)
						.Where(m => !m.Dead && m.Visible && m.SNO.IsTargetable())
						.OrderBy(m => PowerMath.Distance2D(m.Position, this.Body.Position))
						.Cast<Actor>()
						.ToList();
					if (this.Body.World.Game.PvP)
						targets = (this.Body as Minion).Master.GetObjectsInRange<Player>(30f).Where(p => p.GlobalID != (this.Body as Minion).Master.GlobalID && p.Attributes[GameAttribute.TeamID] != (this.Body as Minion).Master.Attributes[GameAttribute.TeamID]).Cast<Actor>().ToList();
					if (this.Body.World.IsPvP)
						targets = (this.Body as Minion).Master.GetObjectsInRange<Player>(30f).Where(p => p.GlobalID != (this.Body as Minion).Master.GlobalID).Cast<Actor>().ToList();

					if (targets.Count != 0 && PowerMath.Distance2D(this.Body.Position, (this.Body as Minion).Master.Position) < 80f)
					{
						var elites = targets.Where(t => t is Champion || t is Rare || t is RareMinion);
						if (elites.Count() > 0)
							_target = elites.First();
						else
							_target = targets.First();

						int powerToUse = PickPowerToUse();
						if (powerToUse > 0)
						{
							PowerScript power = PowerLoader.CreateImplementationForPowerSNO(powerToUse);
							power.User = this.Body;
							float attackRange = this.Body.ActorData.Cylinder.Ax2 + (power.EvalTag(PowerKeys.AttackRadius) > 0f ? (powerToUse == 30592 ? 10f : power.EvalTag(PowerKeys.AttackRadius)) : 35f);
							float targetDistance = PowerMath.Distance2D(_target.Position, this.Body.Position);
							if (targetDistance < attackRange + _target.ActorData.Cylinder.Ax2)
							{
								if (this.Body.WalkSpeed != 0)
									this.Body.TranslateFacing(_target.Position, false); //columns and other non-walkable shit can't turn

								float cdReduction = (this.Body as Minion).CooldownReduction;

								//Logger.Trace("PowerAction to target");
								this.CurrentAction = new PowerAction(this.Body, powerToUse, _target);

								if (power is SummoningSkill)
									this.PresetPowers[powerToUse] = new Cooldown { CooldownTimer = null, CooldownTime = (7f * cdReduction) };

								if (this.PresetPowers[powerToUse].CooldownTime > 0f)
									this.PresetPowers[powerToUse] = new Cooldown { CooldownTimer = new SecondsTickTimer(this.Body.World.Game, this.PresetPowers[powerToUse].CooldownTime), CooldownTime = (this.PresetPowers[powerToUse].CooldownTime * cdReduction) };
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
									attackRange + _target.ActorData.Cylinder.Ax2
								);
							}
						}
					}
					else
					{
						//if ((this.Body as Minion).Master.World != (this.Body as Minion).World) 
						//this.Body.ChangeWorld((this.Body as Minion).Master.World, (this.Body as Minion).Master.Position);

						var distToMaster = PowerMath.Distance2D(this.Body.Position, (this.Body as Minion).Master.Position);
						if ((distToMaster > 8f) || (distToMaster < 3f))
						{
							var Rand = FastRandom.Instance;
							var position = (this.Body as Minion).Master.Position;
							float angle = (float)(Rand.NextDouble() * Math.PI * 2);
							float radius = 3f + (float)Rand.NextDouble() * (8f - 3f);
							var near = new Vector3D(position.X + (float)Math.Cos(angle) * radius, position.Y + (float)Math.Sin(angle) * radius, position.Z);
							this.CurrentAction = new MoveToPointAction(this.Body, near);
						}
					}
				}
			}
		}

		protected virtual int PickPowerToUse()
		{
			if (!_warnedNoPowers && this.PresetPowers.Count == 0)
			{
				Logger.Debug("Minion \"{0}\" has no usable powers. ", this.Body.Name);
				_warnedNoPowers = true;
			}

			// randomly used an implemented power
			if (this.PresetPowers.Count > 0)
			{
				//int power = this.PresetPowers[RandomHelper.Next(this.PresetPowers.Count)].Key;
				List<int> availablePowers = Enumerable.ToList(this.PresetPowers.Where(p => (p.Value.CooldownTimer == null || p.Value.CooldownTimer.TimedOut) && PowerSystem.PowerLoader.HasImplementationForPowerSNO(p.Key)).Select(p => p.Key));
				if (availablePowers.Where(p => p != 30592).Count() > 0)
					return availablePowers.Where(p => p != 30592).ToList()[RandomHelper.Next(availablePowers.Where(p => p != 30592).ToList().Count())];
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
				this.PresetPowers.Add(powerSNO, new Cooldown { CooldownTimer = null, CooldownTime = 1f + (float)FastRandom.Instance.NextDouble() });
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
