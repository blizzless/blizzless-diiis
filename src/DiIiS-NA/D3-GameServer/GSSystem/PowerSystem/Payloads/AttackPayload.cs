using DiIiS_NA.Core.Logging;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem.Payloads
{
	/// <summary>
	/// First stage of the damage pipeline: the object a power builds when
	/// it decides to deal damage. Holds the list of damage entries, the
	/// set of targets, and the set of on-hit buffs to apply.
	///
	/// <para><see cref="Apply"/> iterates over <see cref="Targets"/>, rolls
	/// for crit once per target via <see cref="_DoCriticalHit"/>, then
	/// spawns a <see cref="HitPayload"/> per target and lets the
	/// <see cref="BuffManager"/> intercept the payload for any buffs that
	/// want to react to it.</para>
	///
	/// <para>Balance notes:</para>
	/// <list type="bullet">
	///   <item><description>The crit chance cap is hard-coded at
	///     <c>0.85f</c> in <see cref="_DoCriticalHit"/> — change this to
	///     tune the global crit ceiling.</description></item>
	///   <item><description>Raw damage numbers come from the
	///     <see cref="DamageEntry"/> list; weapon-based entries read the
	///     caster's weapon attributes inside
	///     <see cref="HitPayload"/>.</description></item>
	/// </list>
	/// </summary>
	public class AttackPayload : Payload
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		/// <summary>
		/// Targets to try and hit with this payload. MUST be set before
		/// calling <see cref="Apply"/>; either directly, or via
		/// <see cref="SetSingleTarget"/>.
		/// </summary>
		public TargetList Targets;

		/// <summary>
		/// Additive crit-chance bonus applied on top of the caster's
		/// normal crit stats (set by powers that want a one-off crit
		/// boost, e.g. guaranteed-crit finishers).
		/// </summary>
		public float chcBonus = 0f;

		/// <summary>
		/// A single damage component of an attack. A power can add
		/// multiple entries of different <see cref="DamageType"/> to
		/// combine, for instance, physical + fire damage in one hit.
		/// </summary>
		public class DamageEntry
		{
			/// <summary>Elemental / physical type of this component.</summary>
			public DamageType DamageType;

			/// <summary>Minimum flat damage (used when
			/// <see cref="IsWeaponBasedDamage"/> is false).</summary>
			public float MinDamage;

			/// <summary>Random spread added to <see cref="MinDamage"/>.</summary>
			public float DamageDelta;

			/// <summary>
			/// When true, damage is computed from the caster's weapon
			/// damage attributes scaled by
			/// <see cref="WeaponDamageMultiplier"/>. Used by most player
			/// skills (tooltip "X% weapon damage").
			/// </summary>
			public bool IsWeaponBasedDamage;

			/// <summary>Weapon-damage multiplier, e.g. 1.85 for 185%.</summary>
			public float WeaponDamageMultiplier;
		}

		/// <summary>List of damage components that make up this attack.</summary>
		public List<DamageEntry> DamageEntries = new List<DamageEntry>();

		/// <summary>
		/// Optional callback invoked on every successful hit. Used by
		/// powers that need custom per-hit logic (spawning visual effects,
		/// proccing on-hit runes, etc.).
		/// </summary>
		public Action<HitPayload> OnHit = null;

		/// <summary>
		/// Optional callback invoked when a hit kills its target. Passed
		/// through to the spawned <see cref="HitPayload"/>.
		/// </summary>
		public Action<DeathPayload> OnDeath = null;

		/// <summary>
		/// Some powers use custom hit effects but don't tag them correctly
		/// in the MPQ tagmaps. Setting this to <c>false</c> suppresses the
		/// automatic hit-effect generation inside <see cref="HitPayload"/>.
		/// </summary>
		public bool AutomaticHitEffects = true;

		/// <summary>
		/// Buffs to apply to every target that gets hit. Stored as
		/// factories so each target gets an independent buff instance.
		/// </summary>
		private List<Func<Buff>> _hitBuffs = new List<Func<Buff>>();

		/// <summary>
		/// Creates a new attack payload whose <see cref="Target"/> defaults
		/// to the caster. Call one of the <see cref="AddDamage"/> overloads
		/// and set <see cref="Targets"/> before calling <see cref="Apply"/>.
		/// </summary>
		public AttackPayload(PowerContext context)
			: base(context, context.User)
		{
		}

		/// <summary>
		/// Adds a flat (non-weapon-based) damage component.
		/// </summary>
		/// <param name="minDamage">Minimum damage.</param>
		/// <param name="damageDelta">Spread added to <paramref name="minDamage"/>.</param>
		/// <param name="damageType">Elemental / physical damage type.</param>
		public void AddDamage(float minDamage, float damageDelta, DamageType damageType)
		{
			DamageEntries.Add(new DamageEntry
			{
				DamageType = damageType,
				MinDamage = minDamage,
				DamageDelta = damageDelta,
				IsWeaponBasedDamage = false,
			});
		}

		/// <summary>
		/// Adds a weapon-damage-based component ("X% weapon damage"
		/// tooltip style). Actual value is resolved against the caster's
		/// weapon stats later, inside <see cref="HitPayload"/>.
		/// </summary>
		public void AddWeaponDamage(float damageMultiplier, DamageType damageType)
		{
			DamageEntries.Add(new DamageEntry
			{
				DamageType = damageType,
				IsWeaponBasedDamage = true,
				WeaponDamageMultiplier = damageMultiplier,
			});
		}

		/// <summary>Replaces <see cref="Targets"/> with a single-target list.</summary>
		public void SetSingleTarget(Actor target)
		{
			Targets = new TargetList();
			Targets.Actors.Add(target);
		}

		/// <summary>
		/// Registers a buff to apply to every target hit by this payload.
		/// </summary>
		public void AddBuffOnHit<T>() where T : Buff, new()
		{
			_hitBuffs.Add(() => new T());
		}

		/// <summary>
		/// Executes the attack:
		/// <list type="number">
		///   <item><description>Gives the <see cref="BuffManager"/> a chance
		///     to intercept / mutate the payload for both the target and
		///     the caster.</description></item>
		///   <item><description>Bails out if the stack is too deep
		///     (safeguard against power reflection / thorns loops).</description></item>
		///   <item><description>Notifies destructibles about being
		///     targeted by a player (so "smash the pot" rewards XP).</description></item>
		///   <item><description>Rolls crit per-target and spawns a
		///     <see cref="HitPayload"/> for each, applying any on-hit buffs
		///     along the way.</description></item>
		/// </list>
		/// </summary>
		public void Apply()
		{
			Targets ??= new TargetList();
			if (Target.World != null)
			{
				if (!Target.World.Game.Working) return;
				// Let the buff manager see the payload — buffs like
				// Thorns, taunts, or damage modifiers can mutate it here.
				Target.World.BuffManager.SendTargetPayload(Target, this);

				if (Context.User != null) Target.World.BuffManager.SendTargetPayload(Context.User, this);
			}
			// Stack depth safeguard: prevents infinite recursion from
			// reflect / thorns-style chains causing a StackOverflow.
			if (new System.Diagnostics.StackTrace().FrameCount > 35)
			{
				Logger.Warn("AttackPayload.Apply aborted: stack depth >35 (power {0}, user {1}). Likely reflect/thorns loop.",
					Context?.PowerSNO ?? -1,
					Context?.User?.SNO.ToString() ?? "<null>");
				return;
			}

			{
				// Destructible loot containers (pots, barrels) count as
				// "targeted" for player XP tracking.
				if (Target is Player player && DamageEntries.Count > 0)
				{
					foreach (Actor extra in Targets.ExtraActors)
						if (extra is DesctructibleLootContainer)
							extra.OnTargeted(player, null);

				}
			}
			{
				// When a player attacks a monster, remember who attacked
				// so XP bonuses (combat flow / massacre) can credit correctly.
				if (Context.User is Player player && Context.Target is Monster monster && monster.GBHandle.Type == 1)
				{
					player.ExpBonusData.MonsterAttacked(player.InGameClient.Game
						.TickCounter);
					((MonsterBrain)monster.Brain).AttackedBy = player;
				}
			}

			Logger.Trace("AttackPayload.Apply: power {0} from {1} → {2} target(s)",
				Context?.PowerSNO ?? -1,
				Context?.User?.SNO.ToString() ?? "<null>",
				Targets.Actors.Count);

			// Per-target: roll crit, build HitPayload, apply on-hit buffs,
			// fire optional OnHit callback, apply the hit.
			foreach (Actor target in Targets.Actors)
			{
				if (target == null || target.World == null || target.World != null && target.World.PowerManager.IsDeletingActor(target))
					continue;

				var payload = new HitPayload(this, _DoCriticalHit(Context.User, target, chcBonus)
					, target);
				payload.AutomaticHitEffects = AutomaticHitEffects;
				payload.OnDeath = OnDeath;

				foreach (Func<Buff> buffFactory in _hitBuffs)
					Context.AddBuff(target, buffFactory());
				if (payload.Successful)
				{
					try
					{
						if (OnHit != null && AutomaticHitEffects)
							OnHit(payload);
					}
					catch { }
					payload.Apply();
				}
			}
		}

		/// <summary>
		/// Computes per-hit crit chance and rolls once.
		///
		/// <para>The final crit chance is the sum of:</para>
		/// <list type="bullet">
		///   <item><description><c>Weapon_Crit_Chance</c> — base crit
		///     from weapon affix.</description></item>
		///   <item><description><c>Crit_Percent_Bonus_Capped</c> /
		///     <c>Uncapped</c> — flat +crit bonuses from gear / passives.</description></item>
		///   <item><description><c>Power_Crit_Percent_Bonus</c> — per-power
		///     crit bonus (e.g. "your Meteor has +10% crit chance").</description></item>
		///   <item><description><c>Bonus_Chance_To_Be_Crit_Hit</c> — debuff
		///     on the target (e.g. Crusader's Judgment rune).</description></item>
		///   <item><description><paramref name="chcBonus"/> — per-cast
		///     additive bonus from the payload itself.</description></item>
		///   <item><description>Class-specific bonuses (Single Out passive,
		///     Spectral Blade / Ice Blades, Judgment / Resolved,
		///     Punish / Fury).</description></item>
		/// </list>
		///
		/// <para>The sum is clamped to <c>0.85f</c> (the global crit
		/// ceiling — tune this to change maximum crit across the whole
		/// game).</para>
		///
		/// <para>Returns <c>false</c> early if the target is flagged
		/// <c>Ignores_Critical_Hits</c> or this is Monk's Exploding Palm
		/// without the Essence rune (which never crits).</para>
		/// </summary>
		private bool _DoCriticalHit(Actor user, Actor target, float chcBonus = 0f)
		{
			if (target.Attributes[GameAttributes.Ignores_Critical_Hits]) return false;

			//Monk -> Exploding Palm
			if (Context.PowerSNO == 97328 && Context.Rune_E <= 0) return false;

			float additionalCritChance = chcBonus;

			// DH Single Out: +25% crit vs. isolated targets.
			if (user is Player && (user as Player).SkillSet.HasPassive(338859)) //Single Out
				if (target.GetMonstersInRange(20f).All(m => m == target))
					additionalCritChance += 0.25f;

			//Wizard -> Spectral Blade -> Ice Blades
			if (target.World.BuffManager.HasBuff<Implementations.WizardSpectralBlade.BladesChcDebuff>(target))
				additionalCritChance += target.World.BuffManager.GetFirstBuff<Implementations.WizardSpectralBlade.BladesChcDebuff>(target).Percentage;

			//Crusader -> Judgment -> Resolved
			if (target.World.BuffManager.HasBuff<Implementations.CrusaderJudgment.JudgedDebuffRooted>(target))
				additionalCritChance += target.World.BuffManager.GetFirstBuff<Implementations.CrusaderJudgment.JudgedDebuffRooted>(target).bonusChC;

			//Crusader -> Punish -> Fury
			if (target.World.BuffManager.HasBuff<Implementations.CrusaderPunish.FuryChCBuff>(target))
			{
				additionalCritChance += target.World.BuffManager.GetFirstBuff<Implementations.CrusaderPunish.FuryChCBuff>(target).Percentage;
				target.World.BuffManager.RemoveBuffs(target, SkillsSystem.Skills.Crusader.FaithGenerators.Punish);
			}

			// Sum all sources → clamp to 0.85 global cap → roll.
			var totalCritChance = user.Attributes[GameAttributes.Weapon_Crit_Chance] + user.Attributes[GameAttributes.Crit_Percent_Bonus_Capped] + user.Attributes[GameAttributes.Crit_Percent_Bonus_Uncapped] + user.Attributes[GameAttributes.Power_Crit_Percent_Bonus, Context.PowerSNO] + target.Attributes[GameAttributes.Bonus_Chance_To_Be_Crit_Hit] + additionalCritChance;
			if (totalCritChance > 0.85f) totalCritChance = 0.85f;
			return PowerContext.Rand.NextDouble() < totalCritChance;
		}
	}
}
