using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.Core.Extensions;
using DiIiS_NA.Core.MPQ;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Actions;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
using DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.AISystem.Brains
{
	/// <summary>
	/// "Stay-in-place" variant of <see cref="AggressiveNPCBrain"/>: the NPC
	/// attacks enemies that wander into range but never moves out of its
	/// spawn point to chase them. Used for stationary shooters — siege
	/// towers, turret-style NPCs, archers on castle walls, etc.
	///
	/// <para>Key differences vs. <see cref="AggressiveNPCBrain"/>:</para>
	/// <list type="bullet">
	///   <item><description>Out-of-range targets do NOT cause a
	///     <c>MoveToTargetWithPathfindAction</c> (the "pathfind in" branch
	///     is an empty else).</description></item>
	///   <item><description>When no targets are in range, it still walks
	///     back to <see cref="Actor.CheckPointPosition"/> if it has drifted
	///     off-post.</description></item>
	/// </list>
	/// </summary>
	public class StayAggressiveNPCBrain : Brain
	{
		/// <summary>Power SNOs loaded from MPQ monster data.</summary>
		public List<int> PresetPowers { get; private set; }

		/// <summary>Current combat target.</summary>
		private Actor _target { get; set; }

		/// <summary>Global 1-second cadence between attack attempts.</summary>
		private TickTimer _powerDelay;

		/// <summary>
		/// Creates the brain and loads its skill list from the body's
		/// underlying monster MPQ definition.
		/// </summary>
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

			Logger.Trace("StayAggressiveNPCBrain spawned: {0} with {1} power(s)",
				body?.SNO.ToString() ?? "<null>", PresetPowers.Count);
		}

		/// <summary>
		/// Main AI tick. Attacks enemies in range; out-of-range targets are
		/// ignored (no chase). CC gating matches every other brain.
		/// </summary>
		public override void Think(int tickCounter)
		{
			// this needed? /mdz
			//if (this.Body is NPC) return;

			// CC gate — cancel any running action and bail.
			if (Body.Attributes[GameAttributes.Frozen] ||
				Body.Attributes[GameAttributes.Stunned] ||
				Body.Attributes[GameAttributes.Blind] ||
				Body.Attributes[GameAttributes.Webbed] ||
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

			// Select and start executing a power if no active action.
			if (CurrentAction == null)
			{
				// One-second cadence so packs don't cast in unison.
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

							// Same range convention as every other brain.
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
								// Intentionally empty: stationary NPCs
								// never chase targets out of range.
							}
						}
					}
					else
					{
						// No targets visible → walk back to spawn so a
						// bumped/shoved NPC returns to its post.
						CurrentAction = new MoveToPointAction(Body, Body.CheckPointPosition);
					}
				}
			}
		}

		/// <summary>Picks a random implemented power from the preset list.</summary>
		protected virtual int PickPowerToUse()
		{
			// randomly used an implemented power
			var implementedPowers = PresetPowers.Where(PowerLoader.HasImplementationForPowerSNO);
			return implementedPowers.TryPickRandom(out var randomPower)
				? randomPower
				: -1;
		}

		/// <summary>Adds a power to this NPC's usable set at runtime.</summary>
		public void AddPresetPower(int powerSNO)
		{
			PresetPowers.Add(powerSNO);
		}
	}
}
