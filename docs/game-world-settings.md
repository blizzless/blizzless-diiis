# Game World Settings

The parameters of the world can be easily altered using the configuration file located within `config.ini`.

## Configuration

The parameters specified in the `config.ini` file will be saved to the server folder, overwriting the default settings. For example, all values below use their default settings.

```ini
[Game-Server]
RateExp = 1
RateMoney = 1
RateDrop = 1
RateChangeDrop = 1
RateMonsterHP = 1
RateMonsterDMG = 1

ChanceHighQualityUnidentified = 30
ChanceNormalUnidentified = 5

ResurrectionCharges = 3

BossHealthMultiplier = 6
BossDamageMultiplier = 3

AutoSaveQuests = false

NephalemRiftProgressMultiplier = 1

HealthPotionRestorePercentage = 60
HealthPotionCooldown = 30

UnlockAllWaypoints = false
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
| `AutoSaveQuests` *in tests* | Force Save Quests/Step, even if Act's quest setup marked as Saveable = FALSE. Doesn't apply to OpenWorld games. |
| `NephalemRiftProgressMultiplier` | Nephalem Rift Progress Modifier |
| `HealthPotionRestorePercentage` | How much (from 1-100) a health potion will heal. |
| `HealthPotionCooldown` | How much (in seconds) to use a health potion again. |
| `UnlockAllWaypoints` | Unlocks all waypoints in campaign |

