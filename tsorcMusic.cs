using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
//using tsorcRevamp;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

//[assembly: CompilationRelaxations(8)]
//[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
//[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
//[assembly: AssemblyVersion("0.0.0.0")]

namespace tsorcMusic
{
    public class tsorcMusic : Mod
    {
        public static tsorcMusic instance = new tsorcMusic
        {
            MusicAutoloadingEnabled = true
        };

        public override string Name => "tsorcMusic";

        

        public override void PostSetupContent()
        {           
            Type menuLoaderType = typeof(MenuLoader);
            FieldInfo menuListInfo = menuLoaderType.GetField("menus", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            System.Collections.Generic.List<ModMenu> modMenuList = (System.Collections.Generic.List < ModMenu > )menuListInfo.GetValue(null);

            //Only forcibly set the menu to ours if ours is the only menu loaded, to avoid conflicts (and the wrath of the TML team)
            if (modMenuList[3].Name == "The Story of Red Cloud" && modMenuList.Count == 4)
            {
                FieldInfo LastSelectedModMenuInfo = menuLoaderType.GetField("LastSelectedModMenu", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                LastSelectedModMenuInfo.SetValue(null, modMenuList[3].FullName);
            }
        }

        public override void Close()
        {
            //This code prevented a crash in 1.3, which could happen if the modded main menu music was playing while the mod was unloaded.
            //I'm unsure if this code will be necessary in 1.4, but if it is it'll probably need major edits. So it's staying disabled for now.
            /*
            int titleMusicIndex = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Night");
            int rainMusicIndex = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Rain");
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
            base.Close();*/
        }
    }
    public class tsorcMusicMenu : ModMenu
    {
        public override int Music => MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/VillageDay");
        public override string Name => "The Story of Red Cloud";
        public override void Update(bool isOnTitleScreen)
        {            

           Main.curMusic = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/VillageDay");
        }
    }

   

    public class tsorcMusicScene : ModSceneEffect
    {
        public override int Music => SelectMusic().Item1;        
        public override SceneEffectPriority Priority => SelectMusic().Item2;

        public override bool IsSceneEffectActive(Player player)
        {
            return true;
        }
        
        public Tuple<int, SceneEffectPriority> SelectMusic()
        {
            int Music = 0;
            SceneEffectPriority Priority = SceneEffectPriority.None;
            if (Main.gameMenu)
            {
                Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/VillageDay");
                Priority = SceneEffectPriority.BiomeMedium;
            }
            else // Biomes
            {
                if (Main.LocalPlayer.active)
                {
                    // overworld daytime and not desert or snow or glowshroom
                    if (Main.LocalPlayer.ZoneOverworldHeight && Main.dayTime && (!Main.LocalPlayer.ZoneDesert || Main.LocalPlayer.ZoneGlowshroom || !Main.LocalPlayer.ZoneSnow)) //overworld and daytime and not desert
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/OverworldDay");
                        Priority = SceneEffectPriority.BiomeMedium;
                    }

                    // overworld forest
                    if (Main.LocalPlayer.ZoneForest && Main.dayTime) //overworld and daytime and not desert
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/OverworldDay");
                        Priority = SceneEffectPriority.BiomeMedium;
                    }

                    // overworld and not desert and not snow and night
                    else if (Main.LocalPlayer.ZoneOverworldHeight && !Main.LocalPlayer.ZoneDesert && !Main.LocalPlayer.ZoneSnow && !Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Night");
                        Priority = SceneEffectPriority.BiomeLow;
                    }

                    // marble and granite caves, hives underground and forests underground
                    if (Main.LocalPlayer.ZoneGranite || Main.LocalPlayer.ZoneMarble || (Main.LocalPlayer.ZoneHive && !Main.LocalPlayer.ZoneOverworldHeight) || (Main.LocalPlayer.ZoneForest && Main.LocalPlayer.ZoneRockLayerHeight))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Tunnels");
                        Priority = SceneEffectPriority.BiomeMedium;
                    }

                    // desert
                    // overworld and desert and/or jungle and daytime
                    if (Main.LocalPlayer.ZoneOverworldHeight && Main.dayTime && (Main.LocalPlayer.ZoneDesert || Main.LocalPlayer.ZoneDesert && Main.LocalPlayer.ZoneJungle))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Desert");
                        Priority = SceneEffectPriority.BossLow;
                    }

                    // overworld and desert and/or jungle and night time
                    if (Main.LocalPlayer.ZoneOverworldHeight && !Main.dayTime && (Main.LocalPlayer.ZoneDesert || Main.LocalPlayer.ZoneDesert && Main.LocalPlayer.ZoneJungle))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Hallow");
                        Priority = SceneEffectPriority.BossLow;
                    }

