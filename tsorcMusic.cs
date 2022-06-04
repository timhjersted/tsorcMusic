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
                Priority = (SceneEffectPriority)2;
            }
            else
            {
                // Biomes
                if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneOverworldHeight && Main.dayTime && !Main.player[Main.myPlayer].ZoneDesert)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/OverworldDay");
                    Priority = (SceneEffectPriority)2;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneOverworldHeight && !Main.dayTime && !Main.player[Main.myPlayer].ZoneDesert)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Night");
                    Priority = (SceneEffectPriority)2;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneOverworldHeight && Main.player[Main.myPlayer].ZoneDesert && !Main.player[Main.myPlayer].ZoneUndergroundDesert && Main.dayTime)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Ocean");
                    Priority = (SceneEffectPriority)6;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneOverworldHeight && Main.player[Main.myPlayer].ZoneDesert && Main.player[Main.myPlayer].ZoneJungle && Main.dayTime)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Ocean");
                    Priority = (SceneEffectPriority)6;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneOverworldHeight && Main.player[Main.myPlayer].ZoneDesert && Main.dayTime)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Desert");
                    Priority = (SceneEffectPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneOverworldHeight && Main.player[Main.myPlayer].ZoneUndergroundDesert && !Main.dayTime)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Desert");
                    Priority = (SceneEffectPriority)6;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneDirtLayerHeight && Main.player[Main.myPlayer].ZoneUndergroundDesert)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Desert");
                    Priority = (SceneEffectPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneRockLayerHeight && Main.player[Main.myPlayer].ZoneUndergroundDesert)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundDesert");
                    Priority = (SceneEffectPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneDesert)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Desert");
                    Priority = (SceneEffectPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneDungeon && Main.player[Main.myPlayer].ZoneUndergroundDesert || Main.player[Main.myPlayer].ZoneDesert)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundDesert");
                    Priority = (SceneEffectPriority)6;
                }
                if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneSandstorm)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Sandstorm");
                    Priority = (SceneEffectPriority)8;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneOverworldHeight && Main.player[Main.myPlayer].ZoneHallow)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Hallow");
                    Priority = (SceneEffectPriority)1;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneDirtLayerHeight && Main.player[Main.myPlayer].ZoneHallow)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundHallow");
                    Priority = (SceneEffectPriority)2;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneRockLayerHeight && Main.player[Main.myPlayer].ZoneHallow)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundHallow");
                    Priority = (SceneEffectPriority)2;
                }
                if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneBeach)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Ocean");
                    Priority = (SceneEffectPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneBeach && Main.player[Main.myPlayer].ZoneJungle)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Ocean");
                    Priority = (SceneEffectPriority)5;
                }
                if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneDungeon && Main.player[Main.myPlayer].ZoneJungle)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Dungeon");
                    Priority = (SceneEffectPriority)4;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneDungeon && !Main.player[Main.myPlayer].ZoneJungle)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Dungeon");
                    Priority = (SceneEffectPriority)4;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneDungeon && Main.player[Main.myPlayer].ZoneRockLayerHeight && Main.dayTime)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Dungeon");
                    Priority = (SceneEffectPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneDungeon && Main.player[Main.myPlayer].ZoneRockLayerHeight && !Main.dayTime)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Cavern");
                    Priority = (SceneEffectPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneDungeon && Main.player[Main.myPlayer].ZoneDirtLayerHeight)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Cavern");
                    Priority = (SceneEffectPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneDungeon && Main.player[Main.myPlayer].ZoneUnderworldHeight)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Dungeon");
                    Priority = (SceneEffectPriority)6;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneDungeon && Main.player[Main.myPlayer].ZoneJungle)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Dungeon");
                    Priority = (SceneEffectPriority)5;
                }
                if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneDungeon && Main.player[Main.myPlayer].ZoneCorrupt)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Dungeon");
                    Priority = (SceneEffectPriority)6;
                }
                if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneMeteor)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Eerie");
                    Priority = (SceneEffectPriority)4;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneMeteor && !Main.dayTime)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundEerie");
                    Priority = (SceneEffectPriority)4;
                }
                if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneJungle && !Main.player[Main.myPlayer].ZoneMeteor && !Main.player[Main.myPlayer].ZoneDungeon && !Main.player[Main.myPlayer].ZoneBeach && !Main.player[Main.myPlayer].ZoneDesert)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Jungle");
                    Priority = (SceneEffectPriority)2;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneMeteor && Main.player[Main.myPlayer].ZoneJungle)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Eerie");
                    Priority = (SceneEffectPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneJungle && !Main.player[Main.myPlayer].ZoneMeteor && !Main.player[Main.myPlayer].ZoneBeach && Main.player[Main.myPlayer].ZoneOverworldHeight)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Jungle");
                    Priority = (SceneEffectPriority)3;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneJungle && !Main.player[Main.myPlayer].ZoneMeteor && Main.player[Main.myPlayer].ZoneDirtLayerHeight)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Desert");
                    Priority = (SceneEffectPriority)3;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneJungle && !Main.player[Main.myPlayer].ZoneMeteor && !Main.player[Main.myPlayer].ZoneDungeon && Main.player[Main.myPlayer].ZoneRockLayerHeight && Main.dayTime)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Jungle");
                    Priority = (SceneEffectPriority)3;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneJungle && !Main.player[Main.myPlayer].ZoneMeteor && !Main.player[Main.myPlayer].ZoneDungeon && Main.player[Main.myPlayer].ZoneRockLayerHeight && !Main.dayTime)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Underground");
                    Priority = (SceneEffectPriority)3;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneJungle && Main.player[Main.myPlayer].ZoneDungeon && Main.player[Main.myPlayer].ZoneRockLayerHeight && Main.dayTime)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Desert");
                    Priority = (SceneEffectPriority)4;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneJungle && Main.player[Main.myPlayer].ZoneDungeon && Main.player[Main.myPlayer].ZoneRockLayerHeight && !Main.dayTime)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Underground");
                    Priority = (SceneEffectPriority)4;
                }
                if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneSnow)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Snow");
                    Priority = (SceneEffectPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneSnow && Main.player[Main.myPlayer].ZoneOverworldHeight)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Snow");
                    Priority = (SceneEffectPriority)5;
                }
                if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneSnow && Main.player[Main.myPlayer].ZoneRockLayerHeight)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundSnow");
                    Priority = (SceneEffectPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneSnow && Main.player[Main.myPlayer].ZoneDirtLayerHeight)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Snow");
                    Priority = (SceneEffectPriority)5;
                }
                /*if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneCorrupt)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Corruption");
                    Priority = (SceneEffectPriority)4;
                }*/
                if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneCorrupt && Main.player[Main.myPlayer].ZoneOverworldHeight)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Corruption");
                    Priority = (SceneEffectPriority)4;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneCorrupt && Main.player[Main.myPlayer].ZoneDirtLayerHeight)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Corruption");
                    Priority = (SceneEffectPriority)4;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneCorrupt && Main.player[Main.myPlayer].ZoneRockLayerHeight)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundCorruption");
                    Priority = (SceneEffectPriority)4;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneCorrupt && Main.player[Main.myPlayer].ZoneDesert)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Desert");
                    Priority = (SceneEffectPriority)4;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneCrimson && Main.player[Main.myPlayer].ZoneOverworldHeight)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Crimson");
                    Priority = (SceneEffectPriority)3;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneCrimson && Main.player[Main.myPlayer].ZoneDirtLayerHeight)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Crimson");
                    Priority = (SceneEffectPriority)3;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneCrimson && Main.player[Main.myPlayer].ZoneRockLayerHeight)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundCrimson");
                    Priority = (SceneEffectPriority)3;
                }
                if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneCrimson && Main.player[Main.myPlayer].ZoneJungle)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Crimson");
                    Priority = (SceneEffectPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneUnderworldHeight && Main.dayTime)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Underworld");
                    Priority = (SceneEffectPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneUnderworldHeight && !Main.dayTime)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundEerie");
                    Priority = (SceneEffectPriority)5;
                }
                /*if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneGlowshroom)
                {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Space");
                        Priority = (SceneEffectPriority)6;
                }*/
                if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneGlowshroom && Main.player[Main.myPlayer].ZoneOverworldHeight)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Space");
                    Priority = (SceneEffectPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneGlowshroom && Main.player[Main.myPlayer].ZoneDirtLayerHeight)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Mushrooms");
                    Priority = (SceneEffectPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneGlowshroom && Main.player[Main.myPlayer].ZoneRockLayerHeight)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundMushrooms");
                    Priority = (SceneEffectPriority)3;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneSkyHeight)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Space");
                    Priority = (SceneEffectPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneRain && !Main.player[Main.myPlayer].ZoneCrimson && !Main.player[Main.myPlayer].ZoneCorrupt)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Rain");
                    Priority = (SceneEffectPriority)2;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneDirtLayerHeight && !Main.player[Main.myPlayer].ZoneBeach && !Main.player[Main.myPlayer].ZoneSnow && !Main.player[Main.myPlayer].ZoneCorrupt && !Main.player[Main.myPlayer].ZoneCrimson && !Main.player[Main.myPlayer].ZoneDesert && !Main.player[Main.myPlayer].ZoneDungeon && !Main.player[Main.myPlayer].ZoneGlowshroom && !Main.player[Main.myPlayer].ZoneHallow && !Main.player[Main.myPlayer].ZoneJungle && !Main.player[Main.myPlayer].ZoneMeteor && !Main.player[Main.myPlayer].ZoneOldOneArmy && !Main.player[Main.myPlayer].ZoneTowerNebula && !Main.player[Main.myPlayer].ZoneTowerSolar && !Main.player[Main.myPlayer].ZoneTowerVortex && !Main.player[Main.myPlayer].ZoneTowerStardust && !Main.player[Main.myPlayer].ZoneUnderworldHeight)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Underground");
                    Priority = (SceneEffectPriority)5;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneRockLayerHeight && !Main.player[Main.myPlayer].ZoneBeach && !Main.player[Main.myPlayer].ZoneSnow && !Main.player[Main.myPlayer].ZoneCorrupt && !Main.player[Main.myPlayer].ZoneCrimson && !Main.player[Main.myPlayer].ZoneDesert && !Main.player[Main.myPlayer].ZoneDungeon && !Main.player[Main.myPlayer].ZoneGlowshroom && !Main.player[Main.myPlayer].ZoneHallow && !Main.player[Main.myPlayer].ZoneJungle && !Main.player[Main.myPlayer].ZoneMeteor && !Main.player[Main.myPlayer].ZoneOldOneArmy && !Main.player[Main.myPlayer].ZoneTowerNebula && !Main.player[Main.myPlayer].ZoneTowerSolar && !Main.player[Main.myPlayer].ZoneTowerVortex && !Main.player[Main.myPlayer].ZoneTowerStardust && !Main.player[Main.myPlayer].ZoneUnderworldHeight)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Underground");
                    Priority = (SceneEffectPriority)5;
                }
                //Pillars
                if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneTowerNebula)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Pillars");
                    Priority = (SceneEffectPriority)7;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneTowerSolar)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Pillars");
                    Priority = (SceneEffectPriority)7;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneTowerStardust)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Pillars");
                    Priority = (SceneEffectPriority)7;
                }
                else if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].ZoneTowerVortex)
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Pillars");
                    Priority = (SceneEffectPriority)7;
                }

                // Special Areas
                int playerX = (int)(Main.LocalPlayer.Center.X / 16f);
                int playerY = (int)(Main.LocalPlayer.Center.Y / 16f);
                
                
                if (Main.tile.Width > playerX && Main.tile.Height > playerY)
                {
                    if (Main.tile[playerX, playerY] != null && Main.tile[playerX, playerY].WallType == WallID.LihzahrdBrickUnsafe && !NPC.AnyNPCs(245))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundDesert");
                        Priority = (SceneEffectPriority)7;
                    }
                }

                if (Main.player[Main.myPlayer].active && playerX > 4777 && playerX < 4955 && playerY > 823 && playerY < 883 && (!NPC.downedBoss1 || !NPC.downedBoss2 || !NPC.downedBoss3)) //If in burnt village and haven't beaten EoC, EoW or Skelebones
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Night");
                    Priority = (SceneEffectPriority)6;
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
                    Priority = (SceneEffectPriority)5;
                }

                if (NPC.AnyNPCs(243)) // Ice Golem
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Cavern");
                    Priority = (SceneEffectPriority)6;
                }

                if (NPC.AnyNPCs(578) && !(NPC.AnyNPCs(548) || NPC.AnyNPCs(549) || NPC.AnyNPCs(554) || NPC.AnyNPCs(563))) // DD2LightningBugT3
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundMushrooms");
                    Priority = (SceneEffectPriority)6;
                }

                if (NPC.AnyNPCs(548) || NPC.AnyNPCs(549)) // ETERNIA EVENT
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss2");
                    Priority = (SceneEffectPriority)6;
                }

                // Vanilla Bosses
                if (NPC.AnyNPCs(4)) // Eye of Cthulhu 
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss1");
                    Priority = (SceneEffectPriority)6;
                }
                else if (NPC.AnyNPCs(50)) // King Slime
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss1");
                    Priority = (SceneEffectPriority)6;
                }
                else if (NPC.AnyNPCs(13)) // Eater of Worlds
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss3");
                    Priority = (SceneEffectPriority)6;
                }
                else if (NPC.AnyNPCs(266)) // Brain of Cthulhu
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss3");
                    Priority = (SceneEffectPriority)6;
                }
                else if (NPC.AnyNPCs(222)) // Queen Bee
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss1");
                    Priority = (SceneEffectPriority)6;
                }
                else if (NPC.AnyNPCs(35)) // Skeletron
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss6");
                    Priority = (SceneEffectPriority)7;
                }
                else if (NPC.AnyNPCs(113)) // Wall of Flesh
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss5");
                    Priority = (SceneEffectPriority)7;
                }
                else if (NPC.AnyNPCs(245)) // Golem
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss3");
                    Priority = (SceneEffectPriority)7;
                }
                else if (NPC.AnyNPCs(246)) // Golem Head
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss3");
                    Priority = (SceneEffectPriority)7;
                }
                else if (NPC.AnyNPCs(262)) // Plantera
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss4");
                    Priority = (SceneEffectPriority)7;
                }
                else if (NPC.AnyNPCs(370)) // Duke Fishron
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss3");
                    Priority = (SceneEffectPriority)6;
                }
                else if (NPC.AnyNPCs(439)) // Lunatic Cultist
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss2");
                    Priority = (SceneEffectPriority)6;
                }
                //if (NPC.AnyNPCs(657)) // Queen Slime
                //{
                //	Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss2");
                //	Priority = (SceneEffectPriority)6;
                //}
                else if (NPC.AnyNPCs(134)) // The Destroyer
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss2");
                    Priority = (SceneEffectPriority)6;
                }
                else if (NPC.AnyNPCs(125) || NPC.AnyNPCs(126)) // The Twins
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss1");
                    Priority = (SceneEffectPriority)6;
                }
                else if (NPC.AnyNPCs(127)) // Skeletron Prime
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss6");
                    Priority = (SceneEffectPriority)6;
                }
                if (NPC.AnyNPCs(398) || NPC.AnyNPCs(397) || NPC.AnyNPCs(396)) // Moon Lord
                {
                    //Main.NewText("the thing is playing yes");
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss5");
                    Priority = (SceneEffectPriority)8;
                }

                // Events
                if (Main.bloodMoon && Main.player[Main.myPlayer].ZoneOverworldHeight) // Blood Moon
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Pillars");
                    Priority = (SceneEffectPriority)5;
                }
                if (Main.eclipse && Main.dayTime && Main.player[Main.myPlayer].ZoneOverworldHeight) // Eclipse
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Space");
                    Priority = (SceneEffectPriority)5;
                }
                // Goblin Invasion
                if (NPC.AnyNPCs(26) || NPC.AnyNPCs(27) || NPC.AnyNPCs(28) || NPC.AnyNPCs(29) || NPC.AnyNPCs(111) || NPC.AnyNPCs(471))
                {
                    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss1");
                    Priority = (SceneEffectPriority)7;
                }

                // Red Cloud Music
                Mod tsorcRevamp = ModLoader.GetMod("tsorcRevamp");
                if (tsorcRevamp != null)
                {
                    // Red Cloud Enemy Music
                    if (!Main.hardMode && NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("RedKnight").Type))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss1");
                        Priority = (SceneEffectPriority)7;
                    }

                    if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("LeonhardPhase1").Type))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss2");
                        Priority = (SceneEffectPriority)7;
                    }

                    // Red Cloud Boss Music
                    if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("AncientOolacileDemon").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("AncientDemonOfTheAbyss").Type))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss6");
                        Priority = (SceneEffectPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("HeroofLumelia").Type))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss2");
                        Priority = (SceneEffectPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("JungleWyvernHead").Type))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss1");
                        Priority = (SceneEffectPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("TheRage").Type))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss1");
                        Priority = (SceneEffectPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("TheSorrow").Type))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss4");
                        Priority = (SceneEffectPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("TheHunter").Type))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss2");
                        Priority = (SceneEffectPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Artorias").Type))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss6");
                        Priority = (SceneEffectPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Witchking").Type))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss3");
                        Priority = (SceneEffectPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Blight").Type))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Pillars");
                        Priority = (SceneEffectPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("WyvernMage").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("MechaDragonHead").Type))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss2");
                        Priority = (SceneEffectPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Gaibon").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Slogra").Type))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss6");
                        Priority = (SceneEffectPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("SerrisHead").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("SerrisX").Type)) // Serris X
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss6");
                        Priority = (SceneEffectPriority)7;
                    }
                    //Fiends
                    if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("EarthFiendLich").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("LichKingDisciple").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("LichKingSerpentHead").Type))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss5");
                        Priority = (SceneEffectPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("FireFiendMarilith").Type))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss4");
                        Priority = (SceneEffectPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("WaterFiendKraken").Type))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss2");
                        Priority = (SceneEffectPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Chaos").Type))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss4");
                        Priority = (SceneEffectPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("DarkCloud").Type))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss1");
                        Priority = (SceneEffectPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Death").Type))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss6");
                        Priority = (SceneEffectPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("WyvernMageShadow").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("GhostDragonHead").Type))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss4");
                        Priority = (SceneEffectPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("HellkiteDragonHead").Type))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss2");
                        Priority = (SceneEffectPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("SeathTheScalelessHead").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("PrimordialCrystal").Type))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss5");
                        Priority = (SceneEffectPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("AbysmalOolacileSorcerer").Type))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss3");
                        Priority = (SceneEffectPriority)7;
                    }
                    if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Gwyn").Type))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss5");
                        Priority = (SceneEffectPriority)7;
                    }
                    //Attraidies 4 phases
                    if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("DarkShogunMask").Type)) //phase 1
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss5");
                        Priority = (SceneEffectPriority)7;
                    }
                    else if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("DarkDragonMask").Type)) //phase 2
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss5");
                        Priority = (SceneEffectPriority)7;
                    }
                    else if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Okiku").Type)) //phase 3a
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss5");
                        Priority = (SceneEffectPriority)7;
                    }
                    else if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("BrokenOkiku").Type)) //phase 3b
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss5");
                        Priority = (SceneEffectPriority)7;
                    }
                    else if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("AttraidiesMimic").Type)) //phase 4a
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss5");
                        Priority = (SceneEffectPriority)7;
                    }
                    else if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Attraidies").Type)) //phase 4b
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss5");
                        Priority = (SceneEffectPriority)7;
                    }
                    else if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("DarkShogunMask").Type)) //phase 4c
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss5");
                        Priority = (SceneEffectPriority)7;
                    }
                }
            }

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
