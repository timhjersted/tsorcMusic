using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
            if (modMenuList.Count == 4 && modMenuList[3].Name == "The Story of Red Cloud")
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
        private const int AnimationFrameCount = 36;
        private const int AnimationFrameWidth = 960;
        private const int AnimationFrameHeight = 512;
        private const int AnimationAtlasColumns = 4;
        private const int AnimationFramesPerAtlas = 20;
        private const float AnimationSecondsPerFrame = 0.11f;

        private Asset<Texture2D> logo;
        private Asset<Texture2D>[] backgroundAtlases;

        public override int Music => MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Night");
        public override string Name => "The Story of Red Cloud";
        public override Asset<Texture2D> Logo => logo ??= ModContent.Request<Texture2D>("tsorcMusic/MainMenu/tsorc_logo", AssetRequestMode.ImmediateLoad);

        public override void Load()
        {
            if (!Main.dedServ)
            {
                backgroundAtlases = new[]
                {
                    ModContent.Request<Texture2D>("tsorcMusic/MainMenu/praise_the_sun_background_atlas_0", AssetRequestMode.ImmediateLoad),
                    ModContent.Request<Texture2D>("tsorcMusic/MainMenu/praise_the_sun_background_atlas_1", AssetRequestMode.ImmediateLoad)
                };
            }
        }

        public override void Unload()
        {
            logo = null;
            backgroundAtlases = null;
        }

        public override void Update(bool isOnTitleScreen)
        {
            Main.curMusic = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Night");
        }

        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            DrawMenuBackground(spriteBatch);

            Texture2D logoTexture = Logo.Value;
            float maxLogoWidth = Main.screenWidth * 0.58f;
            float maxLogoHeight = Main.screenHeight * 0.42f;
            logoScale = MathHelper.Min(maxLogoWidth / logoTexture.Width, maxLogoHeight / logoTexture.Height);
            logoRotation = 0f;
            logoDrawCenter = new Vector2(Main.screenWidth * 0.5f, Main.screenHeight * 0.18f);

            return true;
        }

        private void DrawMenuBackground(SpriteBatch spriteBatch)
        {
            if (backgroundAtlases == null)
            {
                return;
            }

            int frame = (int)(Main.GlobalTimeWrappedHourly / AnimationSecondsPerFrame) % AnimationFrameCount;
            int atlasIndex = frame / AnimationFramesPerAtlas;
            int localFrame = frame % AnimationFramesPerAtlas;
            Asset<Texture2D> atlas = backgroundAtlases[atlasIndex];
            if (atlas?.IsLoaded != true)
            {
                return;
            }

            Rectangle source = new Rectangle(
                localFrame % AnimationAtlasColumns * AnimationFrameWidth,
                localFrame / AnimationAtlasColumns * AnimationFrameHeight,
                AnimationFrameWidth,
                AnimationFrameHeight);

            float scale = MathHelper.Max((float)Main.screenWidth / AnimationFrameWidth, (float)Main.screenHeight / AnimationFrameHeight);
            Vector2 size = new Vector2(AnimationFrameWidth, AnimationFrameHeight) * scale;
            Vector2 position = new Vector2((Main.screenWidth - size.X) * 0.5f, (Main.screenHeight - size.Y) * 0.5f);

            spriteBatch.Draw(atlas.Value, position, source, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }

   

    public class tsorcMusicScene : ModSceneEffect
    {
        private ulong cachedSelectionTick = ulong.MaxValue;
        private Tuple<int, SceneEffectPriority> cachedSelection = new Tuple<int, SceneEffectPriority>(0, SceneEffectPriority.None);
        private int catacombsDeepMusic = -1;
        private bool catacombsDeepMusicWasPlaying;

        public override int Music => GetSelectedMusic().Item1;
        public override SceneEffectPriority Priority => GetSelectedMusic().Item2;

        private Tuple<int, SceneEffectPriority> GetSelectedMusic()
        {
            ulong currentTick = (ulong)Main.GameUpdateCount;
            if (cachedSelectionTick != currentTick)
            {
                cachedSelection = SelectMusic();
                cachedSelectionTick = currentTick;
            }

            return cachedSelection;
        }

        public override bool IsSceneEffectActive(Player player)
        {
            if (ModLoader.TryGetMod("tsorcRevamp", out Mod revamp))
            {
                bool remixWorld = revamp.Call("IsRemixWorld") is bool remix && remix;
                bool revampOverride = revamp.Call("HasActiveMusicOverride") is bool active && active;

                // The remix pack owns remix worlds when present. Registered revamp encounters
                // are selected by tsorcRevamp so its registry always has precedence.
                if ((remixWorld && ModLoader.HasMod("tsorcXelvaaMusic")) || revampOverride)
                {
                    return false;
                }
            }

            return true;
        }
        
        private static bool AnyTsorcRevampInvader()
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.ModNPC != null && IsTsorcRevampInvaderType(npc.ModNPC.GetType()))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsTsorcRevampInvaderType(Type type)
        {
            while (type != null)
            {
                if (type.FullName == "tsorcRevamp.NPCs.Invaders.InvaderNPC")
                {
                    return true;
                }

                type = type.BaseType;
            }

            return false;
        }

        public Tuple<int, SceneEffectPriority> SelectMusic()
        {
            // Music rules are evaluated from top to bottom. Later matching rules intentionally override earlier ones.
            int Music = 0;
            SceneEffectPriority Priority = SceneEffectPriority.None;
            if (Main.gameMenu)
            {
                // Title screen/main menu music.
                Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/VillageDay");
                Priority = SceneEffectPriority.BiomeMedium;
            }
            else // In-game music selection
            {
                if (Main.LocalPlayer.active)
                {
                    // Default overworld day music for non-special surface biomes.
                    if (Main.LocalPlayer.ZoneOverworldHeight && Main.dayTime && !Main.LocalPlayer.ZoneDesert && !Main.LocalPlayer.ZoneGlowshroom && !Main.LocalPlayer.ZoneSnow)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/OverworldDay");
                        Priority = SceneEffectPriority.BiomeMedium;
                    }

                    // Forest daytime music, including cases where the forest flag is more specific than the height check above.
                    if (Main.LocalPlayer.ZoneForest && Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/OverworldDay");
                        Priority = SceneEffectPriority.BiomeMedium;
                    }

                    // Default PHM overworld night music for non-desert and non-snow surface areas.
                    else if (Main.LocalPlayer.ZoneOverworldHeight && !Main.LocalPlayer.ZoneDesert && !Main.LocalPlayer.ZoneSnow && !Main.dayTime && !Main.hardMode)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Night");
                        Priority = SceneEffectPriority.BiomeLow;
                    }


// Default hardmode overworld night music for non-desert and non-snow surface areas.
                    else if (Main.LocalPlayer.ZoneOverworldHeight && !Main.LocalPlayer.ZoneDesert && !Main.LocalPlayer.ZoneSnow && !Main.dayTime && Main.hardMode)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/HardmodeNight");
                        Priority = SceneEffectPriority.BiomeLow;
                    }


                    // marble and granite caves, hives underground and forests underground
                    if (Main.LocalPlayer.ZoneGranite || Main.LocalPlayer.ZoneMarble || (Main.LocalPlayer.ZoneHive && !Main.LocalPlayer.ZoneOverworldHeight) || (Main.LocalPlayer.ZoneForest && Main.LocalPlayer.ZoneRockLayerHeight))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Tunnels");
                        Priority = SceneEffectPriority.BiomeMedium;
                    }

                    // Surface desert daytime, including mixed desert/jungle and desert/underground-desert edge cases.
                    if (Main.LocalPlayer.ZoneOverworldHeight && Main.dayTime && (Main.LocalPlayer.ZoneDesert || Main.LocalPlayer.ZoneUndergroundDesert))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Desert");
                        Priority = SceneEffectPriority.BossLow;
                    }

                    // Surface desert night uses the Feralas track.
                    else if (Main.LocalPlayer.ZoneOverworldHeight && !Main.dayTime && (Main.LocalPlayer.ZoneDesert || Main.LocalPlayer.ZoneUndergroundDesert))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Feralas");
                        Priority = SceneEffectPriority.BossLow;
                    }

                    // overworld and desert and daytime
                    //if (Main.LocalPlayer.ZoneOverworldHeight && Main.LocalPlayer.ZoneDesert && Main.dayTime)
                    //{
                    //    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Desert");
                    //    Priority = SceneEffectPriority.Event;
                    //}

                    // overworld and underground desert and night
                    //if (Main.LocalPlayer.ZoneOverworldHeight && Main.LocalPlayer.ZoneUndergroundDesert && !Main.dayTime)
                    //{
                    //    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/GreatUndergroundRivers");
                    //    Priority = SceneEffectPriority.BossLow;
                    //}
                    //// overworld and underground desert and day
                    //else if (Main.LocalPlayer.ZoneOverworldHeight && Main.LocalPlayer.ZoneUndergroundDesert && Main.dayTime)
                    //{
                    //      Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Desert");
                    //      Priority = SceneEffectPriority.BossLow;
                    //}

                    // overworld and desert and not underground desert
                    //else if (Main.LocalPlayer.ZoneOverworldHeight && Main.LocalPlayer.ZoneDesert && !Main.LocalPlayer.ZoneUndergroundDesert)
                    //{
                    //    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Desert");
                    //    Priority = SceneEffectPriority.BiomeHigh;
                    //}

                    // underground desert
                    if (Main.LocalPlayer.ZoneDirtLayerHeight && Main.LocalPlayer.ZoneUndergroundDesert)
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
                        Priority = SceneEffectPriority.Environment;
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

                    // Ocean, including beach/jungle overlap.
                    if (Main.LocalPlayer.ZoneBeach)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Ocean");
                        Priority = SceneEffectPriority.Event;
                    }

                    // molten temple in hell
                    else if (Main.LocalPlayer.ZoneDungeon && Main.LocalPlayer.ZoneUnderworldHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/MoltenTemple");
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

                    // Jungle by height, excluding dungeon and meteor overlaps handled elsewhere.
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

                    // Snow above ground.
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
                    // Snow below ground.
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
                    // Corruption and crimson by height, plus infected jungle/desert overlaps.
                    if (Main.LocalPlayer.ZoneCorrupt && Main.LocalPlayer.ZoneOverworldHeight && Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/CorruptionDay");
                        Priority = SceneEffectPriority.Environment;
                    }
                    else if (Main.LocalPlayer.ZoneCorrupt && Main.LocalPlayer.ZoneDirtLayerHeight && Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/CorruptionDay");
                        Priority = SceneEffectPriority.Environment;
                    }
                    else if (Main.LocalPlayer.ZoneCorrupt && Main.LocalPlayer.ZoneRockLayerHeight && Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/CorruptionUndergroundDay");
                        Priority = SceneEffectPriority.Environment;
                    }
		  else if (Main.LocalPlayer.ZoneCorrupt && Main.LocalPlayer.ZoneOverworldHeight && !Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/CorruptionNight");
                        Priority = SceneEffectPriority.Environment;
                    }
                    else if (Main.LocalPlayer.ZoneCorrupt && Main.LocalPlayer.ZoneDirtLayerHeight && !Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/CorruptionNight");
                        Priority = SceneEffectPriority.Environment;
                    }
                    else if (Main.LocalPlayer.ZoneCorrupt && Main.LocalPlayer.ZoneRockLayerHeight && !Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundCorruptionNight");
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
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Crimson");
                        Priority = SceneEffectPriority.BiomeHigh;
                    }
                    if (Main.LocalPlayer.ZoneCrimson && Main.LocalPlayer.ZoneJungle)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Crimson");
                        Priority = SceneEffectPriority.BiomeMedium;
                    }
                    // Underworld/Hell day and night.
                    else if (Main.LocalPlayer.ZoneUnderworldHeight && Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/MetroidUndergroundDepths");
                        Priority = SceneEffectPriority.Event;
                    }
                    else if (Main.LocalPlayer.ZoneUnderworldHeight && !Main.dayTime)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UnderworldNight");
                        Priority = SceneEffectPriority.Event;
                    }
                    // Glowing mushroom biome by height; dungeon overlap gets the old underground hallow track.
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
                    // Space/sky height.
                    if (Main.LocalPlayer.ZoneSkyHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Space");
                        Priority = SceneEffectPriority.Environment;
                    }
                    // Graveyard.
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
                    // Surface jungle rain overrides the generic rain track.
                    if (Main.LocalPlayer.ZoneJungle && Main.LocalPlayer.ZoneRain && Main.LocalPlayer.ZoneOverworldHeight && !Main.LocalPlayer.ZoneDungeon && !Main.LocalPlayer.ZoneMeteor)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/JungleRain");
                        Priority = SceneEffectPriority.BiomeMedium;
                    }
                    // Plain underground/cavern fallback when no special biome/event flags are present.
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
                  
                    // Celestial pillars.
                    if (Main.LocalPlayer.ZoneTowerNebula || Main.LocalPlayer.ZoneTowerSolar || Main.LocalPlayer.ZoneTowerStardust || Main.LocalPlayer.ZoneTowerVortex)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Pillars");
                        Priority = SceneEffectPriority.BossHigh;
                    }

                    // Coordinate-based Red Cloud world areas.
                    int playerX = (int)(Main.LocalPlayer.Center.X / 16f);
                    int playerY = (int)(Main.LocalPlayer.Center.Y / 16f);
                    bool playerTileInWorld = playerX >= 0 && playerX < Main.tile.Width && playerY >= 0 && playerY < Main.tile.Height;

                    // Oasis in the desert (this just prevents the music cutting to jungle for 5 sec)
                    if (playerX > 1737 && playerX < 1909 && playerY > 715 && playerY < 857)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Desert");
                        Priority = SceneEffectPriority.BiomeHigh; //Set priority
                    }

                    // Western Ocean (Nora Sea) from Overworld edge up to the edge of space, from volcano mouth to the far left
                    if (playerX > 1 && playerX < 946 && playerY > 90 && playerY < 874)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Ocean");
                        Priority = SceneEffectPriority.BiomeHigh; //Set priority
                    }

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
                        if (!Main.dayTime && Main.hardMode)
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/VillageNight"); //Set town night music for hardmode
                            Priority = SceneEffectPriority.BiomeMedium; //Set priority
                        }
                        if (!Main.dayTime && !Main.hardMode)
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/FirelinkShrine"); //Set town night music for pre-HM
                            Priority = SceneEffectPriority.BiomeMedium; //Set priority
                        }
                    }

                    // forgotten city pre-HM
                    if (!Main.LocalPlayer.ZoneOverworldHeight && !Main.LocalPlayer.ZoneUnderworldHeight && Main.dayTime && !Main.hardMode && !Main.LocalPlayer.ZoneSnow && Main.LocalPlayer.ZoneDungeon && Main.LocalPlayer.ZoneJungle)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/ForgottenCity");
                        Priority = SceneEffectPriority.Event;
                    }
                    if (!Main.LocalPlayer.ZoneOverworldHeight && !Main.LocalPlayer.ZoneUnderworldHeight && !Main.dayTime && !Main.hardMode && !Main.LocalPlayer.ZoneSnow && Main.LocalPlayer.ZoneDungeon && Main.LocalPlayer.ZoneJungle)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/GreatUndergroundRivers");
                        Priority = SceneEffectPriority.Event;
                    }

                    // Pre-HM dungeon catch-all; dungeon/jungle overlap is handled by Forgotten City above.
                    if (!Main.hardMode && Main.LocalPlayer.ZoneDungeon && !Main.LocalPlayer.ZoneJungle)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/ForgottenCity");
                        Priority = SceneEffectPriority.Event;
                    }

                    // Forgotten City hardmode experiment, currently disabled.
                    //if (Main.hardMode && (Main.LocalPlayer.ZoneDungeon && !Main.LocalPlayer.ZoneSnow || (Main.LocalPlayer.ZoneDungeon && Main.LocalPlayer.ZoneJungle)))
                    //{
                    //    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss10");
                    //    Priority = SceneEffectPriority.Event;
                    //}

                    // Wyvern Mage Fortress coordinate rectangle.
                    if (playerX > 6336 && playerX < 7385 && playerY > 90 && playerY < 608)
                    {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/WyvernMageFortress"); 
                            Priority = SceneEffectPriority.BiomeHigh; //Set priority
                    }

                    // Jungle Village coordinate rectangle.
                    if (playerX > 5805 && playerX < 5980 && playerY > 740 && playerY < 900)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/JungleVillage");
                        Priority = SceneEffectPriority.BiomeHigh; //Set priority
                    }

                    // ashen caves
                    //if (playerX > 2439 && playerX < 3100 && playerY > 1733 && playerY < 1920)
                    //{
                    //    Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Caverns");
                    //    Priority = SceneEffectPriority.BiomeHigh; //Set priority
                    //}


                    // Sky Temple islands: Empress arena, Attraidies area, Vortex island, Wise Man island, and far-left island.
                    if ((playerX > 1421 && playerX < 4973 && playerY < 470) || (playerX > 1200 && playerX < 2491 && playerY < 430) || (playerX > 44 && playerX < 1015 && playerY < 580))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/SkyTemple");
                        Priority = SceneEffectPriority.BiomeMedium; //Set priority
                    }

                    // Molten Sky Temple: hardmode, deep-map sky temple walls.
                    if (Main.hardMode && playerTileInWorld)
                    {
                        if (playerY > 1685 && Main.tile[playerX, playerY] != null && (Main.tile[playerX, playerY].WallType == WallID.PinkDungeonSlabUnsafe || Main.tile[playerX, playerY].WallType == WallID.StarlitHeavenWallpaper))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/SkyTemple");
                            Priority = SceneEffectPriority.BiomeHigh; //Set priority
                        }
                    }

                    // Water temple: hardmode green dungeon walls and ice walls.
                    if (Main.hardMode && playerTileInWorld)
                    {
                        if (Main.tile[playerX, playerY] != null && (Main.tile[playerX, playerY].WallType == WallID.GreenDungeonTileUnsafe || Main.tile[playerX, playerY].WallType == WallID.GreenDungeonSlabUnsafe || Main.tile[playerX, playerY].WallType == WallID.IceUnsafe))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/WaterTemple");
                            Priority = SceneEffectPriority.BossLow;
                        }
                    }

                    // Shadow Temple: hardmode pink dungeon or crystal walls.
                    if (Main.hardMode && playerTileInWorld)
                    {
                        if (Main.tile[playerX, playerY] != null && (Main.tile[playerX, playerY].WallType == WallID.PinkDungeonUnsafe || Main.tile[playerX, playerY].WallType == WallID.Crystal))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Dungeon");
                            Priority = SceneEffectPriority.BossLow;
                        }
                    }

                    // Catacombs of the Deep: pink dungeon tile walls.
                    if (playerTileInWorld)
                    {
                        if (Main.tile[playerX, playerY] != null && (Main.tile[playerX, playerY].WallType == WallID.PinkDungeonTileUnsafe))
                        {
                            int catacombsSlot = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Catacombs");
                            int tombSlot = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Tomb");

                            var audioTracks = ((LegacyAudioSystem)Main.audioSystem).AudioTracks;
                            bool selectedTrackIsPlaying = catacombsDeepMusic >= 0 && catacombsDeepMusic < audioTracks.Length && audioTracks[catacombsDeepMusic] != null && audioTracks[catacombsDeepMusic].IsPlaying;

                            // Roll only when entering this area, or after the selected track has played and then stops.
                            if ((catacombsDeepMusic != catacombsSlot && catacombsDeepMusic != tombSlot) || (catacombsDeepMusicWasPlaying && !selectedTrackIsPlaying))
                            {
                                catacombsDeepMusic = Main.rand.Next(3) == 0 ? tombSlot : catacombsSlot;
                                catacombsDeepMusicWasPlaying = false;
                            }
                            else if (selectedTrackIsPlaying)
                            {
                                catacombsDeepMusicWasPlaying = true;
                            }

                            Music = catacombsDeepMusic;
                            Priority = SceneEffectPriority.BossLow;
                        }
                        else
                        {
                            catacombsDeepMusic = -1;
                            catacombsDeepMusicWasPlaying = false;
                        }
                    }

                    // Pyramid coordinate rectangles.
                    if ((playerX > 5665 && playerX < 6025 && playerY > 1600 && playerY < 1805) || (playerX > 6000 && playerX < 6123 && playerY > 1671 && playerY < 1800))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Ending");
                        Priority = SceneEffectPriority.BossLow; //Set priority
                    }

                    // Old One's Tree coordinate rectangle.
                    if (playerX > 2535 && playerX < 2850 && playerY > 1055 && playerY < 1550)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/OldOnesTree");
                        Priority = SceneEffectPriority.BossLow; //Set priority
                    }

                    // Dark Tower coordinate rectangle.
                    if (playerX > 1310 && playerX < 1460 && playerY > 354 && playerY < 864)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/DarkTower");
                        Priority = SceneEffectPriority.BossLow; //Set priority
                    }

                    // Lihzahrd Temple walls, unless Golem is active.
                    if (playerTileInWorld)
                    {
                        if (Main.tile[playerX, playerY] != null && (Main.tile[playerX, playerY].WallType == WallID.LihzahrdBrickUnsafe || Main.tile[playerX, playerY].WallType == WallID.DiscWall) && !NPC.AnyNPCs(245))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/UndergroundDesert");
                            Priority = SceneEffectPriority.BossLow;
                        }
                    }

                    // Tomb of Gwyn: underground obsidian/titanstone walls above the underworld.
                    if (!Main.LocalPlayer.ZoneUnderworldHeight && playerY < 1700 && !Main.LocalPlayer.ZoneOverworldHeight && playerTileInWorld)
                    {
                        if (Main.tile[playerX, playerY] != null && (Main.tile[playerX, playerY].WallType == WallID.ObsidianBrickUnsafe || Main.tile[playerX, playerY].WallType == WallID.TitanstoneBlock))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Ending");
                            Priority = SceneEffectPriority.BossLow;
                        }
                    }
                  

                    // Witchlands: deep Titanstone Block Wall, or the underworld X range from 2629 to 3354.
                    if (playerTileInWorld)
                    {
                        bool deepTitanstoneWall = playerY >= 1700 && Main.tile[playerX, playerY] != null && Main.tile[playerX, playerY].WallType == WallID.TitanstoneBlock;
                        bool witchlandsUnderworldRange = Main.LocalPlayer.ZoneUnderworldHeight && playerX >= 2629 && playerX <= 3354;
                        if (deepTitanstoneWall || witchlandsUnderworldRange)
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Witchlands");
                            Priority = SceneEffectPriority.BossLow;
                        }
                    }
                    // Frozen Ocean coordinate rectangle, with day and night variants.
                    if (playerTileInWorld)
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

                    // Burnt Village before Eye of Cthulhu, Eater of Worlds/Brain, and Skeletron are all defeated.
                    if (playerX > 4773 && playerX < 4955 && playerY > 823 && playerY < 883 && (!NPC.downedBoss1 || !NPC.downedBoss2 || !NPC.downedBoss3)) 
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
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Eternia");
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
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss12");
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
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss23");
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
                    

                    // Old One Army.
                    if (Main.LocalPlayer.ZoneOldOneArmy)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss19");
                        Priority = SceneEffectPriority.Event;
                    }
                    // Blood Moon.
                    else if (Main.bloodMoon && Main.LocalPlayer.ZoneOverworldHeight)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Pillars");
                        Priority = SceneEffectPriority.Event;
                    }
                    // Solar Eclipse.
                    if (Main.eclipse && Main.dayTime && Main.LocalPlayer.ZoneOverworldHeight) 
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Space");
                        Priority = SceneEffectPriority.Event;
                    }
                    // Goblin Invasion
                    if (NPC.AnyNPCs(26) || NPC.AnyNPCs(27) || NPC.AnyNPCs(28) || NPC.AnyNPCs(29) || NPC.AnyNPCs(111) || NPC.AnyNPCs(471))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/GoblinInvasion");
                        Priority = SceneEffectPriority.BossMedium;
                    }
                    // Pirate Invasion (212 Pirate Deckhand, 216 Pirate Captain, 491 Flying Dutchman)
                    if (NPC.AnyNPCs(212) || NPC.AnyNPCs(216) || NPC.AnyNPCs(491))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss4");
                        Priority = SceneEffectPriority.BossMedium;
                    }
                    // Pumpkin Moon (551 Mourning Wood, 552 Pumpking)
                    if (NPC.AnyNPCs(551) || NPC.AnyNPCs(552))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss10");
                        Priority = SceneEffectPriority.BossMedium;
                    }
                    // Frost Moon (159 Everscream, 160 Santa-NK1, 161 Ice Queen)
                    if (NPC.AnyNPCs(159) || NPC.AnyNPCs(160) || NPC.AnyNPCs(161))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss4");
                        Priority = SceneEffectPriority.BossMedium;
                    }
                    // Martian Madness (381 Martian Walker, 390 Martian Saucer)
                    if (NPC.AnyNPCs(381) || NPC.AnyNPCs(390))
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss11");
                        Priority = SceneEffectPriority.BossMedium;
                    }
                    // Slime Rain.
                    if (Main.slimeRain)
                    {
                        Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Rain");
                        Priority = SceneEffectPriority.BossLow;
                    }
                    // Torch God.
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
                        // Red Cloud Invaders: default track for any active InvaderNPC subclass, with specific invaders overriding below.
                        if (AnyTsorcRevampInvader())
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/SlugBattle");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // Abyssal Ninja Invader
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("AbyssalNinjaInvader").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss24");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // Studded Leather Warrior
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("StuddedLeatherWarrior").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss22");
                            Priority = SceneEffectPriority.BossMedium;
                        }

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

                        // Hero of Lumelia
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("HeroofLumelia").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss2");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // Jungle Wyvern
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("JungleWyvernHead").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss10");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // The Rage
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("TheRage").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss7");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // The Sorrow
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("TheSorrow").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss12");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // The Hunter
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("TheHunter").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss10");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // The Triad
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Cataluminance").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("RetinazerV2").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("SpazmatismV2").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss8"); // trying boss 8 instead of 9 since 8 is only used 1 other time and it's too good to only hear once
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // Artorias
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Artorias").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss6");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // Witchking
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Witchking").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss14");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // Blight
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Blight").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Pillars");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // Wyvern Mage and Mecha Dragon
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("WyvernMage").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("MechaDragonHead").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss12");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // Gaibon and Slogra
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Gaibon").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Slogra").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss7");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // Serris and Serris X
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("SerrisHead").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("SerrisX").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss19");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // Fiends

                        // Earth Fiend Lich and Lich King serpent.
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("EarthFiendLich").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("LichKingSerpentHead").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss16");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // Fire Fiend Marilith
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("FireFiendMarilith").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss16");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // Water Fiend Kraken
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("WaterFiendKraken").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss12");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // Chaos
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Chaos").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss14");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // Dark Cloud
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("DarkCloud").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss11");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // Death
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Death").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss13");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // Wyvern Mage Shadow and Ghost Dragon
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("WyvernMageShadow").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("GhostDragonHead").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/God-DevouringSerpent");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // Hellkite Dragon
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("HellkiteDragonHead").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/God-DevouringSerpent");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // Seath the Scaleless and Primordial Crystal
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("SeathTheScalelessHead").Type) || NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("PrimordialCrystal").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss9");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // Abysmal Oolacile Sorcerer
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("AbysmalOolacileSorcerer").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Boss14");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // Gwyn
                        if (NPC.AnyNPCs(tsorcRevamp.Find<ModNPC>("Gwyn").Type))
                        {
                            Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, "Sounds/Music/Gwyn");
                            Priority = SceneEffectPriority.BossMedium;
                        }

                        // Attraidies phases
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

            // If no rule matched, still return a custom track so vanilla music has no empty slot to reclaim.
            if (Music == 0)
            {
                Music = MusicLoader.GetMusicSlot(tsorcMusic.instance, Main.dayTime ? "Sounds/Music/OverworldDay" : "Sounds/Music/Night");
                Priority = SceneEffectPriority.BiomeMedium;
            }

            // Priority only decides competition with other scene effects; rule order above decides which tsorcMusic track wins.
            // Use the maximum priority so vanilla scene effects do not seep through this always-active replacement scene.
            Priority = SceneEffectPriority.BossHigh;
            return new Tuple<int, SceneEffectPriority>(Music, Priority);
        }        
    }
}
