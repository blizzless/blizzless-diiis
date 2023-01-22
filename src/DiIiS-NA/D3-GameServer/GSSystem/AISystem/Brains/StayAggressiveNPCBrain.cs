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
			PresetPowers = new List<int>();


			if (body.ActorData.MonsterSNO > 0)
			{
				var monsterData = (DiIiS_NA.Core.MPQ.FileFormats.Monster)MPQStorage.Data.Assets[SNOGroup.Monster][body.ActorData.MonsterSNO].Data;
				foreach (var monsterSkill in monsterData.SkillDeclarations)
				{
					if (monsterSkill.SNOPower > 0)
					{
						PresetPowers.Add(monsterSkill.SNOPower);
					}
				}
			}
		}

		public override void Think(int tickCounter)
		{
			// this needed? /mdz
			//if (this.Body is NPC) return;

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

			// select and start executing a power if no active action
			if (CurrentAction == null)
			{
				// do a little delay so groups of monsters don't all execute at once
				if (_powerDelay == null)
					_powerDelay = new SecondsTickTimer(Body.World.Game, 1f);

				if (_powerDelay.TimedOut)
				{
					var monsters = Body.GetObjectsInRange<Monster>(40f).Where(m => m.Visible & !m.Dead).ToList();
					if (monsters.Count != 0)
					{
						_target = monsters[0];
						//System.Console.Out.WriteLine("Enemy in range, use powers");
						//This will only attack when you and your minions are not moving..TODO: FIX.
						int powerToUse = PickPowerToUse();
						if (powerToUse > 0)
						{
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

							}
						}
					}
					else
					{
						CurrentAction = new MoveToPointAction(Body, Body.CheckPointPosition);
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
