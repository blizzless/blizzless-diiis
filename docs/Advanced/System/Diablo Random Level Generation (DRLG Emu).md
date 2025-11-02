## Map Generator Overview

The **map generator** in the D3 project is called the **DRLG (Diablo Random Level Generation) System**. It's a procedural level layout generation system specifically designed for dungeon/environment generation in the Diablo III game server.

## Files That Compose the Map Generator
The map generator primarily consists of the following structure:

### Main File:
- **`DRLGTemplate.cs`** - Located at `src/DiIiS-NA/D3-GameServer/GSSystem/GeneratorsSystem/DRLGTemplate.cs`

This is a static class within the `DiIiS_NA.GameServer.GSSystem.GeneratorsSystem` namespace that contains all the map generator logic.

## Key Components
Core components of the DRLG Emu include:

### 1. **DRLGLayout Class**
Each map is defined using `DRLGLayout` structures that contain:

| Field            | Type                                | Description                                      |
| ---------------- | ----------------------------------- | ------------------------------------------------ |
| `enterPositionX` | ==Int32==                           | Entry point X coordinate                         |
| `enterPositionY` | ==Int32==                           | Entry point Y coordinate                         |
| `exitPositionX`  | ==Int32==                           | Exit point X coordinate                          |
| `exitPositionY`  | ==Int32==                           | Exit point Y coordinate                          |
| `map`            | ==List== of a ==List== of ==Int32== | 2D grid of integers representing the tile layout |

### 2. **Map Structure**
Maps are composed of:
- **2D Grid Arrays**: Each map is represented as a 2D list of integers where each number represents a specific tile type
- **Tile IDs**: Numbers like `0, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 13, 14, 15` represent different tile types (walls, floors, enemies, objects, etc.)

### 3. **Organization**
The `DRLGTemplate.cs` file contains multiple dictionary entries organized by:
- **Act regions** _(World [[SNO (Data Asset Reference)]] identifiers)_
- **Dungeon names** _(e.g., "Sewers of Caldeum", "Chamber of the Lost Idol", "Ancient Cave")_
- **Multiple layout variations** for each dungeon to _provide variety_

### Example Structure:
```csharp
{WorldSno.a2c1dun_swr_caldeum_01, //Sewers of Caldeum
    new List<DRLGLayout>{
        new DRLGLayout{
            enterPositionX = 3,
            enterPositionY = 1,
            exitPositionX = 2,
            exitPositionY = 5,
            map = new List<List<int>>{
                new List<int>{0, 0, 0, 0, 0, 0, },
                new List<int>{0, 0, 0, 2, 0, 0, },
                // ... more tile data
            }
        },
    }
}
```

## Summary

- DRLG Emu is a template-driven dungeon layout system for the D3 server, implemented as a static C# module (`DRLGTemplate.cs`).
- -Layouts are defined as **DRLGLayout records**: enter/exit coordinates (int) and a 2D tile grid (List List int) encoded as integer tile IDs.
- Templates are organized in dictionaries keyed by World SNO and dungeon name, each offering multiple variants for replay variety.
- At runtime, a variant is selected and instantiated with fixed entry/exit anchors; no procedural stitching or noise-based generation is performed.
- The approach provides deterministic, low-overhead level assembly with curated variability via predefined templates.