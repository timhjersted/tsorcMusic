using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
//using tsorcRevamp;
using Terraria;
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
        public override int Music => MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Night");
        public override string Name => "The Story of Red Cloud";
        public override void Update(bool isOnTitleScreen)
        {            

           Main.curMusic = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Night");
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
                Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Night");
                Priority = SceneEffectPriority.BiomeMedium;
            }
            else //Biomes
            {
                if (Main.LocalPlayer.active)
                {                    
                    if (Main.LocalPlayer.ZoneOverworldHeight && Main.dayTime && !Main.LocalPlayer.ZoneDesert)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/OverworldDay");
                        Priority = SceneEffectPriority.BiomeMedium;
                    }
                    else if (Main.LocalPlayer.ZoneOverworldHeight && !Main.dayTime && !Main.LocalPlayer.ZoneDesert)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Night");
                        Priority = SceneEffectPriority.BiomeMedium;
                    }
                    else if (Main.LocalPlayer.ZoneOverworldHeight && Main.LocalPlayer.ZoneDesert && !Main.LocalPlayer.ZoneUndergroundDesert && Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Ocean");
                        Priority = SceneEffectPriority.BossLow;
                    }
                    else if (Main.LocalPlayer.ZoneOverworldHeight && Main.LocalPlayer.ZoneDesert && Main.LocalPlayer.ZoneJungle && Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Ocean");
                        Priority = SceneEffectPriority.BossLow;
                    }
                    else if (Main.LocalPlayer.ZoneOverworldHeight && Main.LocalPlayer.ZoneDesert && Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Desert");
                        Priority = SceneEffectPriority.Event;
                    }
                    else if (Main.LocalPlayer.ZoneOverworldHeight && Main.LocalPlayer.ZoneUndergroundDesert && !Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Desert");
                        Priority = SceneEffectPriority.BossLow;
                    }
                    else if (Main.LocalPlayer.ZoneDirtLayerHeight && Main.LocalPlayer.ZoneUndergroundDesert)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Desert");
                        Priority = SceneEffectPriority.Event;
                    }
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
                    else if (Main.LocalPlayer.ZoneDungeon && Main.LocalPlayer.ZoneUndergroundDesert || Main.LocalPlayer.ZoneDesert)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundDesert");
                        Priority = SceneEffectPriority.BossLow;
                    }
                    if (Main.LocalPlayer.ZoneSandstorm)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Sandstorm");
                        Priority = SceneEffectPriority.BossMedium;
                    }
                    else if (Main.LocalPlayer.ZoneOverworldHeight && Main.LocalPlayer.ZoneHallow)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Hallow");
                        Priority = SceneEffectPriority.BiomeLow;
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
                    if (Main.LocalPlayer.ZoneDungeon && Main.LocalPlayer.ZoneJungle)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Dungeon");
                        Priority = SceneEffectPriority.Environment;
                    }
                    else if (Main.LocalPlayer.ZoneDungeon && !Main.LocalPlayer.ZoneJungle)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Dungeon");
                        Priority = SceneEffectPriority.Environment;
                    }
                    else if (Main.LocalPlayer.ZoneDungeon && Main.LocalPlayer.ZoneRockLayerHeight && Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Dungeon");
                        Priority = SceneEffectPriority.Event;
                    }
                    else if (Main.LocalPlayer.ZoneDungeon && Main.LocalPlayer.ZoneRockLayerHeight && !Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Cavern");
                        Priority = SceneEffectPriority.Event;
                    }
                    else if (Main.LocalPlayer.ZoneDungeon && Main.LocalPlayer.ZoneDirtLayerHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Cavern");
                        Priority = SceneEffectPriority.Event;
                    }
                    else if (Main.LocalPlayer.ZoneDungeon && Main.LocalPlayer.ZoneUnderworldHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Dungeon");
                        Priority = SceneEffectPriority.BossLow;
                    }
                    else if (Main.LocalPlayer.ZoneDungeon && Main.LocalPlayer.ZoneJungle)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Dungeon");
                        Priority = SceneEffectPriority.Event;
                    }
                    if (Main.LocalPlayer.ZoneDungeon && Main.LocalPlayer.ZoneCorrupt)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Dungeon");
                        Priority = SceneEffectPriority.BossLow;
                    }
                    if (Main.LocalPlayer.ZoneMeteor)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Eerie");
                        Priority = SceneEffectPriority.Environment;
                    }
                    else if (Main.LocalPlayer.ZoneMeteor && !Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundEerie");
                        Priority = SceneEffectPriority.Environment;
                    }
                    if (Main.LocalPlayer.ZoneJungle && !Main.LocalPlayer.ZoneMeteor && !Main.LocalPlayer.ZoneDungeon && !Main.LocalPlayer.ZoneBeach && !Main.LocalPlayer.ZoneDesert)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Jungle");
                        Priority = SceneEffectPriority.BiomeMedium;
                    }
                    else if (Main.LocalPlayer.ZoneMeteor && Main.LocalPlayer.ZoneJungle)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Eerie");
                        Priority = SceneEffectPriority.Event;
                    }
                    else if (Main.LocalPlayer.ZoneJungle && !Main.LocalPlayer.ZoneMeteor && !Main.LocalPlayer.ZoneBeach && Main.LocalPlayer.ZoneOverworldHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Jungle");
                        Priority = SceneEffectPriority.BiomeHigh;
                    }
                    else if (Main.LocalPlayer.ZoneJungle && !Main.LocalPlayer.ZoneMeteor && Main.LocalPlayer.ZoneDirtLayerHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Desert");
                        Priority = SceneEffectPriority.BiomeHigh;
                    }
                    else if (Main.LocalPlayer.ZoneJungle && !Main.LocalPlayer.ZoneMeteor && !Main.LocalPlayer.ZoneDungeon && Main.LocalPlayer.ZoneRockLayerHeight && Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Jungle");
                        Priority = SceneEffectPriority.BiomeHigh;
                    }
                    else if (Main.LocalPlayer.ZoneJungle && !Main.LocalPlayer.ZoneMeteor && !Main.LocalPlayer.ZoneDungeon && Main.LocalPlayer.ZoneRockLayerHeight && !Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Underground");
                        Priority = SceneEffectPriority.BiomeHigh;
                    }
                    else if (Main.LocalPlayer.ZoneJungle && Main.LocalPlayer.ZoneDungeon && Main.LocalPlayer.ZoneRockLayerHeight && Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Desert");
                        Priority = SceneEffectPriority.Environment;
                    }
                    else if (Main.LocalPlayer.ZoneJungle && Main.LocalPlayer.ZoneDungeon && Main.LocalPlayer.ZoneRockLayerHeight && !Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Underground");
                        Priority = SceneEffectPriority.Environment;
                    }
                    if (Main.LocalPlayer.ZoneSnow)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Snow");
                        Priority = SceneEffectPriority.Event;
                    }
                    else if (Main.LocalPlayer.ZoneSnow && Main.LocalPlayer.ZoneOverworldHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Snow");
                        Priority = SceneEffectPriority.Event;
                    }
                    if (Main.LocalPlayer.ZoneSnow && Main.LocalPlayer.ZoneRockLayerHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundSnow");
                        Priority = SceneEffectPriority.Event;
                    }
                    else if (Main.LocalPlayer.ZoneSnow && Main.LocalPlayer.ZoneDirtLayerHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Snow");
                        Priority = SceneEffectPriority.Event;
                    }
                    /*if (Main.LocalPlayer.ZoneCorrupt)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Corruption");
                        Priority = SceneEffectPriority.Environment;
                    }*/
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
                        Priority = SceneEffectPriority.BiomeHigh;
                    }
                    else if (Main.LocalPlayer.ZoneCrimson && Main.LocalPlayer.ZoneDirtLayerHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Crimson");
                        Priority = SceneEffectPriority.BiomeHigh;
                    }
                    else if (Main.LocalPlayer.ZoneCrimson && Main.LocalPlayer.ZoneRockLayerHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundCrimson");
                        Priority = SceneEffectPriority.BiomeHigh;
                    }
                    if (Main.LocalPlayer.ZoneCrimson && Main.LocalPlayer.ZoneJungle)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Crimson");
                        Priority = SceneEffectPriority.Event;
                    }
                    else if (Main.LocalPlayer.ZoneUnderworldHeight && Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Underworld");
                        Priority = SceneEffectPriority.Event;
                    }
                    else if (Main.LocalPlayer.ZoneUnderworldHeight && !Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundEerie");
                        Priority = SceneEffectPriority.Event;
                    }
                    /*if (Main.LocalPlayer.ZoneGlowshroom)
                    {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Space");
                            Priority = SceneEffectPriority.BossLow;
                    }*/
                    if (Main.LocalPlayer.ZoneGlowshroom && Main.LocalPlayer.ZoneOverworldHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Space");
                        Priority = SceneEffectPriority.Event;
                    }
                    else if (Main.LocalPlayer.ZoneGlowshroom && Main.LocalPlayer.ZoneDirtLayerHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Mushrooms");
                        Priority = SceneEffectPriority.Event;
                    }
                    else if (Main.LocalPlayer.ZoneGlowshroom && Main.LocalPlayer.ZoneRockLayerHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundMushrooms");
                        Priority = SceneEffectPriority.BiomeHigh;
                    }
                    else if (Main.LocalPlayer.ZoneSkyHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Space");
                        Priority = SceneEffectPriority.Event;
                    }
                    else if (Main.LocalPlayer.ZoneRain && !Main.LocalPlayer.ZoneCrimson && !Main.LocalPlayer.ZoneCorrupt)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Rain");
                        Priority = SceneEffectPriority.BiomeMedium;
                    }
                    else if (Main.LocalPlayer.ZoneDirtLayerHeight && !Main.LocalPlayer.ZoneBeach && !Main.LocalPlayer.ZoneSnow && !Main.LocalPlayer.ZoneCorrupt && !Main.LocalPlayer.ZoneCrimson && !Main.LocalPlayer.ZoneDesert && !Main.LocalPlayer.ZoneDungeon && !Main.LocalPlayer.ZoneGlowshroom && !Main.LocalPlayer.ZoneHallow && !Main.LocalPlayer.ZoneJungle && !Main.LocalPlayer.ZoneMeteor && !Main.LocalPlayer.ZoneOldOneArmy && !Main.LocalPlayer.ZoneTowerNebula && !Main.LocalPlayer.ZoneTowerSolar && !Main.LocalPlayer.ZoneTowerVortex && !Main.LocalPlayer.ZoneTowerStardust && !Main.LocalPlayer.ZoneUnderworldHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Underground");
                        Priority = SceneEffectPriority.Event;
                    }
                    else if (Main.LocalPlayer.ZoneRockLayerHeight && !Main.LocalPlayer.ZoneBeach && !Main.LocalPlayer.ZoneSnow && !Main.LocalPlayer.ZoneCorrupt && !Main.LocalPlayer.ZoneCrimson && !Main.LocalPlayer.ZoneDesert && !Main.LocalPlayer.ZoneDungeon && !Main.LocalPlayer.ZoneGlowshroom && !Main.LocalPlayer.ZoneHallow && !Main.LocalPlayer.ZoneJungle && !Main.LocalPlayer.ZoneMeteor && !Main.LocalPlayer.ZoneOldOneArmy && !Main.LocalPlayer.ZoneTowerNebula && !Main.LocalPlayer.ZoneTowerSolar && !Main.LocalPlayer.ZoneTowerVortex && !Main.LocalPlayer.ZoneTowerStardust && !Main.LocalPlayer.ZoneUnderworldHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Underground");
                        Priority = SceneEffectPriority.Event;
                    }

                    //Pillars
                    if (Main.LocalPlayer.ZoneTowerNebula || Main.LocalPlayer.ZoneTowerSolar || Main.LocalPlayer.ZoneTowerStardust || Main.LocalPlayer.ZoneTowerVortex)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Pillars");
                        Priority = SceneEffectPriority.BossHigh;
                    }

                    //Special Areas
                    int playerX = (int)(Main.LocalPlayer.Center.X / 16f);
                    int playerY = (int)(Main.LocalPlayer.Center.Y / 16f);

                    //Town music
                    if (playerX > 3980 && playerX < 4393 && playerY > 600 && playerY < 788)
                    {
                        Main.NewText("running");
                        if (Main.dayTime)
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/OverworldDay"); //Set town day music
                            Priority = SceneEffectPriority.BiomeMedium; //Set priority
                        }
                        else
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Night"); //Set town night music
                            Priority = SceneEffectPriority.BiomeMedium; //Set priority
                        }
                    }

                    //Lihzahrd Temple
                    if (Main.tile.Width > playerX && Main.tile.Height > playerY)
                    {
                        if (Main.tile[playerX, playerY] != null && Main.tile[playerX, playerY].WallType == WallID.LihzahrdBrickUnsafe && !NPC.AnyNPCs(245))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundDesert");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                    }

                    //If in burnt village and haven't beaten EoC, EoW or Skelebones
                    if (playerX > 4777 && playerX < 4955 && playerY > 823 && playerY < 883 && (!NPC.downedBoss1 || !NPC.downedBoss2 || !NPC.downedBoss3)) 
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Night");
                        Priority = SceneEffectPriority.BossLow;
                    }

                    // Randomize song for a single biome code, untested
                    //	if (music != MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Jungle") && MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Jungle2");)
                    //	{
                    //		int songType = Main.rand.Next(2);
                    //		if (songType == 0) Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Jungle");
                    //		if (songType == 1) Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Jungle2");
                    //	}


                    //Vanilla Enemies
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
                    else if (NPC.AnyNPCs(50)) // King Slime
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss1");
                        Priority = SceneEffectPriority.BossLow;
                    }
                    else if (NPC.AnyNPCs(13)) // Eater of Worlds
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss3");
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
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss6");
                        Priority = SceneEffectPriority.BossMedium;
                    }
                    else if (NPC.AnyNPCs(113)) // Wall of Flesh
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss5");
                        Priority = SceneEffectPriority.BossMedium;
                    }
                    else if (NPC.AnyNPCs(245)) // Golem
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss3");
                        Priority = SceneEffectPriority.BossMedium;
                    }
                    else if (NPC.AnyNPCs(246)) // Golem Head
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss3");
                        Priority = SceneEffectPriority.BossMedium;
                    }
                    else if (NPC.AnyNPCs(262)) // Plantera
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss4");
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
                    //if (NPC.AnyNPCs(657)) // Queen Slime
                    //{
                    //	Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss2");
                    //	Priority = SceneEffectPriority.BossLow;
                    //}
                    else if (NPC.AnyNPCs(134)) // The Destroyer
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss2");
                        Priority = SceneEffectPriority.BossLow;
                    }
                    else if (NPC.AnyNPCs(125) || NPC.AnyNPCs(126)) // The Twins
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss1");
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
                    if (Main.bloodMoon && Main.LocalPlayer.ZoneOverworldHeight) // Blood Moon
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Pillars");
                        Priority = SceneEffectPriority.Event;
                    }
                    if (Main.eclipse && Main.dayTime && Main.LocalPlayer.ZoneOverworldHeight) // Eclipse
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Space");
                        Priority = SceneEffectPriority.Event;
                    }
                    // Goblin Invasion
                    if (NPC.AnyNPCs(26) || NPC.AnyNPCs(27) || NPC.AnyNPCs(28) || NPC.AnyNPCs(29) || NPC.AnyNPCs(111) || NPC.AnyNPCs(471))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss1");
                        Priority = SceneEffectPriority.BossMedium;
                    }

                    // Red Cloud Music

                    Mod tsorcRevamp;
                    ModLoader.TryGetMod("tsorcRevamp", out tsorcRevamp);
                    if (tsorcRevamp != null)
                    {
                        // Red Cloud Enemy Music
                        if (!Main.hardMode && NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("RedKnight").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss1");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("LeonhardPhase1").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss2");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // Red Cloud Boss Music
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("AncientOolacileDemon").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("AncientDemonOfTheAbyss").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss6");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("HeroofLumelia").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss2");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("JungleWyvernHead").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss1");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("TheRage").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss1");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("TheSorrow").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss4");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("TheHunter").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss2");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Artorias").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss6");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Witchking").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss3");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Blight").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Pillars");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("WyvernMage").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("MechaDragonHead").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss2");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Gaibon").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Slogra").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss6");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("SerrisHead").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("SerrisX").Type)) // Serris X
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss6");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        //Fiends
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("EarthFiendLich").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("LichKingDisciple").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("LichKingSerpentHead").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss5");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("FireFiendMarilith").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss4");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("WaterFiendKraken").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss2");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Chaos").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss4");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("DarkCloud").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss1");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Death").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss6");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("WyvernMageShadow").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("GhostDragonHead").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss4");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("HellkiteDragonHead").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss2");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("SeathTheScalelessHead").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("PrimordialCrystal").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss5");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("AbysmalOolacileSorcerer").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss3");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Gwyn").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss5");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        //Attraidies 4 phases
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("DarkShogunMask").Type)) //phase 1
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss5");
                            Priority = SceneEffectPriority.BossMedium;
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
                        else if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("AttraidiesMimic").Type)) //phase 4a
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss5");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        else if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Attraidies").Type)) //phase 4b
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss5");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                        else if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("DarkShogunMask").Type)) //phase 4c
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss5");
                            Priority = SceneEffectPriority.BossMedium;
                        }
                    }
                }
            }

            //Boost all the priorities by 2, to firmly overwrite all vanilla music
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
