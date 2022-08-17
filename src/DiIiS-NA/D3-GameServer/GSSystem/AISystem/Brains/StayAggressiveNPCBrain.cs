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
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Actions;
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
	public class StayAggressiveNPCBrain : Brain
	{
		// list of power SNOs that are defined for the monster
		public List<int> PresetPowers { get; private set; }
		private Actor _target { get; set; }
		private TickTimer _powerDelay;

		public StayAggressiveNPCBrain(Actor body)
			: base(body)
		{
			this.PresetPowers = new List<int>();


			if (body.ActorData.MonsterSNO > 0)
			{
				var monsterData = (DiIiS_NA.Core.MPQ.FileFormats.Monster)MPQStorage.Data.Assets[SNOGroup.Monster][body.ActorData.MonsterSNO].Data;
				foreach (var monsterSkill in monsterData.SkillDeclarations)
				{
					if (monsterSkill.SNOPower > 0)
					{
						this.PresetPowers.Add(monsterSkill.SNOPower);
					}
				}
			}
		}

		public override void Think(int tickCounter)
		{
			// this needed? /mdz
			//if (this.Body is NPC) return;

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

			// select and start executing a power if no active action
			if (this.CurrentAction == null)
			{
				// do a little delay so groups of monsters don't all execute at once
				if (_powerDelay == null)
					_powerDelay = new SecondsTickTimer(this.Body.World.Game, 1f);

				if (_powerDelay.TimedOut)
				{
					var monsters = this.Body.GetObjectsInRange<Monster>(40f).Where(m => m.Visible & !m.Dead).ToList();
					if (monsters.Count != 0)
					{
						_target = monsters[0];
						//System.Console.Out.WriteLine("Enemy in range, use powers");
						//This will only attack when you and your minions are not moving..TODO: FIX.
						int powerToUse = PickPowerToUse();
						if (powerToUse > 0)
						{
							PowerSystem.PowerScript power = PowerSystem.PowerLoader.CreateImplementationForPowerSNO(powerToUse);
							power.User = this.Body;
							float attackRange = this.Body.ActorData.Cylinder.Ax2 + (power.EvalTag(PowerKeys.AttackRadius) > 0f ? (powerToUse == 30592 ? 10f : power.EvalTag(PowerKeys.AttackRadius)) : 35f);
							float targetDistance = PowerSystem.PowerMath.Distance2D(_target.Position, this.Body.Position);
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

							}
						}
					}
					else
					{
						this.CurrentAction = new MoveToPointAction(this.Body, this.Body.CheckPointPosition);
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
