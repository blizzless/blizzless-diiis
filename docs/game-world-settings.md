# Game World Settings

The parameters of the world can be easily altered using the configuration file located within `config.ini`.

## Configuration

The parameters specified in the `config.ini` file will be saved to the server folder, overwriting the default settings. For example, all values below use their default settings.

```ini
[Game-Server]
; rates
RateExp = 1
RateMoney = 1
RateDrop = 1
RateChangeDrop = 1
RateMonsterHP = 1
RateMonsterDMG = 1
; items
ChanceHighQualityUnidentified = 30
ChanceNormalUnidentified = 5
; bosses
BossHealthMultiplier = 6
BossDamageMultiplier = 3
; nephalem
NephalemRiftProgressMultiplier = 1
NephalemRiftAutoFinish = false
NephalemRiftAutoFinishThreshold = 2
NephalemRiftOrbsChance = 0
; health
HealthPotionRestorePercentage = 60
HealthPotionCooldown = 30
ResurrectionCharges = 3
; waypoints
UnlockAllWaypoints = false
; player attribute modifier
StrengthMultiplier = 1
StrengthParagonMultiplier = 1
DexterityMultiplier = 1
DexterityParagonMultiplier = 1
IntelligenceMultiplier = 1
IntelligenceParagonMultiplier = 1
VitalityMultiplier = 1
VitalityParagonMultiplier = 1
; quests
AutoSaveQuests = false
; minimap
ForceMinimapVisibility = false
```

## Description

| Key              | Description               |
| ---------------- | ------------------------- |
| `RateExp`        | Experience multiplier     |
| `RateMoney`      | Currency multiplier       |
| `RateDrop`       | Drop quantity multiplier  |
| `RateChangeDrop` | Drop quality multiplier   |
| `RateMonsterHP`  | Monsters HP multiplier    |
| `RateMonsterDMG` | Monster damage multiplier |
| `ChanceHighQualityUnidentified` | Percentage that a unique, legendary, set or special item created is unidentified |
| `ChanceNormalUnidentified` | Percentage that normal item created is unidentified |
| `ResurrectionCharges` | Amount of times user can resurrect at corpse |
| `BossHealthMultiplier` | Boss Health Multiplier |
| `BossDamageMultiplier` | Boss Damage Multiplier |
| `HealthPotionRestorePercentage` | How much (from 1-100) a health potion will heal. |
| `HealthPotionCooldown` | How much (in seconds) to use a health potion again. |
| `UnlockAllWaypoints` | Unlocks all waypoints in campaign |
| `StrengthMultiplier` | Player's strength multiplier |
| `StrengthParagonMultiplier` | Player's strength multiplier **for paragons** |
| `DexterityMultiplier` | Player's dexterity multiplier |
| `DexterityParagonMultiplier` | Player's dexterity multiplier **for paragons** |
| `IntelligenceMultiplier` | Player's intelligence multiplier |
| `IntelligenceParagonMultiplier` | Player's intelligence multiplier **for paragons** |
| `VitalityMultiplier` | Player's vitality multiplier |
| `VitalityParagonMultiplier` | Player's vitality multiplier **for paragons** |
| `AutoSaveQuests` *in tests* | Force Save Quests/Step, even if Act's quest setup marked as Saveable = FALSE. Doesn't apply to OpenWorld games. |
| `NephalemRiftProgressMultiplier` | Nephalem Rift Progress Modifier |
| `NephalemRiftAutoFinish` | Nephalem Auto-Finish when there's still `NephalemRiftAutoFinishThreshold` monsters or less are alive on the rift |
| `NephalemRiftAutoFinishThreshold` | Nephalem Rift Progress Modifier |
| `NephalemRiftOrbsChance` | Nephalem Rifts chance of spawning a orb. |
| `ForceMinimapVisibility` | Forces the minimap visibility |
