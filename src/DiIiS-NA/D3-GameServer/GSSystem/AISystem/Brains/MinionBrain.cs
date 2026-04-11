using System;
using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.Core.Extensions;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.Core.MPQ;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Actions;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
using DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.AISystem.Brains
{
	/// <summary>
	/// AI controller for player-summoned combat minions (Necromancer
	/// skeletons, Barbarian ancients, Witch Doctor fetishes / gargantuan,
	/// DH companion, etc.).
	///
	/// <para>Behaves like a leashed <c>MonsterBrain</c>:</para>
	/// <list type="bullet">
	///   <item><description>Attacks the nearest monster within 40 tiles of
	///     the master, preferring elites (Champion / Rare / RareMinion).</description></item>
	///   <item><description>Stays within a loose 3–8 tile ring around the
	///     master when no targets are nearby.</description></item>
	///   <item><description>Shares the same CC / fear / knockback gating
	///     as MonsterBrain.</description></item>
	///   <item><description>Applies the minion's
	///     <see cref="Minion.CooldownReduction"/> to its cast timers so
	///     pet-CDR stats actually work.</description></item>
	/// </list>
	///
	/// <para>In PvP the target list switches from monsters to opposing
	/// players (team-filtered in organised PvP, free-for-all otherwise).</para>
	/// </summary>
	public class MinionBrain : Brain
	{
		/// <summary>
		/// All powers this minion can use, keyed by power SNO. Filled in
		/// from the underlying Monster MPQ data at construction.
		/// </summary>
		public Dictionary<int, Cooldown> PresetPowers { get; private set; }

		/// <summary>Initial per-minion stagger timer so packs don't cast in unison.</summary>
		private TickTimer _powerDelay;

		/// <summary>Sticky flag: true while fleeing from a fear effect.</summary>
		private bool Feared = false;

		/// <summary>Cached target chosen inside <see cref="Think(int)"/>.</summary>
		private Actor _target { get; set; }

		/// <summary>One-shot warning flag to avoid log spam for powerless minions.</summary>
		private bool _warnedNoPowers;

		/// <summary>Per-power cooldown tracker. <see cref="CooldownTimer"/> null = ready.</summary>
		public struct Cooldown
		{
			/// <summary>Active timer; <c>null</c> when ready to cast.</summary>
			public TickTimer CooldownTimer;

			/// <summary>Base cooldown duration in seconds.</summary>
			public float CooldownTime;
		}

		/// <summary>
		/// Creates a minion brain and loads the preset powers from the
		/// underlying monster data. Every declared skill starts with a flat
		/// 1 s cooldown baseline.
		/// </summary>
		public MinionBrain(Actor body)
			: base(body)
		{
			PresetPowers = new Dictionary<int, Cooldown>();
			Logger.Trace("MinionBrain spawned for {0}", body?.SNO.ToString() ?? "<null>");

			// Build the list of powers defined in the monster MPQ data.
			if (body.ActorData.MonsterSNO > 0)
			{
				var monsterData = (DiIiS_NA.Core.MPQ.FileFormats.Monster)MPQStorage.Data.Assets[SNOGroup.Monster][body.ActorData.MonsterSNO].Data;
				foreach (var monsterSkill in monsterData.SkillDeclarations)
				{
					if (monsterSkill.SNOPower > 0)
					{
						PresetPowers.Add(monsterSkill.SNOPower, new Cooldown { CooldownTimer = null, CooldownTime = 1f });
					}
				}
			}
		}

		/// <summary>
		/// Main AI tick for a minion. See class remarks for the decision
		/// flow; mirrors <see cref="MonsterBrain.Think(int)"/> with the
		/// addition of master-leashing and PvP targeting.
		/// </summary>
		public override void Think(int tickCounter)
		{
			// this needed? /mdz
			//if (this.Body is NPC) return;

			// Without a master there is nothing to guard / follow, so skip.
			if ((Body as Minion).Master == null) return;

			if (Body.World.Game.Paused) return;

			// CC gate — identical to MonsterBrain. Cancel any running action
			// and bail; timers reset so the minion doesn't instantly cast
			// when the CC ends.
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

			// Fear handling — run to a random point 3–8 tiles away and
			// stop thinking until the fear expires.
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

			// Only select a new power if nothing is currently in-flight.
			if (CurrentAction == null)
			{
				// Small random delay on first think so groups of minions
				// don't all cast at the exact same tick (visual/feel win).
				if (_powerDelay == null)
					_powerDelay = new SecondsTickTimer(Body.World.Game, (float)RandomHelper.NextDouble());

				if (_powerDelay.TimedOut)
				{
					// Target acquisition — sweep 40 tiles around the master
					// and keep only the valid, visible, targetable monsters,
					// sorted by distance from the minion itself.
					List<Actor> targets = (Body as Minion).Master
						.GetObjectsInRange<Monster>(40f)
						.Where(m => !m.Dead && m.Visible && m.SNO.IsTargetable())
						.OrderBy(m => PowerMath.Distance2D(m.Position, Body.Position))
						.Cast<Actor>()
						.ToList();

					// PvP overrides: attack enemy players instead of monsters.
					if (Body.World.Game.PvP)
						targets = (Body as Minion).Master.GetObjectsInRange<Player>(30f).Where(p => p.GlobalID != (Body as Minion).Master.GlobalID && p.Attributes[GameAttributes.TeamID] != (Body as Minion).Master.Attributes[GameAttributes.TeamID]).Cast<Actor>().ToList();
					if (Body.World.IsPvP)
						targets = (Body as Minion).Master.GetObjectsInRange<Player>(30f).Where(p => p.GlobalID != (Body as Minion).Master.GlobalID).Cast<Actor>().ToList();

					// 80-tile leash — if the minion has wandered too far
					// from the master we skip attacking entirely and walk
					// back (see the else branch below).
					if (targets.Count != 0 && PowerMath.Distance2D(Body.Position, (Body as Minion).Master.Position) < 80f)
					{
						// Prefer elites so pet builds actually contribute to
						// elite kills (which are the high-value pack drops).
						var elite = targets.FirstOrDefault(target => target is Champion or Rare or RareMinion);
						_target = elite ?? targets.First();

						int powerToUse = PickPowerToUse();
						if (powerToUse > 0)
						{
							PowerScript power = PowerLoader.CreateImplementationForPowerSNO(powerToUse);
							power.User = Body;

							// Same range computation as MonsterBrain:
							// body cylinder + power.AttackRadius, with a
							// 10-tile floor for melee and a 35-tile fallback.
							float attackRange = Body.ActorData.Cylinder.Ax2 + (power.EvalTag(PowerKeys.AttackRadius) > 0f ? (powerToUse == 30592 ? 10f : power.EvalTag(PowerKeys.AttackRadius)) : 35f);
							float targetDistance = PowerMath.Distance2D(_target.Position, Body.Position);
							if (targetDistance < attackRange + _target.ActorData.Cylinder.Ax2)
							{
								// In range → face the target (unless we're
								// a fixed / pillar-type minion) and cast.
								if (Body.WalkSpeed != 0)
									Body.TranslateFacing(_target.Position, false); //columns and other non-walkable shit can't turn

								float cdReduction = (Body as Minion).CooldownReduction;

								//Logger.Trace("PowerAction to target");
								CurrentAction = new PowerAction(Body, powerToUse, _target);

								// Summon skills get a 7s cooldown (scaled
								// by pet CDR) so pets can't indefinitely
								// spawn more pets.
								if (power is SummoningSkill)
									PresetPowers[powerToUse] = new Cooldown { CooldownTimer = null, CooldownTime = (7f * cdReduction) };

								// Arm the cooldown for the just-cast power.
								if (PresetPowers[powerToUse].CooldownTime > 0f)
									PresetPowers[powerToUse] = new Cooldown { CooldownTimer = new SecondsTickTimer(Body.World.Game, PresetPowers[powerToUse].CooldownTime), CooldownTime = (PresetPowers[powerToUse].CooldownTime * cdReduction) };
							}
							else
							{
								// Out of range → pathfind in.
								//Logger.Trace("$[underline white]$MoveToTargetWithPathfindAction$[/]$ to target");
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
									attackRange + _target.ActorData.Cylinder.Ax2
								);
							}
						}
					}
					else
					{
						// No target in range (or too far from master) →
						// drift into a loose ring 3–8 tiles from the master.
						// This is the "wander with the player" idle behaviour.
						var distToMaster = PowerMath.Distance2D(Body.Position, (Body as Minion).Master.Position);
						if ((distToMaster > 8f) || (distToMaster < 3f))
						{
							var Rand = FastRandom.Instance;
							var position = (Body as Minion).Master.Position;
							float angle = (float)(Rand.NextDouble() * Math.PI * 2);
							float radius = 3f + (float)Rand.NextDouble() * (8f - 3f);
							var near = new Vector3D(position.X + (float)Math.Cos(angle) * radius, position.Y + (float)Math.Sin(angle) * radius, position.Z);
							CurrentAction = new MoveToPointAction(Body, near);
						}
					}
				}
			}
		}

		/// <summary>
		/// Picks a random power whose cooldown has expired and which has a
		/// C# implementation. Biased away from melee: if any non-melee power
		/// is available it is preferred, and melee is only chosen as a
		/// fallback. Returns <c>-1</c> when nothing is ready.
		/// </summary>
		protected virtual int PickPowerToUse()
		{
			if (!_warnedNoPowers && PresetPowers.Count == 0)
			{
				Logger.Debug("Minion \"{0}\" has no usable powers. ", Body.Name);
				_warnedNoPowers = true;
			}

			// Randomly use an implemented power.
			if (PresetPowers.Count > 0)
			{
				// int power = this.PresetPowers[RandomHelper.Next(this.PresetPowers.Count)].Key;
				List<int> availablePowers = PresetPowers.Where(p => (p.Value.CooldownTimer == null || p.Value.CooldownTimer.TimedOut) && PowerLoader.HasImplementationForPowerSNO(p.Key)).Select(p => p.Key).ToList();
				if (availablePowers.Where(p => p != 30592).TryPickRandom(out var randomItem))
					return randomItem;
				if (availablePowers.Contains(30592))
					return 30592; // melee attack
			}

			// No usable power.
			return -1;
		}

		/// <summary>
		/// Adds a runtime power to this minion's repertoire. Duplicates are
		/// silently skipped. New powers get a 5 s cooldown if the minion
		/// already has melee (so they don't fire too often), or a random
		/// 1–2 s placeholder otherwise.
		/// </summary>
		public void AddPresetPower(int powerSNO)
		{
			if (PresetPowers.ContainsKey(powerSNO))
			{
				// Logger.MethodTrace("power sno {0} already defined for monster \"{1}\"",
				//powerSNO, this.Body.ActorSNO.Name);
				return;
			}
			if (PresetPowers.ContainsKey(30592)) //if can cast melee
				PresetPowers.Add(powerSNO, new Cooldown { CooldownTimer = null, CooldownTime = 5f });
			else
				PresetPowers.Add(powerSNO, new Cooldown { CooldownTimer = null, CooldownTime = 1f + (float)FastRandom.Instance.NextDouble() });
		}

		/// <summary>Removes a power from this minion. No-op if it wasn't present.</summary>
		public void RemovePresetPower(int powerSNO)
		{
			if (PresetPowers.ContainsKey(powerSNO))
			{
				PresetPowers.Remove(powerSNO);
			}
		}
	}
}
