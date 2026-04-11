using DiIiS_NA.Core.MPQ;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Actions;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.Core.Extensions;

namespace DiIiS_NA.GameServer.GSSystem.AISystem.Brains
{
	/// <summary>
	/// AI controller for friendly NPCs that should actively engage enemies
	/// — for example town guards during scripted battles, or allied units
	/// summoned during quest events.
	///
	/// <para>Behaviour:</para>
	/// <list type="bullet">
	///   <item><description>Attacks any monster within 40 tiles.</description></item>
	///   <item><description>Pathfinds to engage if out of attack range.</description></item>
	///   <item><description>Walks back to <see cref="Actor.CheckPointPosition"/>
	///     (spawn point) when no enemies are visible.</description></item>
	/// </list>
	///
	/// <para>Use <see cref="StayAggressiveNPCBrain"/> for stationary
	/// archers / turrets that should never leave their spawn.</para>
	/// </summary>
	public class AggressiveNPCBrain : Brain
	{
		/// <summary>All power SNOs this NPC may cast, loaded from MPQ monster data.</summary>
		public List<int> PresetPowers { get; private set; }

		/// <summary>Current combat target.</summary>
		private Actor _target { get; set; }

		/// <summary>Global 1-second cadence between attack attempts.</summary>
		private TickTimer _powerDelay;

		/// <summary>
		/// Creates the brain and loads its skill list from the body's
		/// underlying monster MPQ definition.
		/// </summary>
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

			Logger.Trace("AggressiveNPCBrain spawned: {0} with {1} power(s)",
				body?.SNO.ToString() ?? "<null>", PresetPowers.Count);
		}

		/// <summary>
		/// Main AI tick. CC-gated identically to the monster / minion brains;
		/// when idle, sweeps for the nearest monster and either engages or
		/// walks back to its spawn.
		/// </summary>
		public override void Think(int tickCounter)
		{
			// CC gate — stop and reset the cadence timer if disabled.
			if (Body.ShouldStopTickAction ||
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
				// 1-second global cadence between attacks.
				if (_powerDelay == null)
					_powerDelay = new SecondsTickTimer(Body.World.Game, 1f);

				if (_powerDelay.TimedOut)
				{
					// Sweep monsters within 40f (default) tiles of the body.
					var monsters = Body.GetObjectsInRange<Monster>(GameServerConfig.Instance.SweepMonstersTiles).Where(m => m.Visible & !m.Dead).ToList();
					if (monsters.Count != 0)
					{
						// Take the first monster returned (first in natural
						// iteration order, not distance-sorted — kept as-is
						// to preserve the original behaviour).
						_target = monsters[0];
						int powerToUse = PickPowerToUse();
						if (powerToUse > 0)
						{
							PowerSystem.PowerScript power = PowerSystem.PowerLoader.CreateImplementationForPowerSNO(powerToUse);
							power.User = Body;

							// Same range convention as every other brain.
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
								// Out of range → pathfind in.
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
						// No monsters in range → walk back to spawn so the
						// NPC doesn't drift away from its post.
						CurrentAction = new MoveToPointAction(Body, Body.CheckPointPosition);
					}
				}
			}
		}

		/// <summary>Picks a random implemented power from the preset list.</summary>
		protected virtual int PickPowerToUse()
		{
			var implementedPowers = PresetPowers.Where(PowerSystem.PowerLoader.HasImplementationForPowerSNO);
			return implementedPowers.TryPickRandom(out var randomPower)
				? randomPower
				: -1;
		}

		/// <summary>Adds a power to the NPC's usable set at runtime.</summary>
		public void AddPresetPower(int powerSNO)
		{
			PresetPowers.Add(powerSNO);
		}
	}
}
