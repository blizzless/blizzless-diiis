## Map Generator Overview

The **map generator** in the D3 project is called the **DRLG (Diablo Random Level Generation) System**. It's a procedural level layout generation system specifically designed for dungeon/environment generation in the Diablo III game server.

## Files That Compose the Map Generator

The map generator primarily consists of the following structure:

### Main File:
- **`DRLGTemplate.cs`** - Located at `src/DiIiS-NA/D3-GameServer/GSSystem/GeneratorsSystem/DRLGTemplate.cs`

This is a static class within the `DiIiS_NA.GameServer.GSSystem.GeneratorsSystem` namespace that contains all the map generator logic.

## Key Components

### 1. **DRLGLayout Class**
Each map is defined using `DRLGLayout` structures that contain:
- `enterPositionX` - Entry point X coordinate
- `enterPositionY` - Entry point Y coordinate
- `exitPositionX` - Exit point X coordinate
- `exitPositionY` - Exit point Y coordinate
- `map` - 2D grid of integers representing the tile layout (`List<List<int>>`)

### 2. **Map Structure**
Maps are composed of:
- **2D Grid Arrays**: Each map is represented as a 2D list of integers where each number represents a specific tile type
- **Tile IDs**: Numbers like 0, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 13, 14, 15 represent different tile types (walls, floors, enemies, objects, etc.)

### 3. **Organization**
The `DRLGTemplate.cs` file contains multiple dictionary entries organized by:
- **Act regions** (World SNO identifiers)
- **Dungeon names** (e.g., "Sewers of Caldeum", "Chamber of the Lost Idol", "Ancient Cave")
- **Multiple layout variations** for each dungeon to provide variety

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

The map generator system is a **template-based procedural generation system** that:
- Uses predefined layout templates for each dungeon
- Stores tile-based 2D maps with fixed entry/exit points
- Provides multiple layout variations per dungeon for replay variety
- Is implemented in a single comprehensive C# file (`DRLGTemplate.cs`)