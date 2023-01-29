using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.Core.MPQ;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Actions;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiIiS_NA.Core.Extensions;

namespace DiIiS_NA.GameServer.GSSystem.AISystem.Brains
{
	public class AggressiveNPCBrain : Brain
	{
		public List<int> PresetPowers { get; private set; }
		private Actor _target { get; set; }
		private TickTimer _powerDelay;

		public AggressiveNPCBrain(Actor body)
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
			if (Body.Attributes[GameAttribute.Frozen] ||
				Body.Attributes[GameAttribute.Stunned] ||
				Body.Attributes[GameAttribute.Blind] ||
				Body.Attributes[GameAttribute.Webbed] ||
				Body.Disable ||
				Body.World.BuffManager.GetFirstBuff<PowerSystem.Implementations.KnockbackBuff>(Body) != null)
			{
				if (CurrentAction != null)
				{
					CurrentAction.Cancel(tickCounter);
					CurrentAction = null;
				}
				_powerDelay = null;

				return;
			}

			if (CurrentAction == null)
			{
				if (_powerDelay == null)
					_powerDelay = new SecondsTickTimer(Body.World.Game, 1f);

				if (_powerDelay.TimedOut)
				{
					var monsters = Body.GetObjectsInRange<Monster>(40f).Where(m => m.Visible & !m.Dead).ToList();
					if (monsters.Count != 0)
					{
						_target = monsters[0];
						int powerToUse = PickPowerToUse();
						if (powerToUse > 0) // FIXME: probably >= 0 as 0 can be a valid power?
						{
							PowerSystem.PowerScript power = PowerSystem.PowerLoader.CreateImplementationForPowerSNO(powerToUse);
							power.User = Body;
							float attackRange = Body.ActorData.Cylinder.Ax2 + (power.EvalTag(PowerKeys.AttackRadius) > 0f ? (powerToUse == 30592 ? 10f : power.EvalTag(PowerKeys.AttackRadius)) : 35f);
							float targetDistance = PowerSystem.PowerMath.Distance2D(_target.Position, Body.Position);
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
						CurrentAction = new MoveToPointAction(Body, Body.CheckPointPosition);
					}
				}
			}
		}

		protected virtual int PickPowerToUse()
		{
			if (PresetPowers.Count > 0)
			{
				var randomPower = PresetPowers.PickRandom();
				
				// should we try several times or pick random from implemented only powers?
				if (PowerSystem.PowerLoader.HasImplementationForPowerSNO(randomPower))
					return randomPower;
			}

			return -1;
		}

		public void AddPresetPower(int powerSNO)
		{
			PresetPowers.Add(powerSNO);
		}
	}
}
