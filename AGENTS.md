# AGENTS.md

This file provides guidance to Codex (Codex.ai/code) when working with code in this repository.

## Overview

**tsorcMusic** is a tModLoader client-side music mod for Terraria. It replaces all vanilla music and adds custom biome/boss music for *The Story of Red Cloud* (`tsorcRevamp`) mod. The mod is `side = Client` — no server-side code.

- tModLoader 1.4.4 / Terraria 1.4.4.9
- Build: tModLoader's built-in build system (not dotnet CLI alone — build via the tModLoader IDE helper or in-game "Build & Reload")
- The `.csproj` targets tModLoader's SDK; building with `dotnet build` directly requires the tModLoader SDK to be installed

## Architecture

The entire mod lives in one file: [`tsorcMusic.cs`](tsorcMusic.cs).

### Key classes

| Class | Purpose |
|---|---|
| `tsorcMusic` | `Mod` entry point. Sets `MusicAutoloadingEnabled = true` (all `.ogg` files in `Sounds/Music/` are auto-registered). `PostSetupContent` force-selects the custom main menu if no other mod menu is loaded. |
| `tsorcMusicMenu` | `ModMenu` that plays `VillageDay` on the title screen. |
| `tsorcMusicScene` | `ModSceneEffect` — always active (`IsSceneEffectActive` returns `true`). Delegates to `SelectMusic()`. |

### Music selection logic (`SelectMusic`)

`SelectMusic()` returns a `(musicSlot, SceneEffectPriority)` tuple evaluated every tick. Priority order (last write wins within the method, with some exceptions):

1. **Biomes** — overworld day/night, snow, desert, hallow, corruption/crimson, jungle, mushroom, ocean, space/sky, underworld, etc.
2. **Coordinate-based areas** — hardcoded tile-coordinate rectangles for tsorcRevamp world-specific locations (Western Ocean, Village, Sky Temple, Wyvern Mage Fortress, etc.). These are mapped to the tsorcRevamp campaign world layout and will be wrong for other worlds.
3. **Wall-type detection** — dungeons (Shadow Temple, Water Temple, Sky Temple molten section, Catacombs, Tomb of Gwyn, Lihzahrd Temple) are identified by `WallType`.
4. **NPC presence** — vanilla bosses and events checked via `NPC.AnyNPCs(id)`.
5. **tsorcRevamp bosses** — loaded via `ModLoader.TryGetMod("tsorcRevamp")` and then `Find<ModNPC>("Name").Type`. Gracefully skipped if `tsorcRevamp` isn't loaded.

After all checks, all priorities are boosted by +2 (clamped to max 8) to override any vanilla music that might otherwise win.

### Adding a new music track

1. Drop the `.ogg` into `Sounds/Music/` — it is auto-registered as `MusicLoader.GetMusicSlot(instance, "Sounds/Music/<name>")`.
2. Add a condition block in `SelectMusic()` in [tsorcMusic.cs](tsorcMusic.cs). Copy an existing `if` block for the appropriate category (biome, boss, area).
3. Update `README.md` and `description.txt` with the song credit.

### Adding music for a new tsorcRevamp boss

```csharp
if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("BossClassName").Type))
{
    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/BossN");
    Priority = SceneEffectPriority.BossMedium;
}
```

Place this inside the `if (tsorcRevamp != null)` block. The class name must match the `ModNPC` subclass name in the `tsorcRevamp` mod exactly.

### Priority reference

tModLoader's `SceneEffectPriority` enum runs from `None` (0) through `BossHigh` (8). Higher value beats lower. Common values used here:

- `BiomeLow` / `BiomeMedium` / `BiomeHigh` — standard biome music
- `Environment` — events like sandstorm, meteor
- `Event` — zone-based events
- `BossLow` / `BossMedium` / `BossHigh` — boss encounters

The +2 blanket boost at the end of `SelectMusic()` means the effective range is shifted up, firmly overriding vanilla.
