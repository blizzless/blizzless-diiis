# Global settings for game

Using the configuration file you can easily change the parameters of the world.

## Configuration

Apply parameters in `config.ini` file to the server folder (It overwrites the default settings)

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

