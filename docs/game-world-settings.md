# Game World Settings

The parameters of the world can be easily altered using the configuration file located within `config.maps.json`, which is built on server initialization.

For older configs, it will be migrated from `config.ini` automatically.

## Configuration

The parameters specified in the `config.mods.json` file will be created on the server folder, migrating from config.ini, to overwrite the default settings. For example, all values below use their default settings.

The default configuration can be found at [config.mods.json](https://github.com/blizzless/blizzless-diiis/blob/community/configs/config.mods.json)

## Description

```json
{
  "Rate": {
    "Experience": 1.0, // Experience Rate
    "Money": 1.0, // money rate
    "Drop": 1.0, // drop rate
    "ChangeDrop": 1.0 // change drop rate
  },
  "Health": {
    "PotionRestorePercentage": 60.0, // how many in percent will a potion restore
    "PotionCooldown": 30.0, // how many seconds for a full potion recharge
    "ResurrectionCharges": 3 // how many times can you revive at corpse
  },
  "Monster": {
    "HealthMultiplier": 1.0, // monster health multiplier
    "DamageMultiplier": 1.0 // monster damage multiplier
  },
  "Boss": {
    "HealthMultiplier": 6.0, // boss health multiplier
    "DamageMultiplier": 3.0 // boss damage multiplier
  },
  "Quest": {
    "AutoSave": false, // auto save at every quest
    "UnlockAllWaypoints": false // unlocks all waypoints in-game
  },
  "Player": {
    "Multipliers": { // multipliers for the player (e.g. a paragon might need twice these values for fairer gameplay)
      "Strength": {
        "Normal": 1.0,
        "Paragon": 1.0
      },
      "Dexterity": {
        "Normal": 1.0,
        "Paragon": 1.0
      },
      "Intelligence": {
        "Normal": 1.0,
        "Paragon": 1.0
      },
      "Vitality": {
        "Normal": 1.0,
        "Paragon": 1.0
      }
    }
  },
  "Items": {
    "UnidentifiedDropChances": { // chances in % of a dropped item to be unidentified
      "HighQuality": 30.0,
      "NormalQuality": 5.0
    }
  },
  "Minimap": {
    "ForceVisibility": false // forces in-game minimap to be always visible
  },
  "NephalemRift": { // improves overall nephalem rift experience
    "ProgressMultiplier": 1.0,
    "AutoFinish": false,
    "AutoFinishThreshold": 2,
    "OrbsChance": 2.0 // chances of spawning an orb
  }
}
```
