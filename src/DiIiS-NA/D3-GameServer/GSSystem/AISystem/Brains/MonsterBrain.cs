using System;
using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.Core.Extensions;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.Core.MPQ;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.SNO;
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
    /// AI controller for every combat-capable monster (normal, champion,
    /// rare and boss). Drives aggro acquisition, target selection, power
    /// usage and pathfinding.
    ///
    /// <para>Decision flow executed every <see cref="Think(int)"/>:</para>
    /// <list type="number">
    ///   <item><description><see cref="ShouldSkipThink"/> — special-case
    ///     opt-outs (Siegebreaker, garden pillars, hidden/dead bodies,
    ///     paused games).</description></item>
    ///   <item><description><see cref="IsCrowdControlled"/> — bail on stun,
    ///     freeze, blind, web, knockback or the summoning intro buff.</description></item>
    ///   <item><description><see cref="HandleFearEffect(int)"/> — flee for
    ///     the duration of any <c>Feared</c> flag.</description></item>
    ///   <item><description>Throttle: only run the expensive combat block
    ///     every <c>60 * GameServerConfig.MonsterThinkTick</c> ticks.</description></item>
    ///   <item><description><see cref="EvaluateTargetsAndAct(int)"/> — retarget
    ///     on a 2-second cadence and attack on a 1-second cadence.</description></item>
    /// </list>
    ///
    /// <para>See <c>docs/Battle.md</c> §3.2 for a full walkthrough and the
    /// balance knobs in <see cref="DEFAULT_SEARCH_RANGE"/>,
    /// <see cref="POWER_DELAY_SECONDS"/>, etc.</para>
    /// </summary>
    public class MonsterBrain : Brain
    {
        /// <summary>Named logger; hides the base <c>Brain.Logger</c>.</summary>
        private new readonly Logger Logger;

        /// <summary>
        /// All powers this monster can use, keyed by power SNO, each with
        /// its own individual <see cref="Cooldown"/> tracker. Populated from
        /// the monster's MPQ <c>SkillDeclarations</c> in the constructor.
        /// A synthetic melee entry (<see cref="MELEE_ATTACK_SNO"/>) is added
        /// if no melee is declared, so every monster can always swing.
        /// </summary>
        public Dictionary<int, Cooldown> PresetPowers { get; private set; }

        /// <summary>Global throttle between attack attempts (1 second).</summary>
        private TickTimer _powerDelay;

        /// <summary>Throttle between target re-selection passes (2 seconds).</summary>
        private TickTimer _targetUpdateDelay;

        /// <summary>
        /// Per-power cooldown tracker. <see cref="CooldownTimer"/> being
        /// <c>null</c> means "off-cooldown right now".
        /// </summary>
        public struct Cooldown
        {
            /// <summary>Active timer; <c>null</c> when the power is ready.</summary>
            public TickTimer CooldownTimer;

            /// <summary>Base cooldown duration in seconds (pre-CDR).</summary>
            public float CooldownTime;
        }

        // ─────────────────────────── AI tuning constants ──────────────────
        // These are the primary balance knobs for monster AI responsiveness.
        // See docs/Battle.md §5.6 for guidance on tuning them.

        /// <summary>Aggro / target search radius in world units.</summary>
        private const int DEFAULT_SEARCH_RANGE = 50;

        /// <summary>Power SNO of the basic monster melee attack.</summary>
        private const int MELEE_ATTACK_SNO = 30592;

        /// <summary>Power SNO of the Firewall special (shares a cooldown bucket).</summary>
        private const int FIREWALL_SNO = 223284;

        /// <summary>Reserved — unused, retained for future firewall handling.</summary>
        private const int FIREWALL_ACTOR_SNO = -1; // ActorSno._a1dun_leor_firewall2 - use actual value

        /// <summary>Melee attack range padding added on top of the body cylinder.</summary>
        private const float BASE_MELEE_RANGE = 10f;

        /// <summary>Absolute upper bound for monster attack range.</summary>
        private const float MAX_ATTACK_RANGE = 35f;

        /// <summary>Minimum distance to flee when feared.</summary>
        private const float FEARED_RETREAT_MIN = 3f;

        /// <summary>Maximum distance to flee when feared.</summary>
        private const float FEARED_RETREAT_MAX = 8f;

        /// <summary>Seconds between attack attempts (default monster cadence).</summary>
        private const float POWER_DELAY_SECONDS = 1.0f;

        /// <summary>Seconds between target re-selection passes.</summary>
        private const float TARGET_UPDATE_DELAY_SECONDS = 2.0f;

        /// <summary>Cooldown applied to boss summon skills after each cast.</summary>
        private const float SUMMONING_COOLDOWN_BOSS = 15f;

        /// <summary>Cooldown applied to non-boss summon skills after each cast.</summary>
        private const float SUMMONING_COOLDOWN_NORMAL = 7f;

        /// <summary>Cooldown applied to "special" powers (firewall, 96925).</summary>
        private const float SPECIAL_POWER_COOLDOWN = 10f;

        // ──────────────────────── Per-brain state ────────────────────────

        /// <summary>Set once after logging a "no usable powers" warning, to avoid log spam.</summary>
        private bool _warnedNoPowers;

        /// <summary>Cached current target chosen by <see cref="UpdateTarget"/>.</summary>
        private Actor _target;

        /// <summary>Count of powers declared in MPQ (for the "no usable powers" warning).</summary>
        private int _mpqPowerCount;

        /// <summary>Sticky flag: true while we are actively running from a fear effect.</summary>
        private bool _feared;

        /// <summary>
        /// Last actor that attacked this monster. Used as priority target so
        /// the monster retaliates against whoever is currently hurting it.
        /// Written by <c>AttackPayload.Apply</c> when a player hits a monster.
        /// </summary>
        public Actor AttackedBy;

        /// <summary>
        /// Expiration timer on <see cref="AttackedBy"/>. Reserved for future
        /// use — currently the retaliation persists until the target dies.
        /// </summary>
        public TickTimer TimeoutAttacked;

        /// <summary>
        /// Scripted priority target — takes precedence over both
        /// <see cref="AttackedBy"/> and nearest-enemy selection.
        /// Set from boss phase scripts to lock a boss onto a specific player.
        /// </summary>
        public Actor PriorityTarget;

        /// <summary>
        /// Creates a new monster brain, loading the body's declared skills
        /// from the monster MPQ file and building the initial
        /// <see cref="PresetPowers"/> dictionary.
        /// </summary>
        /// <param name="body">The monster actor this brain will drive.</param>
        public MonsterBrain(Actor body)
            : base(body)
        {
            Logger = LogManager.CreateLogger(GetType().Name);
            PresetPowers = new Dictionary<int, Cooldown>();

            // Actors without a monster SNO (pillars, dummies, visuals) have
            // nothing to do — leave PresetPowers empty.
            if (body.ActorData.MonsterSNO <= 0)
            {
                Logger.Warn($"$[red]${GetType().Name}$[/]$ - $[red]$Monster $[white bold underline]$\"{body.SNO}\"$[/]$$ has no monster SNO$[/]$");
                return;
            }

            // Load the monster's MPQ record to enumerate its declared skills.
            var monsterData = (DiIiS_NA.Core.MPQ.FileFormats.Monster)MPQStorage.Data.Assets[SNOGroup.Monster][body.ActorData.MonsterSNO].Data;
            _mpqPowerCount = monsterData.SkillDeclarations.Count(e => e.SNOPower != -1);

            // Walk every declared skill in parallel with its MonsterSkillDeclarations
            // entry (which carries the Timer / cooldown). Only skills that we
            // have an actual C# implementation for end up in PresetPowers.
            for (int i = 0; i < monsterData.SkillDeclarations.Length; i++)
            {
                if (monsterData.SkillDeclarations[i].SNOPower == -1) continue;
                if (PowerLoader.HasImplementationForPowerSNO(monsterData.SkillDeclarations[i].SNOPower))
                {
                    // MPQ stores cooldown as tenths of seconds.
                    var cooldownTime = monsterData.MonsterSkillDeclarations[i].Timer / 10f;
                    PresetPowers.Add(monsterData.SkillDeclarations[i].SNOPower, new Cooldown { CooldownTimer = null, CooldownTime = cooldownTime });
                }
            }

            // Guarantee every monster has *some* basic attack — if no melee
            // was declared, inject the canonical melee SNO with zero cooldown.
            if (!monsterData.SkillDeclarations.Any(s => s.SNOPower == MELEE_ATTACK_SNO))
                PresetPowers.Add(MELEE_ATTACK_SNO, new Cooldown { CooldownTimer = null, CooldownTime = 0f });
        }

        /// <summary>
        /// Main AI tick. Runs the decision flow documented on the class.
        /// </summary>
        /// <param name="tickCounter">Current game tick.</param>
        public override void Think(int tickCounter)
        {
            // Step 1: hard opt-outs (special actors, hidden, dead, paused).
            if (ShouldSkipThink())
                return;

            // Step 2: CC gate — stunned/frozen/blind/webbed/knocked/summoning.
            if (IsCrowdControlled())
                return;

            // Step 3: fear handling — run away for the duration of the fear.
            if (HandleFearEffect(tickCounter))
                return;

            _feared = false;

            // Step 4: throttle. GameServerConfig.MonsterThinkTick is in
            // seconds; 1.0 → think once per second, 0.5 → twice per second.
            // This is the primary lever for monster AI responsiveness.
            if (tickCounter % (60 * GameServerConfig.Instance.MonsterThinkTick) != 0)
                return;

            // Step 5: main combat logic only runs if we are not already mid-action.
            if (CurrentAction == null)
            {
                EvaluateTargetsAndAct(tickCounter);
            }
        }

        /// <summary>
        /// Hard opt-outs that must skip every <see cref="Think(int)"/>
        /// pass. Covers scripted uber bosses, non-thinking decorative
        /// actors (pillars, portals), NPCs, hidden or dead bodies, paused
        /// games and the global <c>Disabled</c> flag.
        /// </summary>
        /// <returns><c>true</c> if this brain should not think this tick.</returns>
        private bool ShouldSkipThink()
        {
            // Hard-coded special actors whose AI is externally scripted
            // (boss phase state machines, etc.) and must not run the default
            // monster brain logic.
            switch (Body.SNO)
            {
                case ActorSno._uber_siegebreakerdemon:
                case ActorSno._a4dun_garden_corruption_monster:
                case ActorSno._a4dun_garden_hellportal_pillar:
                case ActorSno._belialvoiceover:
                    return true;
            }

            // Skip NPCs (shopkeepers / quest givers), hidden / invisible
            // actors, dead bodies, paused games, and anything the game has
            // explicitly flagged Disabled.
            return Body is NPC || Body.Hidden || !Body.Visible || Body.Dead ||
                   Body.World.Game.Paused || Body.Attributes[GameAttributes.Disabled];
        }

        /// <summary>
        /// Returns true if the monster is under any form of crowd control
        /// that should cancel its current action and prevent it from acting
        /// this tick. Also cancels <see cref="Brain.CurrentAction"/> when CC
        /// is detected so interrupted attacks do not resume when the CC ends.
        /// </summary>
        private bool IsCrowdControlled()
        {
            bool isCrowdControlled = Body.Attributes[GameAttributes.Frozen] ||
                                     Body.Attributes[GameAttributes.Stunned] ||
                                     Body.Attributes[GameAttributes.Blind] ||
                                     Body.Attributes[GameAttributes.Webbed] ||
                                     Body.Disable ||
                                     // KnockbackBuff and SummonedBuff are
                                     // implemented as regular buffs — the
                                     // summoning buff is what holds newly
                                     // summoned mobs still during their intro.
                                     Body.World.BuffManager.GetFirstBuff<KnockbackBuff>(Body) != null ||
                                     Body.World.BuffManager.GetFirstBuff<SummonedBuff>(Body) != null;

            if (isCrowdControlled)
            {
                CancelCurrentAction();
                _powerDelay = null;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Runs the monster in a random direction while the <c>Feared</c>
        /// attribute is set. Re-rolls the flee target on the edge (i.e. when
        /// the fear first starts or when the previous flee finishes), but
        /// does not interrupt an already-running flee.
        /// </summary>
        /// <returns><c>true</c> if fear is active and this tick should stop processing.</returns>
        private bool HandleFearEffect(int tickCounter)
        {
            if (!Body.IsFeared)
                return false;

            // Start a new flee only on rising edge (we weren't feared last
            // tick) or if the previous flee action completed / was cancelled.
            if (!_feared || CurrentAction == null)
            {
                if (!_feared)
                    Logger.Trace("{0} began fleeing (feared)", Body.SNO);
                CancelCurrentAction();
                _feared = true;
                CurrentAction = new MoveToPointWithPathfindAction(
                    Body,
                    // Random point between FEARED_RETREAT_MIN and
                    // FEARED_RETREAT_MAX tiles away, in any direction.
                    PowerContext.RandomDirection(Body.Position, FEARED_RETREAT_MIN, FEARED_RETREAT_MAX)
                );
            }

            return true;
        }

        /// <summary>
        /// Throttled retarget + attack step. Called from <see cref="Think(int)"/>
        /// at most once per <see cref="POWER_DELAY_SECONDS"/> after the
        /// overall throttle has already decided we can think this tick.
        /// </summary>
        private void EvaluateTargetsAndAct(int tickCounter)
        {
            // Lazy-init the timers the first time this monster actually gets
            // to act (zero-arg ctor would run during monster load and waste
            // allocations for mobs the player never gets near).
            _powerDelay ??= new SecondsTickTimer(Body.World.Game, POWER_DELAY_SECONDS);
            _targetUpdateDelay ??= new SecondsTickTimer(Body.World.Game, TARGET_UPDATE_DELAY_SECONDS);

            // Refresh the target every TARGET_UPDATE_DELAY_SECONDS. Cheaper
            // than rescanning every tick and also gives the player a chance
            // to kite — no instant re-aggro.
            if (_targetUpdateDelay.TimedOut)
            {
                _targetUpdateDelay = new SecondsTickTimer(Body.World.Game, TARGET_UPDATE_DELAY_SECONDS);
                UpdateTarget();
            }

            // Gate attacks on POWER_DELAY_SECONDS. This is the "rhythm" of
            // monster attacks — everyone attacks at ~1 Hz unless a specific
            // power has its own per-skill cooldown.
            if (_powerDelay.TimedOut)
            {
                _powerDelay = new SecondsTickTimer(Body.World.Game, POWER_DELAY_SECONDS);

                if (_target != null && !_target.Dead)
                {
                    ExecuteAttackOnTarget(tickCounter);
                }
                else if (Body.Position != Body.CheckPointPosition)
                {
                    // No target in range → drift back to spawn so packs
                    // don't slowly migrate across the level.
                    CurrentAction = new MoveToPointWithPathfindAction(Body, Body.CheckPointPosition);
                }
            }
        }

        /// <summary>
        /// Selects the next target in priority order:
        /// <list type="number">
        ///   <item><description>Scripted <see cref="PriorityTarget"/>.</description></item>
        ///   <item><description>Most recent <see cref="AttackedBy"/>.</description></item>
        ///   <item><description>Nearest valid combat target (see
        ///     <see cref="IsValidCombatTarget(Actor)"/>).</description></item>
        /// </list>
        /// </summary>
        private void UpdateTarget()
        {
            var prevTarget = _target;

            // Scripted priority — boss phase logic uses this to lock onto a
            // specific player regardless of positioning.
            if (PriorityTarget != null && !PriorityTarget.Dead)
            {
                _target = PriorityTarget;
                if (prevTarget != _target)
                    Logger.Trace("{0} UpdateTarget → priority target {1}", Body.SNO, _target.SNO);
                return;
            }

            // Retaliation priority — attack whoever hit us last. This is how
            // ranged mobs remember to keep shooting at the caster instead of
            // wandering off to the nearest player.
            if (AttackedBy != null && !AttackedBy.Dead)
            {
                PriorityTarget = AttackedBy;
                _target = AttackedBy;
                if (prevTarget != _target)
                    Logger.Trace("{0} UpdateTarget → retaliation target {1}", Body.SNO, _target.SNO);
                return;
            }

            // Fallback: nearest valid actor.
            var nearbyTargets = FindValidTargets();
            _target = nearbyTargets.FirstOrDefault();
            if (prevTarget != _target && _target != null)
                Logger.Trace("{0} UpdateTarget → nearest target {1}", Body.SNO, _target.SNO);
            else if (prevTarget != null && _target == null)
                Logger.Trace("{0} lost target (prev: {1})", Body.SNO, prevTarget.SNO);
        }

        /// <summary>
        /// Builds a distance-sorted list of valid combat targets in
        /// <see cref="DEFAULT_SEARCH_RANGE"/>. Respects the
        /// <c>Team_Override</c> flag, which flips the monster onto the
        /// player's side (betrayal scenarios) — betrayed monsters then
        /// search for other monsters instead of players.
        /// </summary>
        private List<Actor> FindValidTargets()
        {
            var validTargets = new List<Actor>();

            if (Body.Attributes[GameAttributes.Team_Override] == 1)
            {
                // Team override: mind-controlled monsters attack their
                // former allies.
                validTargets.AddRange(
                    Body.GetObjectsInRange<Monster>(DEFAULT_SEARCH_RANGE)
                        .Where(p => !p.Dead)
                        .OrderBy(m => PowerMath.Distance2D(m.Position, Body.Position))
                );
            }
            else
            {
                // Normal targeting: any actor in range that passes
                // IsValidCombatTarget, sorted by distance.
                validTargets.AddRange(
                    Body.GetActorsInRange(DEFAULT_SEARCH_RANGE)
                        .Where(IsValidCombatTarget)
                        .OrderBy(a => PowerMath.Distance2D(a.Position, Body.Position))
                );
            }

            return validTargets;
        }

        /// <summary>
        /// Predicate for "is this actor a legal target for this monster to
        /// attack right now". Rejects dead / hidden / ghosted actors and
        /// non-combat helper minions; accepts players, fighting minions,
        /// hirelings and (specifically) door/barricade destructibles.
        /// </summary>
        private bool IsValidCombatTarget(Actor actor)
        {
            if (actor.Dead || actor == Body || actor.Hidden)
                return false;

            // Players: must be fully loaded, not a helper actor, and not
            // ghosted (e.g. corpse walker invulnerability).
            if (actor is Player player)
            {
                return !player.Attributes[GameAttributes.Loading] &&
                       !player.Attributes[GameAttributes.Is_Helper] &&
                       player.World.BuffManager.GetFirstBuff<ActorGhostedBuff>(player) == null;
            }

            // Summoned minions: attack combat minions, ignore utility helpers.
            if (actor is Minion minion)
            {
                return !minion.Attributes[GameAttributes.Is_Helper];
            }

            // Hirelings (Templar, Scoundrel, Enchantress, Leah) are always
            // legal targets.
            if (actor is Hireling)
            {
                return true;
            }

            // Only doors and barricades among destructibles — we don't want
            // mobs to randomly smash lootable vases.
            if (actor is DesctructibleLootContainer destructible)
            {
                return destructible.SNO.IsDoorOrBarricade();
            }

            return false;
        }

        /// <summary>
        /// Picks a power, computes range, then either casts in place or
        /// pursues the target until in range.
        /// </summary>
        private void ExecuteAttackOnTarget(int tickCounter)
        {
            int powerToUse = PickPowerToUse();
            if (powerToUse <= 0)
                return;

            PowerScript power = PowerLoader.CreateImplementationForPowerSNO(powerToUse);
            power.User = Body;

            float attackRange = CalculateAttackRange(power, powerToUse);
            float targetDistance = PowerMath.Distance2D(_target.Position, Body.Position);

            if (IsTargetInRange(targetDistance, attackRange))
            {
                ExecutePowerAttack(powerToUse, power);
            }
            else if (CanApproachTarget())
            {
                ApproachTarget(powerToUse, attackRange);
            }
            // Else: immobile mob out of range → no action this tick; the
            // next tick's target update may find a closer target.
        }

        /// <summary>
        /// Computes the effective attack range for a given power:
        /// body cylinder + power <c>AttackRadius</c>, with a floor for
        /// melee and a ceiling of <see cref="MAX_ATTACK_RANGE"/>.
        /// </summary>
        private float CalculateAttackRange(PowerScript power, int powerSNO)
        {
            float baseRange = Body.ActorData.Cylinder.Ax2; // body radius
            float powerRange = power.EvalTag(PowerKeys.AttackRadius);

            if (powerRange > 0f)
            {
                // Melee gets a flat 10-tile padding so monsters actually
                // commit to their swing instead of orbiting their target.
                if (powerSNO == MELEE_ATTACK_SNO)
                    return baseRange + BASE_MELEE_RANGE;

                return baseRange + Math.Min(powerRange, MAX_ATTACK_RANGE);
            }

            // Power declared no attack radius → assume long-range.
            return baseRange + MAX_ATTACK_RANGE;
        }

        /// <summary>
        /// Returns <c>true</c> if the target is within the computed attack
        /// range, accounting for the target's own body cylinder (so big
        /// targets like bosses are hit from the edge, not the centre).
        /// </summary>
        private bool IsTargetInRange(float targetDistance, float attackRange)
        {
            return targetDistance < attackRange + _target.ActorData.Cylinder.Ax2;
        }

        /// <summary>
        /// Immobile monsters (e.g. stationary casters, siege weapons) have
        /// <c>WalkSpeed == 0</c>; they cannot pursue.
        /// </summary>
        private bool CanApproachTarget()
        {
            return Body.WalkSpeed != 0;
        }

        /// <summary>
        /// Actually fires the attack: face the target, queue a
        /// <see cref="PowerAction"/>, and apply the per-power cooldown.
        /// </summary>
        private void ExecutePowerAttack(int powerSNO, PowerScript power)
        {
            // Only rotate if we can actually turn (pillars / columns have
            // WalkSpeed 0 and face a fixed direction).
            if (Body.WalkSpeed != 0)
                Body.TranslateFacing(_target.Position, false);

            CurrentAction = new PowerAction(Body, powerSNO, _target);
            ApplyPowerCooldown(powerSNO, power);

            Logger.Trace($"{GetType().Name} {nameof(PowerAction)} on {_target.ActorType} at {_target.Position}");
        }

        /// <summary>
        /// Moves the body toward the target. Woodwraith/wasp variants use
        /// a non-pathfinding <see cref="MoveToPointAction"/> to preserve
        /// their swooping / flight behaviour — they are not bound to
        /// navmesh cells.
        /// </summary>
        private void ApproachTarget(int powerSNO, float attackRange)
        {
            if (Body.SNO.IsWoodwraithOrWasp())
            {
                // Flying mobs — straight-line move, skip pathfinding.
                CurrentAction = new MoveToPointAction(Body, _target.Position);
                Logger.Trace($"{GetType().Name} approaching target (ranged) at {_target.Position}");
            }
            else
            {
                // Ground mobs — proper navmesh pathfinding that stops at
                // the edge of attack range, carrying the intended powerSNO
                // so the action can transition straight into a cast when
                // it arrives.
                CurrentAction = new MoveToTargetWithPathfindAction(
                    Body,
                    _target,
                    attackRange + _target.ActorData.Cylinder.Ax2,
                    powerSNO
                );
                Logger.Trace($"{GetType().Name} approaching target with pathfinding");
            }
        }

        /// <summary>
        /// Applies a cooldown to the just-cast power. Respects
        /// <c>GameServerConfig.DisableMonsterPowerCooldowns</c> as a global
        /// kill switch and picks the correct duration based on power type.
        /// </summary>
        /// <param name="powerSNO">Power that was cast.</param>
        /// <param name="power">Resolved <see cref="PowerScript"/> for the power.</param>
        /// <param name="cooldownTime">Optional override; <c>0</c> means "use per-category default".</param>
        private void ApplyPowerCooldown(int powerSNO, PowerScript power, float cooldownTime = 0f)
        {
            // Global kill-switch for stress tests — never ship with this on.
            if (GameServerConfig.Instance.DisableMonsterPowerCooldowns)
                return;

            // Category-based cooldowns when the MPQ didn't set one.
            if (power is SummoningSkill)
            {
                cooldownTime = Body is Boss ? SUMMONING_COOLDOWN_BOSS : SUMMONING_COOLDOWN_NORMAL;
            }
            else if (power is MonsterAffixSkill monsterAffixSkill)
            {
                cooldownTime = monsterAffixSkill.CooldownTime;
            }
            else if (IsSpecialPowerSNO(powerSNO))
            {
                cooldownTime = SPECIAL_POWER_COOLDOWN;
            }

            // Only overwrite the tracker if this power actually uses a cooldown.
            if (cooldownTime > 0f)
            {
                PresetPowers[powerSNO] = new Cooldown
                {
                    CooldownTimer = new SecondsTickTimer(Body.World.Game, cooldownTime),
                    CooldownTime = cooldownTime
                };
            }
        }

        /// <summary>
        /// Special-case powers that need a fixed global cooldown regardless
        /// of their MPQ data. Currently: <c>96925</c> and the Firewall
        /// effect (<see cref="FIREWALL_SNO"/>).
        /// </summary>
        private bool IsSpecialPowerSNO(int powerSNO)
        {
            return powerSNO == 96925 || powerSNO == FIREWALL_SNO;
        }

        /// <summary>Cancels and clears <see cref="Brain.CurrentAction"/> if one is running.</summary>
        private void CancelCurrentAction()
        {
            if (CurrentAction != null)
            {
                CurrentAction.Cancel(0);
                CurrentAction = null;
            }
        }

        /// <summary>
        /// Chooses a power to cast from <see cref="PresetPowers"/>. Filters
        /// to powers that are off-cooldown AND actually have a C# script
        /// implementation, then applies a 50/50 bias between "prefer
        /// non-melee" and "use melee" — which produces the familiar D3
        /// monster rhythm of mostly-melee-sometimes-ranged.
        /// </summary>
        /// <returns>Power SNO to cast, or <c>-1</c> if no power is available.</returns>
        protected virtual int PickPowerToUse()
        {
            // One-time warning for monsters that were defined in MPQ but
            // have no implemented powers. Not repeated each tick because it
            // would spam the log for every misconfigured mob in the world.
            if (!_warnedNoPowers && PresetPowers.Count == 0)
            {
                Logger.Warn($"Monster $[red]$\"{Body.Name}\"$[/]$ has no usable powers. {_mpqPowerCount} are defined in mpq data.");
                _warnedNoPowers = true;
                return -1;
            }

            if (PresetPowers.Count <= 0)
                return -1;

            // Enumerate the powers that are ready now: cooldown expired AND
            // we have an implementation class for them.
            var availablePowers = PresetPowers
                .Where(p => p.Value.CooldownTimer == null || p.Value.CooldownTimer.TimedOut)
                .Where(p => PowerLoader.HasImplementationForPowerSNO(p.Key))
                .Select(p => p.Key)
                .ToList();

            // 50% chance to try a non-melee pick; on failure we fall
            // through to melee. This keeps casters casting while still
            // letting them melee-poke at point-blank range.
            if (FastRandom.Instance.Chance(50))
            {
                if (availablePowers.Where(p => p != MELEE_ATTACK_SNO).TryPickRandom(out var selectedPower))
                    return selectedPower;
                else
                {
                    if (availablePowers.Contains(MELEE_ATTACK_SNO))
                        return MELEE_ATTACK_SNO;
                }
            }
            else
            {
                // Melee preferred branch.
                if (availablePowers.Contains(MELEE_ATTACK_SNO))
                    return MELEE_ATTACK_SNO;
            }
            return -1;
        }

        /// <summary>
        /// Adds a new power at runtime (e.g. when an elite affix grants an
        /// extra ability mid-fight). Assigns a reasonable placeholder
        /// cooldown — 5 s if the monster already has melee, or a random
        /// 1–2 s if not (so we don't generate a free-cast loop).
        /// </summary>
        public void AddPresetPower(int powerSNO)
        {
            if (PresetPowers.ContainsKey(powerSNO))
            {
                Logger.Debug($"Monster $[red]$\"{Body.Name}\"$[/]$ already has power {powerSNO}.");
                return;
            }

            float cooldownTime = PresetPowers.ContainsKey(MELEE_ATTACK_SNO) ? 5f : 1f + (float)FastRandom.Instance.NextDouble();
            PresetPowers.Add(powerSNO, new Cooldown { CooldownTimer = null, CooldownTime = cooldownTime });
        }

        /// <summary>
        /// Removes a power from this monster's repertoire. No-op if the
        /// power was not present.
        /// </summary>
        public void RemovePresetPower(int powerSNO)
        {
            if (PresetPowers.ContainsKey(powerSNO))
            {
                PresetPowers.Remove(powerSNO);
            }
        }

        /// <summary>
        /// Utility: picks a random navmesh-walkable point in an annulus
        /// around <paramref name="position"/>. Used by several skill
        /// implementations that need to place effects near an actor.
        /// Retries up to 100 times before giving up and returning the last
        /// point (which may be on an unwalkable cell — callers should
        /// re-validate if that matters).
        /// </summary>
        public static Core.Types.Math.Vector3D RandomPossibleDirection(Core.Types.Math.Vector3D position, float minRadius, float maxRadius, MapSystem.World world)
        {
            float angle = (float)(FastRandom.Instance.NextDouble() * Math.PI * 2);
            float radius = minRadius + (float)FastRandom.Instance.NextDouble() * (maxRadius - minRadius);
            Core.Types.Math.Vector3D point = null;
            int attemptCount = 0;

            // Reject-sample until we find a walkable point or time out.
            while (attemptCount < 100)
            {
                point = new Core.Types.Math.Vector3D(
                    position.X + (float)Math.Cos(angle) * radius,
                    position.Y + (float)Math.Sin(angle) * radius,
                    position.Z
                );

                if (world.CheckLocationForFlag(point, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
                    break;

                attemptCount++;
            }

            return point;
        }

        /// <summary>
        /// Bypasses normal targeting/delay logic and immediately casts the
        /// given skill on the given target. Used by boss phase scripts and
        /// other "scripted move" situations where the monster must react
        /// right now instead of waiting for its next think tick.
        /// </summary>
        public void FastAttack(Actor target, int skillSNO)
        {
            PowerScript power = PowerLoader.CreateImplementationForPowerSNO(skillSNO);
            power.User = Body;

            if (Body.WalkSpeed != 0)
                Body.TranslateFacing(target.Position, false);

            CurrentAction = new PowerAction(Body, skillSNO, target);
            ApplyPowerCooldown(skillSNO, power);

            //Logger.Trace($"{GetType().Name} {nameof(FastAttack)} on {target.ActorType}");
        }
    }
}