                    // overworld and desert and daytime
                    else if (Main.LocalPlayer.ZoneOverworldHeight && Main.LocalPlayer.ZoneDesert && Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Desert");
                        Priority = SceneEffectPriority.Event;
                    }

                    // overworld and underground desert and night
                    else if (Main.LocalPlayer.ZoneOverworldHeight && Main.LocalPlayer.ZoneUndergroundDesert && !Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Hallow");
                        Priority = SceneEffectPriority.BossLow;
                    }

                    // overworld and desert and not underground desert
                    else if (Main.LocalPlayer.ZoneOverworldHeight && Main.LocalPlayer.ZoneDesert && !Main.LocalPlayer.ZoneUndergroundDesert)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Desert");
                        Priority = SceneEffectPriority.BiomeHigh;
                    }
                    else if (Main.LocalPlayer.ZoneDirtLayerHeight && Main.LocalPlayer.ZoneUndergroundDesert)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Desert");
                        Priority = SceneEffectPriority.Event;
                    }

                    // underground desert
                    else if (Main.LocalPlayer.ZoneRockLayerHeight && Main.LocalPlayer.ZoneUndergroundDesert)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundDesert");
                        Priority = SceneEffectPriority.Event;
                    }
                    else if (Main.LocalPlayer.ZoneDesert)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Desert");
                        Priority = SceneEffectPriority.Event;
                    }

                    // sandstorm
                    if (Main.LocalPlayer.ZoneSandstorm)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Sandstorm");
                        Priority = SceneEffectPriority.Environment;
                    }

                    // hallow
                    if (Main.LocalPlayer.ZoneOverworldHeight && Main.LocalPlayer.ZoneHallow)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Hallow");
                        Priority = SceneEffectPriority.BiomeMedium;
                    }
                    else if (Main.LocalPlayer.ZoneDirtLayerHeight && Main.LocalPlayer.ZoneHallow)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundHallow");
                        Priority = SceneEffectPriority.BiomeMedium;
                    }
                    else if (Main.LocalPlayer.ZoneRockLayerHeight && Main.LocalPlayer.ZoneHallow)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundHallow");
                        Priority = SceneEffectPriority.BiomeMedium;
                    }

                    // ocean
                    if (Main.LocalPlayer.ZoneBeach)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Ocean");
                        Priority = SceneEffectPriority.Event;
                    }
                    else if (Main.LocalPlayer.ZoneBeach && Main.LocalPlayer.ZoneJungle)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Ocean");
                        Priority = SceneEffectPriority.Event;
                    }

                    // sky temple in hell
                    else if (Main.LocalPlayer.ZoneDungeon && Main.LocalPlayer.ZoneUnderworldHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/SkyTemple");
                        Priority = SceneEffectPriority.BossLow;
                    }

                    // dungeon and corrupt
                    if (Main.LocalPlayer.ZoneDungeon && Main.LocalPlayer.ZoneCorrupt)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Dungeon");
                        Priority = SceneEffectPriority.BiomeHigh;
                    }

                    // meteor day and night
                    if (Main.LocalPlayer.ZoneMeteor && Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Eerie");
                        Priority = SceneEffectPriority.Environment;
                    }
                    else if (Main.LocalPlayer.ZoneMeteor && !Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundEerie");
                        Priority = SceneEffectPriority.Environment;
                    }

                    // jungle 
                    if (Main.LocalPlayer.ZoneJungle && !Main.LocalPlayer.ZoneDungeon && !Main.LocalPlayer.ZoneMeteor && Main.LocalPlayer.ZoneOverworldHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Jungle");
                        Priority = SceneEffectPriority.BiomeLow;
                    }
                    else if (Main.LocalPlayer.ZoneJungle && !Main.LocalPlayer.ZoneDungeon && !Main.LocalPlayer.ZoneMeteor && Main.LocalPlayer.ZoneDirtLayerHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Jungle");
                        Priority = SceneEffectPriority.BiomeLow;
                    }
                    else if (Main.LocalPlayer.ZoneJungle && !Main.LocalPlayer.ZoneDungeon && !Main.LocalPlayer.ZoneMeteor && Main.LocalPlayer.ZoneRockLayerHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundJungle");
                        Priority = SceneEffectPriority.BiomeLow;
                    }

                    // snow

                    // snow above ground
                    if (Main.LocalPlayer.ZoneSnow && Main.LocalPlayer.ZoneOverworldHeight && Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundHallow");
                        Priority = SceneEffectPriority.Event;
                    }
                    else if (Main.LocalPlayer.ZoneSnow && Main.LocalPlayer.ZoneOverworldHeight && !Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Snow");
                        Priority = SceneEffectPriority.Event;
                    }
                    else if (Main.LocalPlayer.ZoneSnow)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Snow");
                        Priority = SceneEffectPriority.Event;
                    }
                    // snow below ground
                    if (Main.LocalPlayer.ZoneSnow && Main.LocalPlayer.ZoneRockLayerHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundSnow");
                        Priority = SceneEffectPriority.BiomeMedium; //was event
                    }
                    else if (Main.LocalPlayer.ZoneSnow && Main.LocalPlayer.ZoneDirtLayerHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Snow");
                        Priority = SceneEffectPriority.Event;
                    }
                    // corruption or crimson
                    if (Main.LocalPlayer.ZoneCorrupt && Main.LocalPlayer.ZoneOverworldHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Corruption");
                        Priority = SceneEffectPriority.Environment;
                    }
                    else if (Main.LocalPlayer.ZoneCorrupt && Main.LocalPlayer.ZoneDirtLayerHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Corruption");
                        Priority = SceneEffectPriority.Environment;
                    }
                    else if (Main.LocalPlayer.ZoneCorrupt && Main.LocalPlayer.ZoneRockLayerHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundCorruption");
                        Priority = SceneEffectPriority.Environment;
                    }
                    else if (Main.LocalPlayer.ZoneCorrupt && Main.LocalPlayer.ZoneDesert)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Desert");
                        Priority = SceneEffectPriority.Environment;
                    }
                    else if (Main.LocalPlayer.ZoneCrimson && Main.LocalPlayer.ZoneOverworldHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Crimson");
                        Priority = SceneEffectPriority.BiomeMedium;
                    }
                    else if (Main.LocalPlayer.ZoneCrimson && Main.LocalPlayer.ZoneDirtLayerHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Crimson");
                        Priority = SceneEffectPriority.BiomeMedium;
                    }
                    else if (Main.LocalPlayer.ZoneCrimson && Main.LocalPlayer.ZoneRockLayerHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundCrimson");
                        Priority = SceneEffectPriority.BiomeHigh;
                    }
                    if (Main.LocalPlayer.ZoneCrimson && Main.LocalPlayer.ZoneJungle)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Crimson");
                        Priority = SceneEffectPriority.BiomeMedium;
                    }
                    // underworld day and night
                    else if (Main.LocalPlayer.ZoneUnderworldHeight && Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Underworld");
                        Priority = SceneEffectPriority.Event;
                    }
                    else if (Main.LocalPlayer.ZoneUnderworldHeight && !Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundCrimson");
                        Priority = SceneEffectPriority.Event;
                    }
                    // glowshroom above ground
                    if (Main.LocalPlayer.ZoneGlowshroom && Main.LocalPlayer.ZoneOverworldHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Space");
                        Priority = SceneEffectPriority.BiomeLow;
                    }
                    else if (Main.LocalPlayer.ZoneGlowshroom && Main.LocalPlayer.ZoneDirtLayerHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Mushrooms");
                        Priority = SceneEffectPriority.BiomeLow;
                    }
                    else if (Main.LocalPlayer.ZoneGlowshroom && !Main.LocalPlayer.ZoneDungeon && Main.LocalPlayer.ZoneRockLayerHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundMushrooms");
                        Priority = SceneEffectPriority.BiomeLow;
                    }
                    else if (Main.LocalPlayer.ZoneGlowshroom && Main.LocalPlayer.ZoneDungeon && Main.LocalPlayer.ZoneRockLayerHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundHallowOld");
                        Priority = SceneEffectPriority.BiomeHigh;
                    }
                    // space
                    if (Main.LocalPlayer.ZoneSkyHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Space");
                        Priority = SceneEffectPriority.Environment;
                    }
                    // graveyard
                    if (Main.LocalPlayer.ZoneGraveyard)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Catacombs");
                        Priority = SceneEffectPriority.BiomeHigh;
                    }
                    // rain but not crimson or corruption and above ground! (makes no sense to hear it below ground haha ;P)
                    if (Main.LocalPlayer.ZoneRain && Main.LocalPlayer.ZoneOverworldHeight && !Main.LocalPlayer.ZoneCrimson && !Main.LocalPlayer.ZoneCorrupt)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Rain");
                        Priority = SceneEffectPriority.BiomeMedium;
                    }
                    // underground but not anything else
                    if (Main.LocalPlayer.ZoneDirtLayerHeight && !Main.LocalPlayer.ZoneBeach && !Main.LocalPlayer.ZoneSnow && !Main.LocalPlayer.ZoneCorrupt && !Main.LocalPlayer.ZoneCrimson && !Main.LocalPlayer.ZoneDesert && !Main.LocalPlayer.ZoneDungeon && !Main.LocalPlayer.ZoneGlowshroom && !Main.LocalPlayer.ZoneHallow && !Main.LocalPlayer.ZoneJungle && !Main.LocalPlayer.ZoneMeteor && !Main.LocalPlayer.ZoneOldOneArmy && !Main.LocalPlayer.ZoneTowerNebula && !Main.LocalPlayer.ZoneTowerSolar && !Main.LocalPlayer.ZoneTowerVortex && !Main.LocalPlayer.ZoneTowerStardust && !Main.LocalPlayer.ZoneUnderworldHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Tunnels");
                        Priority = SceneEffectPriority.Event;
                    }
                    else if (Main.LocalPlayer.ZoneRockLayerHeight && !Main.LocalPlayer.ZoneBeach && !Main.LocalPlayer.ZoneSnow && !Main.LocalPlayer.ZoneCorrupt && !Main.LocalPlayer.ZoneCrimson && !Main.LocalPlayer.ZoneDesert && !Main.LocalPlayer.ZoneDungeon && !Main.LocalPlayer.ZoneGlowshroom && !Main.LocalPlayer.ZoneHallow && !Main.LocalPlayer.ZoneJungle && !Main.LocalPlayer.ZoneMeteor && !Main.LocalPlayer.ZoneOldOneArmy && !Main.LocalPlayer.ZoneTowerNebula && !Main.LocalPlayer.ZoneTowerSolar && !Main.LocalPlayer.ZoneTowerVortex && !Main.LocalPlayer.ZoneTowerStardust && !Main.LocalPlayer.ZoneUnderworldHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Underground");
                        Priority = SceneEffectPriority.Event;
                    }
                  
                    // pillars
                    if (Main.LocalPlayer.ZoneTowerNebula || Main.LocalPlayer.ZoneTowerSolar || Main.LocalPlayer.ZoneTowerStardust || Main.LocalPlayer.ZoneTowerVortex)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Pillars");
                        Priority = SceneEffectPriority.BossHigh;
                    }

                    // special areas
                    int playerX = (int)(Main.LocalPlayer.Center.X / 16f);
                    int playerY = (int)(Main.LocalPlayer.Center.Y / 16f);

                    // village, plus "towns" with 3 or more npcs anywhere
                    if ((playerX > 3999 && playerX < 4393 && playerY > 600 && playerY < 742) || Main.LocalPlayer.townNPCs > 2 || (playerX > 4053 && playerX < 4255 && playerY > 600 && playerY < 765)) //X - left then right coordinate, higher in the sky then lower in ground number
                    {
                        if (Main.dayTime && !Main.hardMode)
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/VillageDay"); //Set town day music
                            Priority = SceneEffectPriority.BiomeMedium; //Set priority
                        }
                        if (Main.dayTime && Main.hardMode)
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/RoundtableHold"); //Set town day music after Red fucks up
                            Priority = SceneEffectPriority.BiomeMedium; //Set priority
                        }
                        if (!Main.dayTime)
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/VillageNight"); //Set town night music
                            Priority = SceneEffectPriority.BiomeMedium; //Set priority
                        }
                    }

                    // forgotten city pre-HM
                    if (Main.dayTime && !Main.hardMode && !Main.LocalPlayer.ZoneSnow && (Main.LocalPlayer.ZoneDungeon || (Main.LocalPlayer.ZoneDungeon && Main.LocalPlayer.ZoneJungle)))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/ForgottenCity");
                        Priority = SceneEffectPriority.Event;
                    }
                    if (!Main.dayTime && !Main.hardMode && !Main.LocalPlayer.ZoneSnow && (Main.LocalPlayer.ZoneDungeon || (Main.LocalPlayer.ZoneDungeon && Main.LocalPlayer.ZoneJungle)))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/GreatUndergroundRivers");
                        Priority = SceneEffectPriority.Event;
                    }

                    // forgotten City HM
                    if (Main.hardMode && (Main.LocalPlayer.ZoneDungeon && !Main.LocalPlayer.ZoneSnow || (Main.LocalPlayer.ZoneDungeon && Main.LocalPlayer.ZoneJungle)))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss10");
                        Priority = SceneEffectPriority.Event;
                    }

                    // wyvern mage fortress
                    if (playerX > 6336 && playerX < 7385 && playerY > 90 && playerY < 608)
                    {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/WyvernMageFortress"); 
                            Priority = SceneEffectPriority.BiomeHigh; //Set priority
                    }

                    // jungle village
                    if (playerX > 5805 && playerX < 5980 && playerY > 740 && playerY < 900)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/JungleVillage");
                        Priority = SceneEffectPriority.BiomeHigh; //Set priority
                    }

                    // ashen caves
                    if (playerX > 2439 && playerX < 3100 && playerY > 1733 && playerY < 1920)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Caverns");
                        Priority = SceneEffectPriority.BiomeHigh; //Set priority
                    }


                    // sky temple
                    if ((playerX > 1421 && playerX < 4973 && playerY < 470) || (playerX > 1200 && playerX < 2491 && playerY < 430) || (playerX > 44 && playerX < 1015 && playerY < 580)) // empress of light, attraidies sky temple plus vortex island to the right of attraidies, wise man and island far left
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/SkyTemple");
                        Priority = SceneEffectPriority.BiomeMedium; //Set priority
                    }

                    // molten sky temple
                    if (Main.hardMode && Main.tile.Width > playerX && Main.tile.Height > playerY)
                    {
                        if (playerY > 1685 && Main.tile[playerX, playerY] != null && (Main.tile[playerX, playerY].WallType == WallID.PinkDungeonSlabUnsafe || Main.tile[playerX, playerY].WallType == WallID.StarlitHeavenWallpaper))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/SkyTemple");
                            Priority = SceneEffectPriority.BiomeHigh; //Set priority
                        }
                    }

                    // water temple
                    if (Main.hardMode && Main.tile.Width > playerX && Main.tile.Height > playerY)
                    {
                        if (Main.tile[playerX, playerY] != null && (Main.tile[playerX, playerY].WallType == WallID.GreenDungeonTileUnsafe || Main.tile[playerX, playerY].WallType == WallID.GreenDungeonSlabUnsafe) || (Main.tile[playerX, playerY].WallType == WallID.IceUnsafe && Main.tile[playerX, playerY].WallType == WallID.GreenDungeonTileUnsafe || Main.tile[playerX, playerY].WallType == WallID.GreenDungeonSlabUnsafe))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/WaterTemple");
                            Priority = SceneEffectPriority.BossLow;
                        }
                    }

                    // shadow temple
                    if (Main.hardMode && Main.tile.Width > playerX && Main.tile.Height > playerY)
                    {
                        if (Main.tile[playerX, playerY] != null && (Main.tile[playerX, playerY].WallType == WallID.PinkDungeonUnsafe || Main.tile[playerX, playerY].WallType == WallID.Crystal))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Dungeon");
                            Priority = SceneEffectPriority.BossLow;
                        }
                    }

                    // catacombs of the deep (SHM)
                    if (Main.tile.Width > playerX && Main.tile.Height > playerY) 
                    {
                        if (Main.tile[playerX, playerY] != null && (Main.tile[playerX, playerY].WallType == WallID.PinkDungeonTileUnsafe))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Catacombs");
                            Priority = SceneEffectPriority.BossLow;
                        }
                    }

                    // pyramid
                    if ((playerX > 5665 && playerX < 6025 && playerY > 1600 && playerY < 1805) || (playerX > 6000 && playerX < 6123 && playerY > 1671 && playerY < 1800))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Ending");
                        Priority = SceneEffectPriority.BossLow; //Set priority
                    }

                    // old one's tree
                    if (playerX > 2535 && playerX < 2850 && playerY > 1055 && playerY < 1550)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/OldOnesTree");
                        Priority = SceneEffectPriority.BossLow; //Set priority
                    }

                    // dark tower
                    if (playerX > 1310 && playerX < 1460 && playerY > 354 && playerY < 864)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/DarkTower");
                        Priority = SceneEffectPriority.BossLow; //Set priority
                    }

                    // lihzahrd temple
                    if (Main.tile.Width > playerX && Main.tile.Height > playerY)
                    {
                        if (Main.tile[playerX, playerY] != null && (Main.tile[playerX, playerY].WallType == WallID.LihzahrdBrickUnsafe || Main.tile[playerX, playerY].WallType == WallID.DiscWall) && !NPC.AnyNPCs(245))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundDesert");
                            Priority = SceneEffectPriority.BossLow;
                        }
                    }

                    // tomb of gwyn
                    if (!Main.LocalPlayer.ZoneOverworldHeight && Main.tile.Width > playerX && Main.tile.Height > playerY)
                    {
                        if (Main.tile[playerX, playerY] != null && (Main.tile[playerX, playerY].WallType == WallID.ObsidianBrickUnsafe || Main.tile[playerX, playerY].WallType == WallID.TitanstoneBlock))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Ending");
                            Priority = SceneEffectPriority.BossLow;
                        }
                    }

                    // frozen ocean above ground
                    if (Main.tile.Width > playerX && Main.tile.Height > playerY)
                    {
                        if (playerX > 7600 && playerX < 8370 && playerY > 567 && playerY < 1170 && !Main.dayTime)
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundHallow");
                            Priority = SceneEffectPriority.BossLow;
                        }
                        if (playerX > 7600 && playerX < 8370 && playerY > 567 && playerY < 1170 && Main.dayTime)
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Snow");
                            Priority = SceneEffectPriority.BossLow;
                        }
                    }

                    // if in burnt village and haven't beaten EoC, EoW or Skelebones
                    if (playerX > 4773 && playerX < 4955 && playerY > 823 && playerY < 883 && (!NPC.downedBoss1 || !NPC.downedBoss2 || !NPC.downedBoss3)) 
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Opening");
                        Priority = SceneEffectPriority.BossLow;
                    }

                    // Randomize song for a single biome code, untested
                    //	if (music != MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Jungle") && MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Jungle2");)
                    //	{
                    //		int songType = Main.rand.Next(2);
                    //		if (songType == 0) Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Jungle");
                    //		if (songType == 1) Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Jungle2");
                    //	}


                    // Vanilla Enemies
                    if (NPC.AnyNPCs(343)) // Yeti
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Cavern");
                        Priority = SceneEffectPriority.Event;
                    }

                    if (NPC.AnyNPCs(243)) // Ice Golem
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Cavern");
                        Priority = SceneEffectPriority.BossLow;
                    }

                    if (NPC.AnyNPCs(578) && !(NPC.AnyNPCs(548) || NPC.AnyNPCs(549) || NPC.AnyNPCs(554) || NPC.AnyNPCs(563))) // DD2LightningBugT3
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundMushrooms");
                        Priority = SceneEffectPriority.BossLow;
                    }

                    if (NPC.AnyNPCs(548) || NPC.AnyNPCs(549)) // ETERNIA EVENT
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss2");
                        Priority = SceneEffectPriority.BossLow;
                    }

                    // Vanilla Bosses
                    if (NPC.AnyNPCs(4)) // Eye of Cthulhu 
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss1");
                        Priority = SceneEffectPriority.BossLow;
                    }
                    else if (NPC.AnyNPCs(668)) // Deerclops
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss10");
                        Priority = SceneEffectPriority.BossLow;
                    }
                    else if (NPC.AnyNPCs(50)) // King Slime
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss10");
                        Priority = SceneEffectPriority.BossLow;
                    }
                    else if (NPC.AnyNPCs(13)) // Eater of Worlds
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss3");
                        Priority = SceneEffectPriority.BossLow;
                    }
                    else if (NPC.AnyNPCs(636)) // Empress of Light
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss12");
                        Priority = SceneEffectPriority.BossLow;
                    }
                    else if (NPC.AnyNPCs(266)) // Brain of Cthulhu
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss3");
                        Priority = SceneEffectPriority.BossLow;
                    }
                    else if (NPC.AnyNPCs(222)) // Queen Bee
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss1");
                        Priority = SceneEffectPriority.BossLow;
                    }
                    else if (NPC.AnyNPCs(35)) // Skeletron
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Sandstorm");
                        Priority = SceneEffectPriority.BossMedium;
                    }
                    else if (NPC.AnyNPCs(113)) // Wall of Flesh
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss17");
                        Priority = SceneEffectPriority.BossMedium;
                    }
                    else if (NPC.AnyNPCs(245)) // Golem
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss7");
                        Priority = SceneEffectPriority.BossMedium;
                    }
                    else if (NPC.AnyNPCs(246)) // Golem Head
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss7");
                        Priority = SceneEffectPriority.BossMedium;
                    }
                    else if (NPC.AnyNPCs(262)) // Plantera
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss14");
                        Priority = SceneEffectPriority.BossMedium;
                    }
                    else if (NPC.AnyNPCs(370)) // Duke Fishron
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss3");
                        Priority = SceneEffectPriority.BossLow;
                    }
                    else if (NPC.AnyNPCs(439)) // Lunatic Cultist
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss2");
                        Priority = SceneEffectPriority.BossLow;
                    }
                    if (NPC.AnyNPCs(657)) // Queen Slime
                    {
                    	Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss2");
                    	Priority = SceneEffectPriority.BossLow;
                    }
                    else if (NPC.AnyNPCs(134)) // The Destroyer
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss7");
                        Priority = SceneEffectPriority.BossLow;
                    }
                    else if (NPC.AnyNPCs(125) || NPC.AnyNPCs(126)) // The Twins
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss11");
                        Priority = SceneEffectPriority.BossLow;
                    }
                    else if (NPC.AnyNPCs(127)) // Skeletron Prime
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss6");
                        Priority = SceneEffectPriority.BossLow;
                    }
                    if (NPC.AnyNPCs(398) || NPC.AnyNPCs(397) || NPC.AnyNPCs(396)) // Moon Lord
                    {
                        //Main.NewText("the thing is playing yes");
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss5");
                        Priority = SceneEffectPriority.BossHigh;
                    }

                    // Events
                    

                    // Old One Army
                    if (Main.bloodMoon && Main.LocalPlayer.ZoneOldOneArmy) 
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss19");
                        Priority = SceneEffectPriority.Event;
                    }
                    // Blood Moon
                    if (Main.bloodMoon && Main.LocalPlayer.ZoneOverworldHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Pillars");
                        Priority = SceneEffectPriority.Event;
                    }
                    // Eclipse
                    if (Main.eclipse && Main.dayTime && Main.LocalPlayer.ZoneOverworldHeight) 
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Space");
                        Priority = SceneEffectPriority.Event;
                    }
                    // Goblin Invasion
                    if (NPC.AnyNPCs(26) || NPC.AnyNPCs(27) || NPC.AnyNPCs(28) || NPC.AnyNPCs(29) || NPC.AnyNPCs(111) || NPC.AnyNPCs(471))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss6");
                        Priority = SceneEffectPriority.BossMedium;
                    }
                    // Torch God
                    if (Main.LocalPlayer.happyFunTorchTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Invader");
                        Priority = SceneEffectPriority.BossHigh;
                    }

                    // Red Cloud Music

                    Mod tsorcRevamp;
                    ModLoader.TryGetMod("tsorcRevamp", out tsorcRevamp);
                    if (tsorcRevamp != null)
                    {
                        // Red Cloud Boss Music

                        // Pinwheel
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Pinwheel").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss18");
                            Priority = SceneEffectPriority.BossMedium;
                        }                      

                        // The Machine
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("TheMachine").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss11");
                            Priority = SceneEffectPriority.BossMedium;
                        }  
                        
                        // Red Knight 
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("RedKnight").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Sandstorm");
                            Priority = SceneEffectPriority.BossLow;
                        }

                        // Great Red Knight 
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("GreatRedKnight").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss13");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // Black Knight
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("BlackKnight").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss15");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // Leonhard Phase 1
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("LeonhardPhase1").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Invader");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // Lothric Black Knight
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("LothricBlackKnight").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Invader");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // Ancient Demons
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("AncientOolacileDemon").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("AncientDemonOfTheAbyss").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("AncientDemon").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss15");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("HeroofLumelia").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss2");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("JungleWyvernHead").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss10");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("TheRage").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss7");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("TheSorrow").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss12");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("TheHunter").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss10");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // The Triad
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Cataluminance").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("RetinazerV2").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("SpazmatismV2").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss9");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Artorias").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss6");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Witchking").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss14");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Blight").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Pillars");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("WyvernMage").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("MechaDragonHead").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss12");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Gaibon").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Slogra").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss7");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("SerrisHead").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("SerrisX").Type)) // Serris X
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss19");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        //Fiends
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("EarthFiendLich").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("LichKingDisciple").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("LichKingSerpentHead").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss16");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("FireFiendMarilith").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss16");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("WaterFiendKraken").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss12");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Chaos").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss14");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("DarkCloud").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss11");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Death").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss13");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("WyvernMageShadow").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("GhostDragonHead").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/God-DevouringSerpent");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("HellkiteDragonHead").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/God-DevouringSerpent");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("SeathTheScalelessHead").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("PrimordialCrystal").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss9");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("AbysmalOolacileSorcerer").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss14");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Gwyn").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss8");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        //Attraidies 4 phases
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("DarkShogunMask").Type)) //phase 1
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss5");
                            Priority = SceneEffectPriority.BossHigh;
                        }
                        else if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("DarkDragonMask").Type)) //phase 2
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss5");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        else if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Okiku").Type)) //phase 3a
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss5");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        else if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("BrokenOkiku").Type)) //phase 3b
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss5");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("AttraidiesMimic").Type)) //phase 4a
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss8");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        else if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Attraidies").Type)) //phase 4b
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss8");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        
                    }
                }
            }

            //boost all the priorities by 2, to firmly overwrite all vanilla music
            if ((int)Priority <= 6)
            {
                Priority++;
            }
            if ((int)Priority <= 7)
            {
                Priority++;
            }
            return new Tuple<int, SceneEffectPriority>(Music, Priority);
        }        
    }
}
