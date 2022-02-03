using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
//using tsorcRevamp;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: AssemblyVersion("0.0.0.0")]

namespace tsorcMusic
{
    public class tsorcMusic : Mod
    {
        public static tsorcMusic instance;

        public override void UpdateMusic(ref int music, ref MusicPriority priority)
        {
            if (!Main.gameMenu)
            {

                // Biomes
                if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneOverworldHeight && Main.dayTime && !Main.player[Main.myPlayer].ZoneDesert)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/OverworldDay");
                    priority = (MusicPriority)2;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneOverworldHeight && !Main.dayTime && !Main.player[Main.myPlayer].ZoneDesert)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Night");
                    priority = (MusicPriority)2;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneOverworldHeight && Main.player[Main.myPlayer].ZoneDesert && !Main.player[Main.myPlayer].ZoneUndergroundDesert && Main.dayTime)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Ocean");
                    priority = (MusicPriority)6;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneOverworldHeight && Main.player[Main.myPlayer].ZoneDesert && Main.player[Main.myPlayer].ZoneJungle && Main.dayTime)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Ocean");
                    priority = (MusicPriority)6;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneOverworldHeight && Main.player[Main.myPlayer].ZoneDesert && Main.dayTime)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Desert");
                    priority = (MusicPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneOverworldHeight && Main.player[Main.myPlayer].ZoneUndergroundDesert && !Main.dayTime)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Desert");
                    priority = (MusicPriority)6;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneDirtLayerHeight && Main.player[Main.myPlayer].ZoneUndergroundDesert)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Desert");
                    priority = (MusicPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneRockLayerHeight && Main.player[Main.myPlayer].ZoneUndergroundDesert)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/UndergroundDesert");
                    priority = (MusicPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneDesert)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Desert");
                    priority = (MusicPriority)4;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneDungeon && Main.player[Main.myPlayer].ZoneUndergroundDesert || Main.player[Main.myPlayer].ZoneDesert)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/UndergroundDesert");
                    priority = (MusicPriority)6;
                }
                if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneSandstorm)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Sandstorm");
                    priority = (MusicPriority)7;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneOverworldHeight && Main.player[Main.myPlayer].ZoneHoly)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Hallow");
                    priority = (MusicPriority)1;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneDirtLayerHeight && Main.player[Main.myPlayer].ZoneHoly)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/UndergroundHallow");
                    priority = (MusicPriority)2;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneRockLayerHeight && Main.player[Main.myPlayer].ZoneHoly)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/UndergroundHallow");
                    priority = (MusicPriority)2;
                }
                if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneBeach)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Ocean");
                    priority = (MusicPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneBeach && Main.player[Main.myPlayer].ZoneJungle)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Ocean");
                    priority = (MusicPriority)5;
                }
                if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneDungeon && Main.player[Main.myPlayer].ZoneJungle)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Dungeon");
                    priority = (MusicPriority)4;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneDungeon && !Main.player[Main.myPlayer].ZoneJungle)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Dungeon");
                    priority = (MusicPriority)4;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneDungeon && Main.player[Main.myPlayer].ZoneRockLayerHeight && Main.dayTime)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Dungeon");
                    priority = (MusicPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneDungeon && Main.player[Main.myPlayer].ZoneRockLayerHeight && !Main.dayTime)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Cavern");
                    priority = (MusicPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneDungeon && Main.player[Main.myPlayer].ZoneDirtLayerHeight)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Cavern");
                    priority = (MusicPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneDungeon && Main.player[Main.myPlayer].ZoneUnderworldHeight)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Dungeon");
                    priority = (MusicPriority)6;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneDungeon && Main.player[Main.myPlayer].ZoneJungle)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Dungeon");
                    priority = (MusicPriority)5;
                }
                if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneDungeon && Main.player[Main.myPlayer].ZoneCorrupt)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Dungeon");
                    priority = (MusicPriority)6;
                }
                if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneMeteor)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Eerie");
                    priority = (MusicPriority)4;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneMeteor && !Main.dayTime)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/UndergroundEerie");
                    priority = (MusicPriority)4;
                }
                if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneJungle && !Main.player[Main.myPlayer].ZoneMeteor && !Main.player[Main.myPlayer].ZoneDungeon && !Main.player[Main.myPlayer].ZoneBeach && !Main.player[Main.myPlayer].ZoneDesert)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Jungle");
                    priority = (MusicPriority)2;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneMeteor && Main.player[Main.myPlayer].ZoneJungle)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Eerie");
                    priority = (MusicPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneJungle && !Main.player[Main.myPlayer].ZoneMeteor && !Main.player[Main.myPlayer].ZoneBeach && Main.player[Main.myPlayer].ZoneOverworldHeight)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Jungle");
                    priority = (MusicPriority)3;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneJungle && !Main.player[Main.myPlayer].ZoneMeteor && Main.player[Main.myPlayer].ZoneDirtLayerHeight)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Desert");
                    priority = (MusicPriority)3;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneJungle && !Main.player[Main.myPlayer].ZoneMeteor && !Main.player[Main.myPlayer].ZoneDungeon && Main.player[Main.myPlayer].ZoneRockLayerHeight && Main.dayTime)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Jungle");
                    priority = (MusicPriority)3;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneJungle && !Main.player[Main.myPlayer].ZoneMeteor && !Main.player[Main.myPlayer].ZoneDungeon && Main.player[Main.myPlayer].ZoneRockLayerHeight && !Main.dayTime)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Underground");
                    priority = (MusicPriority)3;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneJungle && Main.player[Main.myPlayer].ZoneDungeon && Main.player[Main.myPlayer].ZoneRockLayerHeight && Main.dayTime)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Desert");
                    priority = (MusicPriority)4;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneJungle && Main.player[Main.myPlayer].ZoneDungeon && Main.player[Main.myPlayer].ZoneRockLayerHeight && !Main.dayTime)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Underground");
                    priority = (MusicPriority)4;
                }
                if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneSnow)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Snow");
                    priority = (MusicPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneSnow && Main.player[Main.myPlayer].ZoneOverworldHeight)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Snow");
                    priority = (MusicPriority)5;
                }
                if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneSnow && Main.player[Main.myPlayer].ZoneRockLayerHeight)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/UndergroundSnow");
                    priority = (MusicPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneSnow && Main.player[Main.myPlayer].ZoneDirtLayerHeight)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Snow");
                    priority = (MusicPriority)5;
                }
                /*if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneCorrupt)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Corruption");
                    priority = (MusicPriority)4;
                }*/
                if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneCorrupt && Main.player[Main.myPlayer].ZoneOverworldHeight)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Corruption");
                    priority = (MusicPriority)4;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneCorrupt && Main.player[Main.myPlayer].ZoneDirtLayerHeight)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Corruption");
                    priority = (MusicPriority)4;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneCorrupt && Main.player[Main.myPlayer].ZoneRockLayerHeight)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/UndergroundCorruption");
                    priority = (MusicPriority)4;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneCorrupt && Main.player[Main.myPlayer].ZoneDesert)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Desert");
                    priority = (MusicPriority)4;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneCrimson && Main.player[Main.myPlayer].ZoneOverworldHeight)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Crimson");
                    priority = (MusicPriority)3;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneCrimson && Main.player[Main.myPlayer].ZoneDirtLayerHeight)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Crimson");
                    priority = (MusicPriority)3;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneCrimson && Main.player[Main.myPlayer].ZoneRockLayerHeight)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/UndergroundCrimson");
                    priority = (MusicPriority)3;
                }
                if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneCrimson && Main.player[Main.myPlayer].ZoneJungle)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Crimson");
                    priority = (MusicPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneUnderworldHeight && Main.dayTime)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Underworld");
                    priority = (MusicPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneUnderworldHeight && !Main.dayTime)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/UndergroundEerie");
                    priority = (MusicPriority)5;
                }
                /*if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneGlowshroom)
                {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Space");
                        priority = (MusicPriority)6;
                }*/
                if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneGlowshroom && Main.player[Main.myPlayer].ZoneOverworldHeight)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Space");
                    priority = (MusicPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneGlowshroom && Main.player[Main.myPlayer].ZoneDirtLayerHeight)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Mushrooms");
                    priority = (MusicPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneGlowshroom && Main.player[Main.myPlayer].ZoneRockLayerHeight)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/UndergroundMushrooms");
                    priority = (MusicPriority)3;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneSkyHeight)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Space");
                    priority = (MusicPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneRain && !Main.player[Main.myPlayer].ZoneCrimson && !Main.player[Main.myPlayer].ZoneCorrupt)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Rain");
                    priority = (MusicPriority)2;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneDirtLayerHeight && !Main.player[Main.myPlayer].ZoneBeach && !Main.player[Main.myPlayer].ZoneSnow && !Main.player[Main.myPlayer].ZoneCorrupt && !Main.player[Main.myPlayer].ZoneCrimson && !Main.player[Main.myPlayer].ZoneDesert && !Main.player[Main.myPlayer].ZoneDungeon && !Main.player[Main.myPlayer].ZoneGlowshroom && !Main.player[Main.myPlayer].ZoneHoly && !Main.player[Main.myPlayer].ZoneJungle && !Main.player[Main.myPlayer].ZoneMeteor && !Main.player[Main.myPlayer].ZoneOldOneArmy && !Main.player[Main.myPlayer].ZoneTowerNebula && !Main.player[Main.myPlayer].ZoneTowerSolar && !Main.player[Main.myPlayer].ZoneTowerVortex && !Main.player[Main.myPlayer].ZoneTowerStardust && !Main.player[Main.myPlayer].ZoneUnderworldHeight)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Underground");
                    priority = (MusicPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneRockLayerHeight && !Main.player[Main.myPlayer].ZoneBeach && !Main.player[Main.myPlayer].ZoneSnow && !Main.player[Main.myPlayer].ZoneCorrupt && !Main.player[Main.myPlayer].ZoneCrimson && !Main.player[Main.myPlayer].ZoneDesert && !Main.player[Main.myPlayer].ZoneDungeon && !Main.player[Main.myPlayer].ZoneGlowshroom && !Main.player[Main.myPlayer].ZoneHoly && !Main.player[Main.myPlayer].ZoneJungle && !Main.player[Main.myPlayer].ZoneMeteor && !Main.player[Main.myPlayer].ZoneOldOneArmy && !Main.player[Main.myPlayer].ZoneTowerNebula && !Main.player[Main.myPlayer].ZoneTowerSolar && !Main.player[Main.myPlayer].ZoneTowerVortex && !Main.player[Main.myPlayer].ZoneTowerStardust && !Main.player[Main.myPlayer].ZoneUnderworldHeight)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Underground");
                    priority = (MusicPriority)5;
                }
                //Pillars
                if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneTowerNebula)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Pillars");
                    priority = (MusicPriority)7;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneTowerSolar)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Pillars");
                    priority = (MusicPriority)7;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneTowerStardust)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Pillars");
                    priority = (MusicPriority)7;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneTowerVortex)
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Pillars");
                    priority = (MusicPriority)7;
                }

                // Special Areas
                int playerX = (int)(Main.LocalPlayer.Center.X / 16f);
                int playerY = (int)(Main.LocalPlayer.Center.Y / 16f);
                if (Main.tile.GetUpperBound(0) > playerX && Main.tile.GetUpperBound(1) > playerY)
                {
                    if (Main.tile[playerX, playerY] != null && Main.tile[playerX, playerY].wall == WallID.LihzahrdBrickUnsafe && !NPC.AnyNPCs(245))
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/UndergroundDesert");
                        priority = (MusicPriority)7;
                    }
                }

                if (Main.player[Main.myPlayer].active && playerX > 4777 && playerX < 4955 && playerY > 823 && playerY < 883 && (!NPC.downedBoss1 || !NPC.downedBoss2 || !NPC.downedBoss3)) //If in burnt village and haven't beaten EoC, EoW or Skelebones
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Night");
                    priority = (MusicPriority)6;
                }
                // Randomize song for a single biome code, untested
                //	if (music != ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Jungle") && ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Jungle2");)
                //	{
                //		int songType = Main.rand.Next(2);
                //		if (songType == 0) music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Jungle");
                //		if (songType == 1) music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Jungle2");
                //	}


                //Vanilla Enemies


                if (NPC.AnyNPCs(343)) // Yeti
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Cavern");
                    priority = (MusicPriority)5;
                }

                if (NPC.AnyNPCs(243)) // Ice Golem
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Cavern");
                    priority = (MusicPriority)6;
                }

                if (NPC.AnyNPCs(578) && !(NPC.AnyNPCs(548) || NPC.AnyNPCs(549) || NPC.AnyNPCs(554) || NPC.AnyNPCs(563))) // DD2LightningBugT3
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/UndergroundMushrooms");
                    priority = (MusicPriority)6;
                }

                if (NPC.AnyNPCs(548) || NPC.AnyNPCs(549)) // ETERNIA EVENT
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss2");
                    priority = (MusicPriority)6;
                }

                // Vanilla Bosses
                if (NPC.AnyNPCs(4)) // Eye of Cthulhu 
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss1");
                    priority = (MusicPriority)6;
                }
                else if (NPC.AnyNPCs(50)) // King Slime
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss1");
                    priority = (MusicPriority)6;
                }
                else if (NPC.AnyNPCs(13)) // Eater of Worlds
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss3");
                    priority = (MusicPriority)6;
                }
                else if (NPC.AnyNPCs(266)) // Brain of Cthulhu
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss3");
                    priority = (MusicPriority)6;
                }
                else if (NPC.AnyNPCs(222)) // Queen Bee
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss1");
                    priority = (MusicPriority)6;
                }
                else if (NPC.AnyNPCs(35)) // Skeletron
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss6");
                    priority = (MusicPriority)7;
                }
                else if (NPC.AnyNPCs(113)) // Wall of Flesh
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss5");
                    priority = (MusicPriority)7;
                }
                else if (NPC.AnyNPCs(245)) // Golem
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss3");
                    priority = (MusicPriority)7;
                }
                else if (NPC.AnyNPCs(246)) // Golem Head
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss3");
                    priority = (MusicPriority)7;
                }
                else if (NPC.AnyNPCs(262)) // Plantera
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss4");
                    priority = (MusicPriority)7;
                }
                else if (NPC.AnyNPCs(370)) // Duke Fishron
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss3");
                    priority = (MusicPriority)6;
                }
                else if (NPC.AnyNPCs(439)) // Lunatic Cultist
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss2");
                    priority = (MusicPriority)6;
                }
                //if (NPC.AnyNPCs(657)) // Queen Slime
                //{
                //	music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss2");
                //	priority = (MusicPriority)6;
                //}
                else if (NPC.AnyNPCs(134)) // The Destroyer
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss2");
                    priority = (MusicPriority)6;
                }
                else if (NPC.AnyNPCs(125) || NPC.AnyNPCs(126)) // The Twins
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss1");
                    priority = (MusicPriority)6;
                }
                else if (NPC.AnyNPCs(127)) // Skeletron Prime
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss6");
                    priority = (MusicPriority)6;
                }
                if (NPC.AnyNPCs(398) || NPC.AnyNPCs(397) || NPC.AnyNPCs(396)) // Moon Lord
                {
                    //Main.NewText("the thing is playing yes");
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss5");
                    priority = (MusicPriority)8;
                }

                // Events
                if (Main.bloodMoon && Main.player[Main.myPlayer].ZoneOverworldHeight) // Blood Moon
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Pillars");
                    priority = (MusicPriority)5;
                }
                if (Main.eclipse && Main.dayTime && Main.player[Main.myPlayer].ZoneOverworldHeight) // Eclipse
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Space");
                    priority = (MusicPriority)5;
                }
                // Goblin Invasion
                if (NPC.AnyNPCs(26) || NPC.AnyNPCs(27) || NPC.AnyNPCs(28) || NPC.AnyNPCs(29) || NPC.AnyNPCs(111) || NPC.AnyNPCs(471))
                {
                    music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss1");
                    priority = (MusicPriority)7;
                }

                // Red Cloud Music
                Mod tsorcRevamp = ModLoader.GetMod("tsorcRevamp");
                if (tsorcRevamp != null)
                {
                    // Red Cloud Enemy Music
                    if (!Main.hardMode && NPC.AnyNPCs(tsorcRevamp.NPCType("RedKnight")))
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss1");
                        priority = (MusicPriority)7;
                    }

                    if (NPC.AnyNPCs(tsorcRevamp.NPCType("LeonhardPhase1")))
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss2");
                        priority = (MusicPriority)7;
                    }

                    // Red Cloud Boss Music
                    if (NPC.AnyNPCs(tsorcRevamp.NPCType("HeroofLumelia")))
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss2");
                        priority = (MusicPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.NPCType("JungleWyvernHead")))
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss1");
                        priority = (MusicPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.NPCType("TheRage")))
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss1");
                        priority = (MusicPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.NPCType("TheSorrow")))
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss4");
                        priority = (MusicPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.NPCType("TheHunter")))
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss2");
                        priority = (MusicPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.NPCType("Artorias")))
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss6");
                        priority = (MusicPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.NPCType("Witchking")))
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss3");
                        priority = (MusicPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.NPCType("Blight")))
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Pillars");
                        priority = (MusicPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.NPCType("WyvernMage")) || NPC.AnyNPCs(tsorcRevamp.NPCType("MechaDragonHead")))
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss2");
                        priority = (MusicPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.NPCType("Gaibon")) || NPC.AnyNPCs(tsorcRevamp.NPCType("Slogra")))
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss6");
                        priority = (MusicPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.NPCType("SerrisHead")) || NPC.AnyNPCs(tsorcRevamp.NPCType("SerrisX"))) // Serris X
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss6");
                        priority = (MusicPriority)7;
                    }
                    //Fiends
                    if (NPC.AnyNPCs(tsorcRevamp.NPCType("EarthFiendLich")) || NPC.AnyNPCs(tsorcRevamp.NPCType("LichKingDisciple")) || NPC.AnyNPCs(tsorcRevamp.NPCType("LichKingSerpentHead")))
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss5");
                        priority = (MusicPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.NPCType("FireFiendMarilith")))
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss4");
                        priority = (MusicPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.NPCType("WaterFiendKraken")))
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss2");
                        priority = (MusicPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.NPCType("Chaos")))
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss4");
                        priority = (MusicPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.NPCType("DarkCloud")))
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss1");
                        priority = (MusicPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.NPCType("Death")))
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss6");
                        priority = (MusicPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.NPCType("WyvernMageShadow")) || NPC.AnyNPCs(tsorcRevamp.NPCType("GhostDragonHead")))
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss4");
                        priority = (MusicPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.NPCType("HellkiteDragonHead")))
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss2");
                        priority = (MusicPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.NPCType("SeathTheScalelessHead")) || NPC.AnyNPCs(tsorcRevamp.NPCType("PrimordialCrystal")))
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss5");
                        priority = (MusicPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.NPCType("AbysmalOolacileSorcerer")))
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss3");
                        priority = (MusicPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.NPCType("Gwyn")))
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss5");
                        priority = (MusicPriority)7;
                    }
                    //Attraidies 4 phases
                    if (NPC.AnyNPCs(tsorcRevamp.NPCType("DarkShogunMask"))) //phase 1
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss5");
                        priority = (MusicPriority)7;
                    }
                    else if (NPC.AnyNPCs(tsorcRevamp.NPCType("DarkDragonMask"))) //phase 2
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss5");
                        priority = (MusicPriority)7;
                    }
                    else if (NPC.AnyNPCs(tsorcRevamp.NPCType("Okiku"))) //phase 3a
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss5");
                        priority = (MusicPriority)7;
                    }
                    else if (NPC.AnyNPCs(tsorcRevamp.NPCType("BrokenOkiku"))) //phase 3b
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss5");
                        priority = (MusicPriority)7;
                    }
                    else if (NPC.AnyNPCs(tsorcRevamp.NPCType("AttraidiesMimic"))) //phase 4a
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss5");
                        priority = (MusicPriority)7;
                    }
                    else if (NPC.AnyNPCs(tsorcRevamp.NPCType("Attraidies"))) //phase 4b
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss5");
                        priority = (MusicPriority)7;
                    }
                    else if (NPC.AnyNPCs(tsorcRevamp.NPCType("DarkShogunMask"))) //phase 4c
                    {
                        music = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Boss5");
                        priority = (MusicPriority)7;
                    }
                }
            }
        }

        public override void Close()
        {
            int titleMusicIndex = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Night");
            int rainMusicIndex = ((Mod)this).GetSoundSlot((SoundType)51, "Sounds/Music/Rain");
            if (titleMusicIndex >= 0 && titleMusicIndex < Main.music.Length)
            {
                if (Main.music[titleMusicIndex].IsPlaying)
                {
                    Main.music[titleMusicIndex].Stop(Microsoft.Xna.Framework.Audio.AudioStopOptions.Immediate);
                }
            }
            if (rainMusicIndex >= 0 && rainMusicIndex < Main.music.Length)
            {
                if (Main.music[rainMusicIndex].IsPlaying)
                {
                    Main.music[rainMusicIndex].Stop(Microsoft.Xna.Framework.Audio.AudioStopOptions.Immediate);
                }
            }
            base.Close();
        }

        // this changes the boundaries of the oceans
        // right side has no ocean biome
        public class tsorcMusicPlayer : ModPlayer
        {
            public override void PreUpdate()
            {
                Point point = player.Center.ToTileCoordinates();
                player.ZoneBeach = player.ZoneOverworldHeight && (point.X < 1000 || point.X > Main.maxTilesX - 8398); //default 380 and 380
            }
        }
    }
}
