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
    public class MonsterBrain : Brain
    {
        private new readonly Logger Logger;
        public Dictionary<int, Cooldown> PresetPowers { get; private set; }

        private TickTimer _powerDelay;
        private TickTimer _targetUpdateDelay;

        public struct Cooldown
        {
            public TickTimer CooldownTimer;
            public float CooldownTime;
        }

        // AI Configuration Constants
        private const int DEFAULT_SEARCH_RANGE = 50;
        private const int MELEE_ATTACK_SNO = 30592;
        private const int FIREWALL_SNO = 223284;
        private const int FIREWALL_ACTOR_SNO = -1; // ActorSno._a1dun_leor_firewall2 - use actual value
        private const float BASE_MELEE_RANGE = 10f;
        private const float MAX_ATTACK_RANGE = 35f;
        private const float FEARED_RETREAT_MIN = 3f;
        private const float FEARED_RETREAT_MAX = 8f;
        private const float POWER_DELAY_SECONDS = 1.0f;
        private const float TARGET_UPDATE_DELAY_SECONDS = 2.0f;
        private const float SUMMONING_COOLDOWN_BOSS = 15f;
        private const float SUMMONING_COOLDOWN_NORMAL = 7f;
        private const float SPECIAL_POWER_COOLDOWN = 10f;

        private bool _warnedNoPowers;
        private Actor _target;
        private int _mpqPowerCount;
        private bool _feared;

        public Actor AttackedBy;
        public TickTimer TimeoutAttacked;
        public Actor PriorityTarget;

        public MonsterBrain(Actor body)
            : base(body)
        {
            Logger = LogManager.CreateLogger(GetType().Name);
            PresetPowers = new Dictionary<int, Cooldown>();

            if (body.ActorData.MonsterSNO <= 0)
            {
                Logger.Warn($"$[red]${GetType().Name}$[/]$ - Monster \"{body.SNO}\" has no monster SNO");
                return;
            }

            var monsterData = (DiIiS_NA.Core.MPQ.FileFormats.Monster)MPQStorage.Data.Assets[SNOGroup.Monster][body.ActorData.MonsterSNO].Data;
            _mpqPowerCount = monsterData.SkillDeclarations.Count(e => e.SNOPower != -1);

            for (int i = 0; i < monsterData.SkillDeclarations.Length; i++)
            {
                if (monsterData.SkillDeclarations[i].SNOPower == -1) continue;
                if (PowerLoader.HasImplementationForPowerSNO(monsterData.SkillDeclarations[i].SNOPower))
                {
                    var cooldownTime = monsterData.MonsterSkillDeclarations[i].Timer / 10f;
                    PresetPowers.Add(monsterData.SkillDeclarations[i].SNOPower, new Cooldown { CooldownTimer = null, CooldownTime = cooldownTime });
                }
            }

            // Add melee attack if not present
            if (!monsterData.SkillDeclarations.Any(s => s.SNOPower == MELEE_ATTACK_SNO))
                PresetPowers.Add(MELEE_ATTACK_SNO, new Cooldown { CooldownTimer = null, CooldownTime = 0f });
        }

        public override void Think(int tickCounter)
        {
            // Skip special actors that shouldn't think
            if (ShouldSkipThink())
                return;

            // Exit early for common cases
            if (IsCrowdControlled())
                return;

            // Handle fear effect
            if (HandleFearEffect(tickCounter))
                return;

            _feared = false;

            // Only think every MonsterThinkTick per second (defaults 1 = 60 (once per second), 0.5 = 30 (twice per second))
            if (tickCounter % (60 * GameServerConfig.Instance.MonsterThinkTick) != 0)
                return;

            // Main combat logic
            if (CurrentAction == null)
            {
                EvaluateTargetsAndAct(tickCounter);
            }
        }

        private bool ShouldSkipThink()
        {
            switch (Body.SNO)
            {
                case ActorSno._uber_siegebreakerdemon:
                case ActorSno._a4dun_garden_corruption_monster:
                case ActorSno._a4dun_garden_hellportal_pillar:
                case ActorSno._belialvoiceover:
                    return true;
            }

            return Body is NPC || Body.Hidden || !Body.Visible || Body.Dead ||
                   Body.World.Game.Paused || Body.Attributes[GameAttributes.Disabled];
        }

        private bool IsCrowdControlled()
        {
            bool isCrowdControlled = Body.Attributes[GameAttributes.Frozen] ||
                                     Body.Attributes[GameAttributes.Stunned] ||
                                     Body.Attributes[GameAttributes.Blind] ||
                                     Body.Attributes[GameAttributes.Webbed] ||
                                     Body.Disable ||
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

        private bool HandleFearEffect(int tickCounter)
        {
            if (!Body.Attributes[GameAttributes.Feared])
                return false;

            if (!_feared || CurrentAction == null)
            {
                CancelCurrentAction();
                _feared = true;
                CurrentAction = new MoveToPointWithPathfindAction(
                    Body,
                    PowerContext.RandomDirection(Body.Position, FEARED_RETREAT_MIN, FEARED_RETREAT_MAX)
                );
            }

            return true;
        }

        private void EvaluateTargetsAndAct(int tickCounter)
        {
            _powerDelay ??= new SecondsTickTimer(Body.World.Game, POWER_DELAY_SECONDS);
            _targetUpdateDelay ??= new SecondsTickTimer(Body.World.Game, TARGET_UPDATE_DELAY_SECONDS);

            // Update target selection periodically
            if (_targetUpdateDelay.TimedOut)
            {
                _targetUpdateDelay = new SecondsTickTimer(Body.World.Game, TARGET_UPDATE_DELAY_SECONDS);
                UpdateTarget();
            }

            // Attack if power delay has expired
            if (_powerDelay.TimedOut)
            {
                _powerDelay = new SecondsTickTimer(Body.World.Game, POWER_DELAY_SECONDS);

                if (_target != null && !_target.Dead)
                {
                    ExecuteAttackOnTarget(tickCounter);
                }
                else if (Body.Position != Body.CheckPointPosition)
                {
                    // Return to spawn point if no target
                    CurrentAction = new MoveToPointWithPathfindAction(Body, Body.CheckPointPosition);
                }
            }
        }

        private void UpdateTarget()
        {
            // Handle priority target
            if (PriorityTarget != null && !PriorityTarget.Dead)
            {
                _target = PriorityTarget;
                return;
            }

            // Use AttackedBy as priority
            if (AttackedBy != null && !AttackedBy.Dead)
            {
                PriorityTarget = AttackedBy;
                _target = AttackedBy;
                return;
            }

            // Find new target
            var nearbyTargets = FindValidTargets();
            _target = nearbyTargets.FirstOrDefault();
        }

        private List<Actor> FindValidTargets()
        {
            var validTargets = new List<Actor>();

            if (Body.Attributes[GameAttributes.Team_Override] == 1)
            {
                // Team override - attack allies (for betrayal scenarios)
                validTargets.AddRange(
                    Body.GetObjectsInRange<Monster>(DEFAULT_SEARCH_RANGE)
                        .Where(p => !p.Dead)
                        .OrderBy(m => PowerMath.Distance2D(m.Position, Body.Position))
                );
            }
            else
            {
                // Normal targeting
                validTargets.AddRange(
                    Body.GetActorsInRange(DEFAULT_SEARCH_RANGE)
                        .Where(IsValidCombatTarget)
                        .OrderBy(a => PowerMath.Distance2D(a.Position, Body.Position))
                );
            }

            return validTargets;
        }

        private bool IsValidCombatTarget(Actor actor)
        {
            if (actor.Dead || actor == Body || actor.Hidden)
                return false;

            // Player targets
            if (actor is Player player)
            {
                return !player.Attributes[GameAttributes.Loading] &&
                       !player.Attributes[GameAttributes.Is_Helper] &&
                       player.World.BuffManager.GetFirstBuff<ActorGhostedBuff>(player) == null;
            }

            // Minion targets
            if (actor is Minion minion)
            {
                return !minion.Attributes[GameAttributes.Is_Helper];
            }

            // Hireling targets
            if (actor is Hireling)
            {
                return true;
            }

            // Destructible targets
            if (actor is DesctructibleLootContainer destructible)
            {
                return destructible.SNO.IsDoorOrBarricade();
            }

            return false;
        }

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
        }

        private float CalculateAttackRange(PowerScript power, int powerSNO)
        {
            float baseRange = Body.ActorData.Cylinder.Ax2;
            float powerRange = power.EvalTag(PowerKeys.AttackRadius);

            if (powerRange > 0f)
            {
                if (powerSNO == MELEE_ATTACK_SNO)
                    return baseRange + BASE_MELEE_RANGE;

                return baseRange + Math.Min(powerRange, MAX_ATTACK_RANGE);
            }

            return baseRange + MAX_ATTACK_RANGE;
        }

        private bool IsTargetInRange(float targetDistance, float attackRange)
        {
            return targetDistance < attackRange + _target.ActorData.Cylinder.Ax2;
        }

        private bool CanApproachTarget()
        {
            return Body.WalkSpeed != 0;
        }

        private void ExecutePowerAttack(int powerSNO, PowerScript power)
        {
            // Face the target
            if (Body.WalkSpeed != 0)
                Body.TranslateFacing(_target.Position, false);

            CurrentAction = new PowerAction(Body, powerSNO, _target);
            ApplyPowerCooldown(powerSNO, power);

            Logger.Trace($"{GetType().Name} {nameof(PowerAction)} on {_target.ActorType} at {_target.Position}");
        }

        private void ApproachTarget(int powerSNO, float attackRange)
        {
            // Special handling for ranged mobs
            if (Body.SNO.IsWoodwraithOrWasp())
            {
                CurrentAction = new MoveToPointAction(Body, _target.Position);
                Logger.Trace($"{GetType().Name} approaching target (ranged) at {_target.Position}");
            }
            else
            {
                CurrentAction = new MoveToTargetWithPathfindAction(
                    Body,
                    _target,
                    attackRange + _target.ActorData.Cylinder.Ax2,
                    powerSNO
                );
                Logger.Trace($"{GetType().Name} approaching target with pathfinding");
            }
        }

        private void ApplyPowerCooldown(int powerSNO, PowerScript power, float cooldownTime = 0f)
        {
            // Determine cooldown based on power type
            if (GameServerConfig.Instance.DisableMonsterPowerCooldowns)
                return;
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

            // Apply cooldown if needed
            if (cooldownTime > 0f)
            {
                PresetPowers[powerSNO] = new Cooldown
                {
                    CooldownTimer = new SecondsTickTimer(Body.World.Game, cooldownTime),
                    CooldownTime = cooldownTime
                };
            }
        }

        private bool IsSpecialPowerSNO(int powerSNO)
        {
            return powerSNO == 96925 || powerSNO == FIREWALL_SNO;
        }

        private void CancelCurrentAction()
        {
            if (CurrentAction != null)
            {
                CurrentAction.Cancel(0);
                CurrentAction = null;
            }
        }

        protected virtual int PickPowerToUse()
        {
            if (!_warnedNoPowers && PresetPowers.Count == 0)
            {
                Logger.Warn($"Monster $[red]$\"{Body.Name}\"$[/]$ has no usable powers. {_mpqPowerCount} are defined in mpq data.");
                _warnedNoPowers = true;
                return -1;
            }

            if (PresetPowers.Count <= 0)
                return -1;

            // Get available powers
            var availablePowers = PresetPowers
                .Where(p => p.Value.CooldownTimer == null || p.Value.CooldownTimer.TimedOut)
                .Where(p => PowerLoader.HasImplementationForPowerSNO(p.Key))
                .Select(p => p.Key)
                .ToList();

            // No power, let's use Chance to decide preference over non-melee attacks
            if (FastRandom.Instance.Chance(50))
            {
                // Prefer non-melee attacks
                if (availablePowers.Where(p => p != MELEE_ATTACK_SNO).TryPickRandom(out var selectedPower))
                    return selectedPower;
                else
                {
                    // Fall back to melee
                    if (availablePowers.Contains(MELEE_ATTACK_SNO))
                        return MELEE_ATTACK_SNO;
                }
            }
            else
            {
                // Melee default
                if (availablePowers.Contains(MELEE_ATTACK_SNO))
                    return MELEE_ATTACK_SNO;
            }
            return -1;
        }

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

        public void RemovePresetPower(int powerSNO)
        {
            if (PresetPowers.ContainsKey(powerSNO))
            {
                PresetPowers.Remove(powerSNO);
            }
        }

        public static Core.Types.Math.Vector3D RandomPossibleDirection(Core.Types.Math.Vector3D position, float minRadius, float maxRadius, MapSystem.World world)
        {
            float angle = (float)(FastRandom.Instance.NextDouble() * Math.PI * 2);
            float radius = minRadius + (float)FastRandom.Instance.NextDouble() * (maxRadius - minRadius);
            Core.Types.Math.Vector3D point = null;
            int attemptCount = 0;

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