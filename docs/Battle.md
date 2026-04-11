# Battle.md - DiIiS-NA Combat & Brain System Reference

> End-to-end documentation of the DiIiS-NA combat pipeline, monster AI (brains)
> and a practical "what to modify for a better balanced game" guide. All paths
> are given as `src/DiIiS-NA/...` relative to the repository root.

---

## Table of Contents

1. [High-level architecture](#1-high-level-architecture)
2. [Combat pipeline (end-to-end)](#2-combat-pipeline-end-to-end)
   - 2.1 [PowerManager — spell/skill execution](#21-powermanager--spellskill-execution)
   - 2.2 [AttackPayload — composing an attack](#22-attackpayload--composing-an-attack)
   - 2.3 [HitPayload — damage calculation](#23-hitpayload--damage-calculation)
   - 2.4 [DeathPayload — kill handling](#24-deathpayload--kill-handling)
   - 2.5 [BuffManager — buffs, debuffs, status effects](#25-buffmanager--buffs-debuffs-status-effects)
3. [Brain / AI system](#3-brain--ai-system)
   - 3.1 [Base `Brain`](#31-base-brain)
   - 3.2 [`MonsterBrain`](#32-monsterbrain)
   - 3.3 [`MinionBrain` / `HirelingBrain` / `LooterBrain`](#33-minionbrain--hirelingbrain--looterbrain)
   - 3.4 [`AggressiveNPCBrain` / `StayAggressiveNPCBrain` / `NPCBrain` / `FollowerBrain`](#34-aggressivenpcbrain--stayaggressivenpcbrain--npcbrain--followerbrain)
4. [Monster & minion stat scaling](#4-monster--minion-stat-scaling)
5. [Balance modification guide](#5-balance-modification-guide)
   - 5.1 [Damage input (player → monster)](#51-damage-input-player--monster)
   - 5.2 [Damage output (monster → player)](#52-damage-output-monster--player)
   - 5.3 [Defenses (armor, resistance, dodge, block)](#53-defenses-armor-resistance-dodge-block)
   - 5.4 [Critical hit tuning](#54-critical-hit-tuning)
   - 5.5 [Crowd control / status effects](#55-crowd-control--status-effects)
   - 5.6 [Monster AI responsiveness](#56-monster-ai-responsiveness)
   - 5.7 [Boss tuning](#57-boss-tuning)
   - 5.8 [Runtime knobs via `config.ini` / `GameServerConfig`](#58-runtime-knobs-via-configini--gameserverconfig)
6. [File index](#6-file-index)

---

## 1. High-level architecture

The combat system in DiIiS-NA is split across three orthogonal layers:

```
                    ┌───────────────────────────────────────┐
                    │          PLAYER / MONSTER             │
                    │  (intent: "cast skill X on target Y")│
                    └─────────────────┬─────────────────────┘
                                      │
                   ┌──────────────────▼──────────────────┐
                   │          PowerManager.cs            │  ← skill router /
                   │   RunPower()  →  _StartScript()     │    coroutine engine
                   └──────────────────┬──────────────────┘
                                      │
                   ┌──────────────────▼──────────────────┐
                   │     PowerScript (Implementations)   │  ← per-skill logic
                   │   e.g. Barbarian.cs, Wizard.cs,     │    yields payloads
                   │        BossSkills.cs, ...           │
                   └──────────────────┬──────────────────┘
                                      │ builds
                   ┌──────────────────▼──────────────────┐
                   │          AttackPayload.cs           │  ← DamageEntries,
                   │   AddWeaponDamage() / AddDamage()   │    target list,
                   │                 │                   │    crit roll
                   └─────────────────┬┴──────────────────┘
                                     │ per-target
                   ┌─────────────────▼───────────────────┐
                   │           HitPayload.cs             │  ← full damage
                   │  (element damage → armor → resist   │    calculation
                   │   → class passives → floating nums) │    and application
                   └─────────────────┬───────────────────┘
                                     │ if HP ≤ 0
                   ┌─────────────────▼───────────────────┐
                   │          DeathPayload.cs            │  ← XP, loot,
                   │                                     │    quest advance
                   └─────────────────────────────────────┘
```

On the monster side, the `Brain` hierarchy decides *when* to call the above
pipeline:

```
Game tick (60 Hz)
  │
  ▼
Monster.Update(tick)  ──►  Brain.Update(tick)
                               │
                               ├── Think(tick)   // pick action (target/power)
                               └── Perform(tick) // drive CurrentAction
                                                    │
                                                    ▼
                                              PowerAction
                                                    │
                                                    ▼
                                          PowerManager.RunPower()
                                                    │
                                                    ▼
                                            AttackPayload / HitPayload...
```

Key files (see [§6 File index](#6-file-index) for the full list):

| Concern            | File                                                             |
| ------------------ | ---------------------------------------------------------------- |
| Skill execution    | `GSSystem/PowerSystem/PowerManager.cs`                           |
| Attack composition | `GSSystem/PowerSystem/Payloads/AttackPayload.cs`                 |
| Damage calculation | `GSSystem/PowerSystem/Payloads/HitPayload.cs`                    |
| Death handling     | `GSSystem/PowerSystem/Payloads/DeathPayload.cs`                  |
| Buffs / debuffs    | `GSSystem/PowerSystem/BuffManager.cs`                            |
| Base brain         | `GSSystem/AISystem/Brain.cs`                                     |
| Monster AI         | `GSSystem/AISystem/Brains/MonsterBrain.cs`                       |
| Monster stats      | `GSSystem/ActorSystem/Monster.cs`                                |
| Minion stats       | `GSSystem/ActorSystem/Minion.cs`                                 |
| Runtime knobs      | `D3-GameServer/GameServerConfig.cs` (+ top-level `config.ini`)   |

---

## 2. Combat pipeline (end-to-end)

### 2.1 PowerManager — spell/skill execution

File: `src/DiIiS-NA/D3-GameServer/GSSystem/PowerSystem/PowerManager.cs`

`PowerManager` is the per-world owner of every currently-executing power
coroutine and every open channeled skill. It is updated once per game tick by
the world and drives `PowerScript.Run()` enumerators forward using
`TickTimer`s.

The public entry point is:

```csharp
public bool RunPower(
    Actor user,
    PowerScript power,
    Actor target = null,
    Vector3D targetPosition = null,
    TargetMessage targetMessage = null);
```

Internally it does roughly:

1. Validate the cast (not disabled, line-of-sight for teleport, etc.).
2. Break CC if the power has a `BreaksStun` / `BreaksFear` / `BreaksRoot` tag
   (see `PowerManager.RunPower` around `PowerKeys.BreaksStun`).
3. Attach context (`User`, `Target`, `TargetPosition`) to the `PowerScript`.
4. Sanity-check attack-speed cheats (`user.LastSecondCasts` vs.
   `Attacks_Per_Second_Total`).
5. Start the coroutine via `_StartScript`, which is then ticked every frame by
   `_UpdateExecutingScripts`.

Channeled skills are held in `_channeledSkills` so a second cast of the same
channel replaces (and ticks) the existing instance instead of spawning a new
one.

`PowerManager` also owns `_deletingActors` — a short 10-second purgatory for
actors that died with lingering visual buffs, preventing effects from
orphaning.

### 2.2 AttackPayload — composing an attack

File: `src/DiIiS-NA/D3-GameServer/GSSystem/PowerSystem/Payloads/AttackPayload.cs`

A `PowerScript` builds an `AttackPayload` when it wants to deal damage. Its
responsibility is just composition, not calculation:

- `AddDamage(minDamage, delta, DamageType)` — flat damage, not weapon scaled.
- `AddWeaponDamage(multiplier, DamageType)` — multiplier of `Damage_Weapon_*`
  attributes (e.g. `Barbarian/Bash` adds weapon damage at `3.45f`).
- `SetSingleTarget(Actor)` / `Targets = TargetList(...)` — target list.
- `AddBuffOnHit<T>()` — schedule a buff (applied even if damage is 0).
- `AutomaticHitEffects` — disable hit sounds/effects/procs when a single
  attack logically generates multiple `HitPayload`s (e.g. chained bounces),
  to prevent double-dip procs.
- `OnHit` / `OnDeath` — coroutine hooks.

Calling `Apply()` iterates every `Targets.Actors` and for each one:

```csharp
var payload = new HitPayload(this, _DoCriticalHit(user, target, chcBonus), target);
OnHit?.Invoke(payload);  // hook
payload.Apply();         // actually deal the damage
```

`_DoCriticalHit` is the **single** place the crit roll happens, and it
already respects:

- `Weapon_Crit_Chance` (base from gear + class),
- `Crit_Percent_Bonus_Capped` / `Crit_Percent_Bonus_Uncapped`,
- `Power_Crit_Percent_Bonus[powerSNO]` (per-skill bonus),
- `Bonus_Chance_To_Be_Crit_Hit` (debuff on the target),
- `Ignores_Critical_Hits` (bosses immune).

**Crit chance is hard-capped at 85 %** (`if (totalCritChance > 0.85f) totalCritChance = 0.85f;`).

### 2.3 HitPayload — damage calculation

File: `src/DiIiS-NA/D3-GameServer/GSSystem/PowerSystem/Payloads/HitPayload.cs`

This is the single most important file for balance. It is the canonical
damage pipeline for every hit in the game.

The constructor runs the **entire pre-application** of damage; `Apply()` only
handles dodge/block, floating numbers, HP subtraction, death branching and
hit effects.

#### Damage formula (per element)

For each `DamageEntry` in the attack, the resulting damage on the target is:

```text
damage_e  =  (minDmg  +  rand[0..1) * delta)                  ← roll
           *  (1 + Damage_Type_Percent_Bonus[e]
                  + Damage_Dealt_Percent_Bonus[e])            ← attacker %e
           *  (1 - Damage_Percent_Reduction_From_Type[e]
                  + Amplify_Damage_Type_Percent[e])           ← target %e
           *  ReductionFromResistance(resist_e, attackerLvl)  ← resistance
           *  (0 if Immunity[e]  else 1)                      ← immunity gate
```

For **weapon-based** damage entries the roll is:

```text
damage_e =  WeaponDamageMultiplier
         *  ( Damage_Weapon_Min_Total[0]                ← white min
            + Damage_Weapon_Min_Total[e]                ← +e min
            + rand * (Damage_Weapon_Delta_Total[0]
                    + Damage_Weapon_Delta_Total[e]) )   ← delta
```

Summed over elements, then **global multipliers**:

```text
total = Σ damage_e
total *= (1 + Crit_Damage_Percent)       if critical
total *= ReductionFromArmor(armor, lvl)
total *= (1 - Damage_Done_Reduction_Percent)
total *= (1 + Power_Damage_Percent_Bonus[powerSNO])
total *= (distance < 6f
          ? (1 - Damage_Percent_Reduction_From_Melee)
          : (1 - Damage_Percent_Reduction_From_Ranged))
total *= (1 + PrimaryAttribute / 100f)   (players, weapon damage only)
total *= (1 + Damage_Percent_Bonus_Vs_Monster_Type[t])  (vs. type)
total *= (1 + Damage_Percent_Bonus_Vs_Elites)           (vs. elites)
```

Plus a large cascade of class-specific passive hooks (Wizard `GlassCannon`,
Barb `Brawler`, DH `SteadyAim`, Monk `Resolve`, Crusader `HolyCause` and so
on) — search `HitPayload.cs` for `HasPassive(` to enumerate them.

The two helpers at the bottom of the file are:

```csharp
private static float ReductionFromResistance(float resist, int lvl)
    => 1f - (resist / ((5 * lvl) + resist));

private static float ReductionFromArmor(float armor, int lvl)
    => 1f - (armor / ((50 * lvl) + armor));
```

These are the canonical "diminishing returns" curves — changing the `5` or
`50` constants globally rescales the whole defensive economy.

> ⚠️ `HitPayload.cs` currently hard-codes `TotalDamage *= 0.1f;` for both
> **Player** and **Minion-owned-by-player** branches near line 553/569. This
> is an *"unkillable" safety hack* and is the single largest balance dial in
> the file: it is what keeps players alive against scaled monster damage.
> If you rebalance monster damage, re-evaluate these two lines first.

#### Apply() stage

1. Cancel immediately if the target is invulnerable/immune (broadcast an
   `Immune` floating number).
2. Roll dodge (player only) using `Player.DodgeChance`; if dodged, no damage
   and `DebuffFeared`/trigger `Awareness` passive.
3. Roll block using `Block_Chance_Capped_Total`, clamp `TotalDamage >= 0`.
4. Apply life-on-hit / life-steal / resource-on-crit procs.
5. Broadcast the `FloatingNumberMessage` (colour = red crit, gold crit, red,
   white).
6. `target.Hitpoints_Cur -= total; BroadcastChangedIfRevealed();`
7. If HP drops to 0, build a `DeathPayload` and apply it.
8. Otherwise, play a hit animation (33 % chance) and briefly freeze the
   target's walk speed for a knockback-feel.

### 2.4 DeathPayload — kill handling

File: `src/DiIiS-NA/D3-GameServer/GSSystem/PowerSystem/Payloads/DeathPayload.cs`

The constructor checks cheat-death passives first (`SpiritVessel` for WD,
`NearDeathExperience` for Monk) and clamps HP back up if either is available.

`Apply()` performs:

- XP grant and floating XP number.
- Gold drop (`LootManager.DropRandomGold`).
- Loot table rolls (`LootManager.DropRandomItem`).
- Quest advance triggers.
- Corpse spawn for Necromancer abilities.
- Achievement counters (kill counters, elite-in-X-seconds, etc.).
- `Monster.PlayLore()` for the first kill of a new monster type.

Because this is where XP/gold/loot actually hit the player, this is also the
right place to add global event multipliers (double-XP weekends, etc.).

### 2.5 BuffManager — buffs, debuffs, status effects

File: `src/DiIiS-NA/D3-GameServer/GSSystem/PowerSystem/BuffManager.cs`

`BuffManager` is per-`World`. Buffs are stored in a
`Dictionary<Actor, List<Buff>>` keyed by target.

- `AddBuff(user, target, buff)` — attaches, auto-sets `PowerSNO` via the
  `ImplementsPowerSNO` attribute, calls `buff.Init()` then `buff.Apply()`.
  If a buff of the same type already exists and is stackable
  (`buff.Stack(buff)` returns `true`), the new instance stacks into the old
  one instead of being added.
- `Update()` — called per tick; iterates every actor and calls `buff.Update()`
  (the buff's tick), then removes entries whose `Update()` returned `true`.
- `SendTargetPayload(target, payload)` — fires `OnPayload` for every buff on
  `target` when it is hit (how e.g. `ThornsBuff`, `ParryBuff`, `InnerSanctuary`
  react to incoming damage).
- `RemoveAllBuffs(target, removeCooldowns)` — used on resurrect / zone change.
  Passing `removeCooldowns = false` preserves the cooldown buffs (PowerSNO
  `30176`) so skills don't suddenly become ready again.

Crowd-control effects (stun, freeze, slow, knockback, root) are **regular
buffs** that set a `GameAttribute` flag on their target; there is no separate
CC system. Read `StatusDebuff.cs` for the generic debuff base class.

---

## 3. Brain / AI system

### 3.1 Base `Brain`

File: `src/DiIiS-NA/D3-GameServer/GSSystem/AISystem/Brain.cs`

Every living actor (monster, minion, hireling, NPC) owns exactly one `Brain`.
The base class is intentionally minimal — it implements a two-phase update:

```csharp
public virtual void Update(int tick)
{
    if (State == Dead || State == Off || Body?.World == null) return;
    Think(tick);    // subclass decides what to do
    Perform(tick);  // drives the CurrentAction
}
```

- `Think(tick)` — abstract AI policy. Subclasses override this to read the
  world and assign `CurrentAction`.
- `Perform(tick)` — starts/ticks the `CurrentAction`, clears it when `Done`.
- `Kill()` — cancel current action and set state to `Dead`.
- `Activate()` / `DeActivate()` — for off-screen or quest-gated monsters.

`BrainState` (file: `BrainState.cs`) has 8 values: `Idle`, `Wander`, `Combat`,
`Follow`, `Guard`, `Dead`, `Off`, `End`. Note that **MonsterBrain does not
actively drive this enum** — it uses `CurrentAction == null` as its implicit
state. The enum mostly matters for NPCs and `Kill()`/`DeActivate()`.

### 3.2 `MonsterBrain`

File: `src/DiIiS-NA/D3-GameServer/GSSystem/AISystem/Brains/MonsterBrain.cs`

This is the AI for every regular monster, champion, rare and boss. Its
`Think()` runs the following decision chain (top-to-bottom, short-circuits):

1. **`ShouldSkipThink()`** — skip per-tick thinking for hard-coded special
   actors (e.g. `_uber_siegebreakerdemon`, garden pillars, Belial voice
   actor), paused games, NPC targets, hidden/dead bodies.
2. **`IsCrowdControlled()`** — if the body has `Frozen`, `Stunned`, `Blind`,
   `Webbed`, `Disable`, `KnockbackBuff` or `SummonedBuff`, cancel the current
   action and bail out. This is *also* what makes summoned monsters stand
   still at spawn — they have a `SummonedBuff` during their intro animation.
3. **`HandleFearEffect()`** — on `Feared`, run in a random direction between
   3 and 8 tiles (`FEARED_RETREAT_MIN/MAX`).
4. **Think throttle** — only enter the combat block every
   `60 * GameServerConfig.MonsterThinkTick` ticks. Default `MonsterThinkTick=1`
   ⇒ once per second. Smaller values make monsters react faster but cost CPU.
5. **`EvaluateTargetsAndAct()`**:
   - Every `TARGET_UPDATE_DELAY_SECONDS` (2 s), rebuild the target.
   - Every `POWER_DELAY_SECONDS` (1 s), if we have a live target, attack it;
     otherwise walk back to `CheckPointPosition` (spawn point).
6. **`UpdateTarget()`** — priority order is:
   1. `PriorityTarget` (scripted boss focus).
   2. `AttackedBy` (retaliate against whoever hit you last).
   3. Nearest valid target in `DEFAULT_SEARCH_RANGE = 50`.
7. **`IsValidCombatTarget()`** — filters out dead/hidden actors, ghost
   players, helper minions, non-door destructibles, etc. Respects
   `Team_Override` for mind-controlled (betrayed) monsters who attack their
   former allies.
8. **`ExecuteAttackOnTarget()`**:
   - `PickPowerToUse()` chooses a random available preset power, with a
     50 % bias towards non-melee when available.
   - Power range is computed via `CalculateAttackRange` using
     `PowerKeys.AttackRadius` + the body's collision cylinder, capped at
     `MAX_ATTACK_RANGE = 35`.
   - If in range, face the target and queue a `PowerAction(body, powerSNO,
     target)`.
   - Otherwise, queue `MoveToTargetWithPathfindAction` (or
     `MoveToPointAction` for woodwraith / wasp ranged mobs).
9. **`ApplyPowerCooldown()`** — every successful cast applies a per-power
   cooldown pulled from the monster MPQ `SkillDeclarations.Timer` (divided
   by 10), or a default based on category:
   - Summoning skills: `15s` (boss) / `7s` (normal).
   - Monster affix skills: `MonsterAffixSkill.CooldownTime`.
   - "Special" powers (`96925`, firewall `223284`): 10 s.
   - All cooldowns can be globally disabled via
     `GameServerConfig.DisableMonsterPowerCooldowns` — useful for stress
     tests, **dangerous in live play**.

Preset powers are loaded from the monster's MPQ `SkillDeclarations`. If the
monster has no melee power, a synthetic "basic melee" entry
(`MELEE_ATTACK_SNO = 30592`) with zero cooldown is added so the monster is
never completely silent.

### 3.3 `MinionBrain` / `HirelingBrain` / `LooterBrain`

These three brains share the same "master-centric" pattern — they wrap a
player and split their time between *following* and *attacking the player's
target*.

- **`MinionBrain.cs`** — used by summons that fight (Barb ancients, WD
  gargantuan, Necro skeletons, etc.). Almost identical to `MonsterBrain`
  but with a 40-tile leash to the master, and a "wander close to master"
  fallback when no targets are nearby. Summoning skills get a cooldown of
  `7s * CooldownReduction` (see also `Minion.CooldownReduction` on
  `ActorSystem/Minion.cs`).
- **`HirelingBrain.cs`** — used by Templar, Scoundrel, Enchantress, Leah.
  Only owns one hard-coded "preset power" picked in the constructor:
  `99902` (ranged projectile) for Scoundrel/Leah, `30273` (magic missile)
  for Enchantress, `30592` (melee) for Templar. This is why hirelings only
  spam a single attack — extend the constructor to add variety.
- **`LooterBrain.cs`** — used by Necromancer `Command Skeletons` with the
  loot rune, Treasure Goblins, etc. Its `Update()` override picks up gold
  / blood shards / uniques (if `LootLegendaries`) within a 5-tile radius of
  the body, then its `Think()` walks toward the nearest lootable within 40
  tiles. It does not attack.

### 3.4 `AggressiveNPCBrain` / `StayAggressiveNPCBrain` / `NPCBrain` / `FollowerBrain`

- **`AggressiveNPCBrain.cs`** — used for "friendly-but-fighting" NPCs like
  guards during scripted battles. Attacks any monster in 40 tiles, falls
  back to walking to `CheckPointPosition` when nothing is in range.
- **`StayAggressiveNPCBrain.cs`** — identical but never moves towards
  targets; only attacks if something wanders within range. Use for
  stationary archers / turrets.
- **`NPCBrain.cs`** — empty `Think()`. Shopkeepers, quest givers.
- **`FollowerBrain.cs`** — empty `Think()`. The class exists for type
  discrimination only; actual following is driven externally.

---

## 4. Monster & minion stat scaling

File: `src/DiIiS-NA/D3-GameServer/GSSystem/ActorSystem/Monster.cs`
File: `src/DiIiS-NA/D3-GameServer/GSSystem/ActorSystem/Minion.cs`

### Monster HP / damage (`Monster.UpdateStats`)

The formula used at spawn is (simplified):

```text
level       = connectedPlayers > 1 ? firstPlayerLevel : Game.InitialMonsterLevel
baseHP      = MonsterLevel[level].HPMin + rand(0, HPDelta) * HpMultiplier * Game.HpModifier

Hitpoints_Max = baseHP
Hitpoints_Max_Percent_Bonus_Multiplicative
            = (connectedPlayers + 1) * 1.5 * GameServerConfig.RateMonsterHP

Damage_Weapon_Min[0] = MonsterLevel[level].Dmg
                     * DmgMultiplier
                     * Game.DmgModifier
                     * GameServerConfig.RateMonsterDMG
```

The `MonsterLevel` table lives in the `GameBalance` MPQ file at SNO `19760`
(`monsterLevels.MonsterLevel[level]`). The two per-monster multipliers
`HpMultiplier` / `DmgMultiplier` come from the monster's own MPQ
`AttributeModifiers[4]` (HP) and `AttributeModifiers[55]` (damage).

On reveal, `Monster.Reveal()` rescales stats for co-op:

```text
Damage_Weapon_Min[0] = nativeDmg * (1 + 0.05 * (count - 1) * difficulty)
Hitpoints_Max        = nativeHp  * (1 + (0.75 + 0.1 * difficulty) * (count - 1))
```

So each extra player adds **+5 % damage per difficulty step** and **+75 % HP
+ 10 % per difficulty step** per additional player.

### Minion HP / damage (`Minion` constructor)

```text
Hitpoints_Max = 1000
              + Level * 150
              + Alt_Level * 150
              + 0.35 * master.Hitpoints_Max_Total
```

Minions inherit the master's `Weapon_Crit_Chance`, `Crit_Damage_Percent`,
`Damage_Weapon_Min_Total[0]` and `Damage_Weapon_Delta_Total[0]`. Their
`DamageCoefficient` (default `1f`) scales their weapon damage contribution
in `HitPayload.cs` — it's the knob you want for "pet builds."

---

## 5. Balance modification guide

This is the "where do I change X to get Y" cheat sheet. Each subsection is
ordered from *largest effect* to *smallest effect*.

### 5.1 Damage input (player → monster)

| Goal                                            | File → line                                                                 |
| ----------------------------------------------- | --------------------------------------------------------------------------- |
| Globally up/down all weapon damage              | Edit `HitPayload.cs`, the `case Player:` weapon branch (line ~83)           |
| Tune primary-stat scaling (str/dex/int)         | `HitPayload.cs` `TotalDamage *= 1 + (plr.PrimaryAttribute / 100f)` (~l. 173)|
| Buff/nerf "vs. elites" bonus                    | `HitPayload.cs` `Damage_Percent_Bonus_Vs_Elites` block (~l. 382)            |
| Per-class passives (GlassCannon, Resolve, ...)  | `HitPayload.cs` `HasPassive(SNO)` multipliers                               |
| Tune a single skill in isolation                | Edit the skill file in `PowerSystem/Implementations/HeroSkills/*.cs` and look for `AddWeaponDamage(...)` |
| Per-skill damage bonus                          | Attribute `Power_Damage_Percent_Bonus[powerSNO]` (set from items/passives)  |

### 5.2 Damage output (monster → player)

| Goal                                             | File → line                                                                                  |
| ------------------------------------------------ | -------------------------------------------------------------------------------------------- |
| Globally reduce monster damage                   | `config.ini` → `RateMonsterDMG`  (default `1.2`)                                             |
| Remove the "players take 10 % damage" safety     | `HitPayload.cs` — the two `TotalDamage *= 0.1f;` lines (~553 and ~569). **Read §2.3 first.** |
| Per-class damage reduction (Monk/Barb/Crus -30%) | `HitPayload.cs` "defensive passives" block: `TotalDamage *= 0.7f;` (~l. 456/475/531)         |
| Per-player scaling in multiplayer                | `Monster.Reveal()` — the `0.05f * (count - 1) * difficulty` constant                         |
| Boss damage multiplier                           | `GameServerConfig.BossDamageMultiplier` (default `3f`) *(consumed wherever `Boss` stats are adjusted — grep for `BossDamageMultiplier`)* |

### 5.3 Defenses (armor, resistance, dodge, block)

| Goal                                        | File → line                                                                                |
| ------------------------------------------- | ------------------------------------------------------------------------------------------ |
| Rescale whole armor curve                   | `HitPayload.ReductionFromArmor` — constant `50` (line ~577)                                |
| Rescale whole resistance curve              | `HitPayload.ReductionFromResistance` — constant `5` (line ~575)                            |
| Dodge chance formula                        | `Player.DodgeChance` (grep — lives in `PlayerSystem/Player.cs`)                            |
| Block chance / block amount                 | `HitPayload.Apply()` block using `Block_Chance_Capped_Total`, `Block_Amount_Total_Min/Max` |
| Make bosses immune to crits                 | Set `GameAttribute.Ignores_Critical_Hits = true` on the actor                              |
| Make an actor immune to a damage type       | `GameAttribute.Immunity[damageTypeIndex] = true`                                           |
| Damage reduction vs melee vs ranged         | `GameAttribute.Damage_Percent_Reduction_From_Melee` / `_Ranged` (cut at `distance < 6f`)   |

### 5.4 Critical hit tuning

| Goal                                 | File → line                                                                        |
| ------------------------------------ | ---------------------------------------------------------------------------------- |
| Hard cap on crit chance              | `AttackPayload._DoCriticalHit` — `if (totalCritChance > 0.85f)` (~l. 167)          |
| Crit damage bonus application        | `HitPayload` — `TotalDamage *= (1 + Crit_Damage_Percent)` (~l. 143)                |
| Per-power crit bonus                 | `GameAttribute.Power_Crit_Percent_Bonus[powerSNO]`                                 |
| Target "take extra crit" debuffs     | `GameAttribute.Bonus_Chance_To_Be_Crit_Hit`                                        |

### 5.5 Crowd control / status effects

All CC in DiIiS-NA is implemented as a `Buff` that sets a `GameAttribute`
flag on its target. The actual *effect* of the flag is handled inside each
brain's `IsCrowdControlled()` equivalent:

- **Movement blockers** (`Frozen`, `Stunned`, `Blind`, `Webbed`,
  `KnockbackBuff`, `SummonedBuff`) — cancel `CurrentAction` in every brain
  and skip `Think()`.
- **Fear** — the brain runs away for the duration (`FEARED_RETREAT_MIN/MAX`
  in `MonsterBrain`, `3f, 8f` in `MinionBrain`/`HirelingBrain`).
- **Disable** — the actor is an object (pillar, portal) that should not
  think at all.

To tune CC, search for:

- `DebuffStunned` / `DebuffFrozen` / `DebuffSlowed` / `DebuffFeared` /
  `DebuffChilled` / `DebuffBlind` in
  `PowerSystem/Implementations/General/*.cs` — these are the generic status
  debuff classes.
- The `On_Hit_*_Proc_Chance` block in `HitPayload.cs` (~l. 389 onwards) —
  this is the single place where on-hit CC procs are rolled. Tuning
  `procCoeff` globally up or down adjusts how often these CCs fire.

### 5.6 Monster AI responsiveness

| Goal                                             | File → line                                                                     |
| ------------------------------------------------ | ------------------------------------------------------------------------------- |
| How often monsters think                         | `GameServerConfig.MonsterThinkTick` (default `1` → once/sec)                    |
| How often a monster retargets                    | `MonsterBrain.TARGET_UPDATE_DELAY_SECONDS = 2.0f`                                |
| Cooldown between attacks                         | `MonsterBrain.POWER_DELAY_SECONDS = 1.0f`                                       |
| Aggro / search range                             | `MonsterBrain.DEFAULT_SEARCH_RANGE = 50`                                        |
| Melee range padding                              | `MonsterBrain.BASE_MELEE_RANGE = 10f`                                           |
| Max attack range cap                             | `MonsterBrain.MAX_ATTACK_RANGE = 35f`                                           |
| Disable all monster power cooldowns (stress)     | `GameServerConfig.DisableMonsterPowerCooldowns = true`                          |
| 50/50 melee vs ranged bias                       | `MonsterBrain.PickPowerToUse` — `FastRandom.Chance(50)`                         |

### 5.7 Boss tuning

Bosses derive from `Monster` via the `Boss` class but use the same
`MonsterBrain`. The difference is:

- `GameServerConfig.BossHealthMultiplier` (default `3f`)
- `GameServerConfig.BossDamageMultiplier` (default `3f`)
- Boss summoning cooldown is 15 s (vs 7 s for normal) —
  `MonsterBrain.SUMMONING_COOLDOWN_BOSS`.
- A boss's scripted phases live in `PowerSystem/Implementations/MonsterSkills/BossSkills.cs`
  — each boss phase is a separate `PowerScript`.
- `PriorityTarget` on the brain lets a scripted phase lock the boss onto a
  specific player.

### 5.8 Runtime knobs via `config.ini` / `GameServerConfig`

These are all hot-tunable without a recompile — they are read from
`config.ini` on startup (top-level `src/DiIiS-NA/config.ini`) and can be
overridden by editing the file.

| Setting                        | Default | Effect                                                          |
| ------------------------------ | ------- | --------------------------------------------------------------- |
| `RateExp`                      | `1`     | Multiplier on XP gain.                                          |
| `RateMoney`                    | `1`     | Multiplier on gold drops.                                       |
| `RateDrop`                     | `1`     | Multiplier on item drop rate.                                   |
| `RateChangeDrop`               | `1`     | Multiplier on drop *quality* rolls.                             |
| `RateMonsterHP`                | `1`     | Multiplier on all monster HP.                                   |
| `RateMonsterDMG`               | `1.2`   | Multiplier on all monster damage.                               |
| `BossHealthMultiplier`         | `3`     | Extra HP multiplier applied to bosses on top of `RateMonsterHP`.|
| `BossDamageMultiplier`         | `3`     | Extra damage multiplier applied to bosses.                      |
| `MonsterThinkTick`             | `1`     | Seconds between AI `Think()` steps (smaller = snappier, more CPU).|
| `DisableMonsterPowerCooldowns` | `false` | Removes cooldowns on monster powers (stress-test only).         |
| `DistanceOnPlayerApproaching`  | `3`     | Follow tolerance distance for NPC brains.                       |
| `HealthPotionConsumable`       | `true`  | Potion uses an inventory item vs. cooldown-only.                |
| `HealthPotionRestorePercentage`| `60`    | Percentage of max HP restored.                                  |
| `HealthPotionCooldown`         | `30`    | Seconds between potion uses.                                    |
| `StrengthMultiplier`           | `1`     | Pre-paragon scaling of Strength.                                |
| `StrengthParagonMultiplier`    | `1`     | Paragon scaling of Strength.                                    |
| `DexterityMultiplier`          | `1`     | (same, dex)                                                     |
| `IntelligenceMultiplier`       | `1`     | (same, int)                                                     |
| `VitalityMultiplier`           | `1`     | (same, vit — affects HP since `HP ∝ vit`)                       |
| `NephalemRiftProgressMultiplier` | `1`   | XP-like multiplier for NR progress bar.                         |
| `NephalemRiftOrbsChance`       | `0`     | Chance per kill to drop a progress orb.                         |
| `ChanceHighQualityUnidentified`| `30`    | % chance a rare/set/legendary drops unidentified.               |
| `IdentifyInSeconds`            | `5`     | Identify cast time.                                             |
| `ResurrectionCharges`          | `3`     | Charges granted on zone change.                                 |

**Recommended starting recipe** for a "difficulty-up, grind-up" server:

```ini
RateExp = 2
RateMoney = 2
RateDrop = 1.5
RateMonsterHP = 1.5
RateMonsterDMG = 1.5
BossHealthMultiplier = 4
BossDamageMultiplier = 3.5
MonsterThinkTick = 0.5
```

> If monsters start hitting unreasonably hard, the dominant cap you will
> hit is the `*= 0.1f` "unkillable hack" in `HitPayload.cs` (§2.3). Raising
> it to `0.15f` or `0.20f` is the right move before you touch resistances.

---

## 6. File index

### Combat

| Path (`src/DiIiS-NA/...`)                                                | Purpose                                   |
| ------------------------------------------------------------------------ | ----------------------------------------- |
| `D3-GameServer/GSSystem/PowerSystem/PowerManager.cs`                     | Skill execution router & coroutine engine |
| `D3-GameServer/GSSystem/PowerSystem/Payloads/Payload.cs`                 | `Payload` base class                      |
| `D3-GameServer/GSSystem/PowerSystem/Payloads/AttackPayload.cs`           | Damage entry composition + crit roll      |
| `D3-GameServer/GSSystem/PowerSystem/Payloads/HitPayload.cs`              | **Single source of damage calculation**   |
| `D3-GameServer/GSSystem/PowerSystem/Payloads/DeathPayload.cs`            | Death, XP, loot, quest advance            |
| `D3-GameServer/GSSystem/PowerSystem/BuffManager.cs`                      | Buff/debuff lifecycle                     |
| `D3-GameServer/GSSystem/PowerSystem/BaseBuffs.cs`                        | Abstract `Buff`                           |
| `D3-GameServer/GSSystem/PowerSystem/Implementations/General/StatusDebuff.cs` | Generic CC debuff (stun/freeze/slow/…) |
| `D3-GameServer/GSSystem/PowerSystem/Implementations/General/KnockbackBuff.cs` | Knockback impl                        |
| `D3-GameServer/GSSystem/PowerSystem/Implementations/HeroSkills/*.cs`     | Per-class skill damage numbers            |
| `D3-GameServer/GSSystem/PowerSystem/Implementations/MonsterSkills/*.cs`  | Monster skill implementations             |
| `D3-GameServer/GSSystem/PowerSystem/DamageType.cs`                       | Damage type table                         |
| `D3-GameServer/MessageSystem/GameAttribute.List.cs`                      | All GameAttribute definitions             |

### Actors

| Path (`src/DiIiS-NA/...`)                                    | Purpose                              |
| ------------------------------------------------------------ | ------------------------------------ |
| `D3-GameServer/GSSystem/ActorSystem/Actor.cs`                | Base actor (HP/attrs/buffs)          |
| `D3-GameServer/GSSystem/ActorSystem/Living.cs`               | Living base                          |
| `D3-GameServer/GSSystem/ActorSystem/Monster.cs`              | Monster HP/damage scaling            |
| `D3-GameServer/GSSystem/ActorSystem/Minion.cs`               | Summoned minion scaling              |
| `D3-GameServer/GSSystem/ActorSystem/Actions/PowerAction.cs`  | Action wrapping `PowerManager.RunPower` |
| `D3-GameServer/GSSystem/PlayerSystem/Player.cs`              | Player stats/skills                  |

### AI / Brains

| Path (`src/DiIiS-NA/...`)                                          | Purpose                              |
| ------------------------------------------------------------------ | ------------------------------------ |
| `D3-GameServer/GSSystem/AISystem/Brain.cs`                         | Base brain (Update/Think/Perform)    |
| `D3-GameServer/GSSystem/AISystem/BrainState.cs`                    | Brain state enum                     |
| `D3-GameServer/GSSystem/AISystem/Brains/MonsterBrain.cs`           | Main monster AI                      |
| `D3-GameServer/GSSystem/AISystem/Brains/MinionBrain.cs`            | Summoned minion AI                   |
| `D3-GameServer/GSSystem/AISystem/Brains/HirelingBrain.cs`          | Templar/Scoundrel/Enchantress        |
| `D3-GameServer/GSSystem/AISystem/Brains/LooterBrain.cs`            | Pet loot-picker                      |
| `D3-GameServer/GSSystem/AISystem/Brains/AggressiveNPCBrain.cs`     | Friendly fighting NPC                |
| `D3-GameServer/GSSystem/AISystem/Brains/StayAggressiveNPCBrain.cs` | Stationary fighting NPC              |
| `D3-GameServer/GSSystem/AISystem/Brains/NPCBrain.cs`               | Passive NPC                          |
| `D3-GameServer/GSSystem/AISystem/Brains/FollowerBrain.cs`          | Passive follower base                |

### Balance / configuration

| Path (`src/DiIiS-NA/...`)                                              | Purpose                          |
| ---------------------------------------------------------------------- | -------------------------------- |
| `D3-GameServer/GameServerConfig.cs`                                    | All runtime-tunable rates        |
| `config.ini` (top level next to the exe)                               | Actual values                    |
| `Core/MPQ/FileFormats/GameBalance.cs`                                  | MonsterLevel table, loot weights |
| `Core/MPQ/FileFormats/Monster.cs`                                      | Per-monster HP/damage modifiers  |
| `Core/MPQ/FileFormats/Power.cs`                                        | Power TagMap coefficients        |

---

## Appendix: Damage pipeline quick-reference

```
Player casts X
    │
    ▼
PowerManager.RunPower(X)
    │   validates cast, breaks CC if power has BreakStun,
    │   attaches user/target/position, starts coroutine
    ▼
PowerScript.Run()   ← in HeroSkills/<class>.cs
    │   yields AttackPayload(s) in a coroutine
    ▼
AttackPayload.Apply()
    │   _DoCriticalHit()   ← single crit roll point (85% cap)
    │   for each target → new HitPayload(...)
    ▼
HitPayload (constructor)
    │   roll element damage
    │   ×(1 + type bonuses)  ×(1 − type reduction)
    │   ×resist reduction    ×(immunity ? 0 : 1)
    │   sum → TotalDamage
    │   ×(1 + crit dmg)  ×armor reduction
    │   ×power bonus    ×melee/ranged reduction
    │   ×primary stat   ×vs.type/elite
    │   ×class passives (dozens)
    │   ×0.1f (SAFETY HACK for players/minions)
    ▼
HitPayload.Apply()
    │   invuln/immunity gate
    │   dodge roll → early out
    │   block roll → subtract block amount
    │   life on hit / life steal / resource on crit
    │   broadcast floating number
    │   Hitpoints_Cur -= total
    │   if ≤ 0 → DeathPayload.Apply()
    │   else → play hit anim
    ▼
DeathPayload.Apply()
    │   cheat-death passives
    │   XP / gold / loot / quest advance
    │   corpse spawn / achievements
    ▼
done
```
