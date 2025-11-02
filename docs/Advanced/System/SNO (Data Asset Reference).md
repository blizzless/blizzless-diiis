## What is SNO?

**SNO** stands for **"Strategic Network Object"** (or alternatively, it's a data asset reference system used in Diablo III). It's a universal identifier/handle system used to reference all game assets in the Diablo III server architecture.

## SNO Structure

The SNO system is defined in the following files:

```
src/DiIiS-NA/D3-GameServer/Core/Types/SNO/
├── SNOHandle.cs       - Main SNO handle class
├── SNOGroup.cs        - SNO category enumeration
└── WorldSno.cs        - World/location-specific SNOs
```

### SNOHandle Class
Located at: `src/DiIiS-NA/D3-GameServer/Core/Types/SNO/SNOHandle.cs`

An SNO is composed of two parts:
- **Group** (SNOGroup) - Categorizes the type of asset
- **Id** (int) - Unique numeric identifier for the specific asset

```csharp
public class SNOHandle
{
    public SNOGroup Group { get; }
    public int Id { get; }
    
    // Constructor with both group and ID
    public SNOHandle(SNOGroup group, int id)
    {
        _group = group;
        Id = id;
    }
}
```

## SNO Groups

The **SNOGroup** enum defines all asset categories. Some examples include:

```csharp
public enum SNOGroup : int
{
    Code = -2,
    None = -1,
    Actor = 1,           // Game characters/actors
    Adventure = 2,       // Adventure/quest data
    AiBehavior = 3,      // AI behavior definitions
    Anim = 6,           // Animations
    Appearance = 9,      // Visual appearance data
    Hero = 10,          // Hero definitions
    Monster = 25,       // Monster definitions
    LevelArea = 22,     // Level/area definitions
    World = 51,         // World/map definitions
    Tutorial = 63,      // Tutorial content
    // ... and many more
}
```

## WorldSno Enum

A specific implementation for world/location assets:

```csharp
public enum WorldSno: int
{
    a2c1dun_swr_caldeum_01,      // Sewers of Caldeum
    a2c2dun_zolt_treasurehunter, // Chamber of the Lost Idol
    a2dun_zolt_level01,
    // ... hundreds of world locations
}
```

## How SNO is Used

1. **Asset Reference**: SNOs uniquely identify game assets (worlds, actors, items, etc.)
2. **Serialization**: SNOs can be encoded/decoded for network transmission
3. **Map Generation**: In `DRLGTemplate.cs`, WorldSno values reference specific dungeon layouts
4. **Message System**: SNOs are used in game messages to reference assets

### Example from Map Generator:
```csharp
{WorldSno.a2c1dun_swr_caldeum_01,  // Using SNO as key
    new List<DRLGLayout>{
        new DRLGLayout{
            enterPositionX = 3,
            enterPositionY = 1,
            // ... layout data
        }
    }
}
```

More info: [[Diablo Random Level Generation (DRLG Emu)]]

## Key Files Overview

| File                       | Purpose                                       |
| -------------------------- | --------------------------------------------- |
| **SNOHandle.cs**           | Core SNO class with encoding/decoding methods |
| **SNOGroup.cs**            | Enumeration of all SNO categories             |
| **WorldSno.cs**            | Enumeration of all world/map SNOs             |
| **HandledSNOAttribute.cs** | Attribute to mark which SNOs an actor handles |
| **SNODataMessage.cs**      | Message protocol for transmitting SNO data    |

The SNO system is essentially the game's asset database key system—every game element (world, actor, animation, etc.) has a unique SNO identifier!