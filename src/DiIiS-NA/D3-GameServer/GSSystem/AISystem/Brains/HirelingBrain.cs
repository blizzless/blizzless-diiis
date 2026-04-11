using System;
using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.Core.Extensions;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.GameServer.Core.Types.Math;
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
	/// <summary>
	/// AI controller for the single-player follower NPCs: Templar,
	/// Scoundrel, Enchantress, Leah (Diablo 3's hireling companions).
	///
	/// <para>Unlike <see cref="MinionBrain"/> this brain does not load
	/// powers from MPQ data. Instead it owns exactly one hard-coded
	/// attack power, chosen in the constructor based on the concrete
	/// hireling subclass:</para>
	///
	/// <list type="bullet">
	///   <item><description>Templar (Malthael variant) → <c>30592</c> (melee).</description></item>
	///   <item><description>Scoundrel → <c>99902</c> (ranged projectile).</description></item>
	///   <item><description>Enchantress → <c>30273</c> (magic missile).</description></item>
	///   <item><description>Leah → <c>99902</c> (ranged projectile).</description></item>
	/// </list>
	///
	/// <para>To add more variety to a hireling, extend the constructor's
	/// preset list — <see cref="PickPowerToUse"/> picks a random entry from
	/// the list each cast.</para>
	/// </summary>
	public class HirelingBrain : Brain
	{
		/// <summary>The player this hireling follows and fights for.</summary>
		public Player Owner { get; private set; }

		/// <summary>
		/// Hard-coded list of power SNOs this hireling may use. Populated
		/// in the constructor and only re-read, not re-populated.
		/// </summary>
		public List<int> PresetPowers { get; private set; }

		/// <summary>Current combat target, refreshed each tick while attacking.</summary>
		private Actor _target { get; set; }

		/// <summary>Global 1-second cadence between attack attempts.</summary>
		private TickTimer _powerDelay;

		/// <summary>Sticky flag: true while fleeing from a fear effect.</summary>
		private bool Feared = false;

		/// <summary>
		/// Creates a hireling brain and hard-codes the one-and-only power
		/// SNO based on the hireling class.
		/// </summary>
		/// <param name="body">The hireling actor.</param>
		/// <param name="master">The player this hireling is bound to.</param>
		public HirelingBrain(Actor body, Player master)
			: base(body)
		{
			Owner = master;

			PresetPowers = new List<int>();

			// Class-specific power assignment. This is deliberately a short
			// list — one attack per class gives hirelings a recognisable
			// "feel" (Scoundrel always shoots, Enchantress always zaps).
			if (body is Templar && body is MalthaelHireling)
				PresetPowers.Add(30592); //melee instant
			if (body is Scoundrel)
				PresetPowers.Add(99902); //Scoundrel_ranged_Projectile
			if (body is Enchantress)
				PresetPowers.Add(30273); //HirelingMage_MagicMissile
			if (body is Leah)
				PresetPowers.Add(99902); //Scoundrel_ranged_Projectile

			Logger.Info("HirelingBrain spawned: {0} for player {1} with {2} power(s)",
				body?.SNO.ToString() ?? "<null>",
				master?.Toon?.Name ?? "<unknown>",
				PresetPowers.Count);
			if (PresetPowers.Count == 0)
				Logger.Warn("HirelingBrain {0}: no power SNOs assigned — hireling will be a no-op in combat",
					body?.SNO.ToString() ?? "<null>");
		}

		/// <summary>
		/// Main AI tick. Mirrors <see cref="MinionBrain.Think(int)"/> but
		/// scopes targeting to whatever monsters the owner has visible.
		/// </summary>
		public override void Think(int tickCounter)
		{
			// Orphaned hirelings (owner logged out, etc.) do nothing.
			if (Owner == null) return;

			if (Body.World.Game.Paused) return;

			// CC gate: cancel any running action and bail out.
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

			// Fear handling: same 3–8 tile flee as monsters/minions.
			if (Body.Attributes[GameAttributes.Feared])
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

			// Select and start executing a power if no action is in flight.
			if (CurrentAction == null)
			{
				// One-second cadence between attacks.
				if (_powerDelay == null)
					_powerDelay = new SecondsTickTimer(Body.World.Game, 1f);

				// Look for targets within 40 tiles of the owner (not the
				// hireling — so the hireling can pre-emptively lock on to
				// monsters the player is about to aggro).
				var targets = Owner.GetObjectsInRange<Monster>(40f).Where(p => !p.Dead && p.Visible).OrderBy(m => PowerMath.Distance2D(m.Position, Body.Position)).ToList();
				if (targets.Count != 0 && PowerMath.Distance2D(Body.Position, Owner.Position) < 80f)
				{
					int powerToUse = PickPowerToUse();
					if (powerToUse > 0)
					{
						// Prefer elites so Scoundrel/Enchantress damage
						// contributes to the high-value pack kills.
						var elite = targets.FirstOrDefault(t => t is Champion or Rare or RareMinion);
						_target = elite ?? targets.First();

						PowerScript power = PowerLoader.CreateImplementationForPowerSNO(powerToUse);
						power.User = Body;

						// Same range-computation convention as every other
						// brain in the system.
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
					// Idle follow — stay in a 3–8 tile ring around the
					// owner. Identical behaviour to MinionBrain.
					var distToMaster = PowerMath.Distance2D(Body.Position, Owner.Position);
					if ((distToMaster > 8f) || (distToMaster < 3f))
					{
						var Rand = FastRandom.Instance;
						var position = Owner.Position;
						float angle = (float)(Rand.NextDouble() * Math.PI * 2);
						float radius = 3f + (float)Rand.NextDouble() * (8f - 3f);
						var near = new Vector3D(position.X + (float)Math.Cos(angle) * radius, position.Y + (float)Math.Sin(angle) * radius, position.Z);
						CurrentAction = new MoveToPointAction(Body, near);
					}
				}
			}
		}

		/// <summary>
		/// Picks a random power SNO from <see cref="PresetPowers"/>,
		/// filtered to powers that actually have a C# implementation.
		/// Returns <c>-1</c> if nothing is available (e.g. unconfigured
		/// hireling subclass).
		/// </summary>
		protected virtual int PickPowerToUse()
		{
			// Randomly used an implemented power.
			var implementedPowers = PresetPowers.Where(PowerLoader.HasImplementationForPowerSNO);
			return implementedPowers.TryPickRandom(out var randomPower)
				? randomPower
				: -1;
		}

		/// <summary>
		/// Adds a power SNO at runtime. There is no cooldown tracking for
		/// hirelings — see <see cref="PresetPowers"/>.
		/// </summary>
		public void AddPresetPower(int powerSNO)
		{
			PresetPowers.Add(powerSNO);
		}
	}
}
