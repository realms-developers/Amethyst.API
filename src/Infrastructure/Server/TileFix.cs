using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.GameContent.Events;
using Terraria.IO;
using Terraria.Map;
// terrashit
#pragma warning disable CS8625

namespace Amethyst.Infrastructure.Server;

internal sealed class TileFix : WorldGen
{
    internal static void Initialize()
    {
        On.Terraria.IO.WorldFile.SaveWorldTiles += SaveWorldTiles;
        On.Terraria.WorldGen.clearWorld += OnClearWorld;
    }

    private static void OnClearWorld(On.Terraria.WorldGen.orig_clearWorld orig)
    {
        //AmethystLog.Main.Debug("TileFix", "Requested clear world operation!");

        ResetTreeShakes();
        Main.ladyBugRainBoost = 0;
        Main.getGoodWorld = false;
        Main.drunkWorld = false;
        Main.tenthAnniversaryWorld = false;
        Main.dontStarveWorld = false;
        Main.notTheBeesWorld = false;
        Main.remixWorld = false;
        Main.noTrapsWorld = false;
        Main.zenithWorld = false;
        Main.afterPartyOfDoom = false;
        Main.shimmerAlpha = 0f;
        Main.shimmerDarken = 0f;
        Main.shimmerBrightenDelay = 0f;
        NPC.ResetBadgerHatTime();
        NPC.freeCake = false;
        NPC.mechQueen = -1;
        Main.mapDelay = 2;
        Main.waterStyle = 0;
        Main.ResetWindCounter(resetExtreme: true);
        TownManager = new TownRoomManager();
        PressurePlateHelper.Reset();
        TownManager.Clear();
        NPC.ResetKillCount();
        Main.instance.ClearCachedTileDraws();
        MapHelper.ResetMapData();
        TileEntity.Clear();
        Main.checkXMas();
        Main.checkHalloween();
        DontStarveDarknessDamageDealer.Reset();
        Wiring.ClearAll();
        Main.ParticleSystem_World_BehindPlayers.Particles.Clear();
        Main.ParticleSystem_World_OverPlayers.Particles.Clear();
        if (Main.mapReady)
        {
            for (int i = 0; i < lastMaxTilesX; i++)
            {
                _ = (float)i / (float)lastMaxTilesX;
                Main.statusText = Lang.gen[65].Value;
            }
            if (Main.Map != null)
            {
                Main.Map.Clear();
            }
        }
        NPC.MoonLordCountdown = 0;
        Main.forceHalloweenForToday = false;
        Main.forceXMasForToday = false;
        NPC.RevengeManager.Reset();
        Main.pumpkinMoon = false;
        Main.clearMap = true;
        Main.mapTime = 0;
        Main.updateMap = false;
        Main.mapReady = false;
        Main.refreshMap = false;
        Main.eclipse = false;
        Main.slimeRain = false;
        Main.slimeRainTime = 0.0;
        Main.slimeWarningTime = 0;
        Main.sundialCooldown = 0;
        Main.moondialCooldown = 0;
        Main.fastForwardTimeToDawn = false;
        Main.fastForwardTimeToDusk = false;
        BirthdayParty.WorldClear();
        LanternNight.WorldClear();
        mysticLogsEvent.WorldClear();
        CreditsRollEvent.Reset();
        Sandstorm.WorldClear();
        Main.DroneCameraTracker.WorldClear();
        Main.LocalGolfState.WorldClear();
        Main.CurrentPan = Vector2.Zero;
        Main.UpdateTimeRate();
        Main.wofNPCIndex = -1;
        NPC.waveKills = 0f;
        spawnHardBoss = 0;
        totalSolid2 = 0;
        totalGood2 = 0;
        totalEvil2 = 0;
        totalBlood2 = 0;
        totalSolid = 0;
        totalGood = 0;
        totalEvil = 0;
        totalBlood = 0;
        WorldFile.ResetTemps();
        Main.maxRaining = 0f;
        totalX = 0;
        totalD = 0;
        tEvil = 0;
        tBlood = 0;
        tGood = 0;
        spawnEye = false;
        prioritizedTownNPCType = 0;
        shadowOrbCount = 0;
        altarCount = 0;
        SavedOreTiers.Copper = -1;
        SavedOreTiers.Iron = -1;
        SavedOreTiers.Silver = -1;
        SavedOreTiers.Gold = -1;
        SavedOreTiers.Cobalt = -1;
        SavedOreTiers.Mythril = -1;
        SavedOreTiers.Adamantite = -1;
        //GenVars.shimmerPosition = Vector2D.Zero;
        Main.cloudBGActive = 0f;
        Main.raining = false;
        Main.hardMode = false;
        Main.helpText = 0;
        Main.BartenderHelpTextIndex = 0;
        Main.dungeonX = 0;
        Main.dungeonY = 0;
        NPC.downedBoss1 = false;
        NPC.downedBoss2 = false;
        NPC.downedBoss3 = false;
        NPC.downedQueenBee = false;
        NPC.downedSlimeKing = false;
        NPC.downedMechBossAny = false;
        NPC.downedMechBoss1 = false;
        NPC.downedMechBoss2 = false;
        NPC.downedMechBoss3 = false;
        NPC.downedFishron = false;
        NPC.downedAncientCultist = false;
        NPC.downedMoonlord = false;
        NPC.downedHalloweenKing = false;
        NPC.downedHalloweenTree = false;
        NPC.downedChristmasIceQueen = false;
        NPC.downedChristmasSantank = false;
        NPC.downedChristmasTree = false;
        NPC.downedPlantBoss = false;
        NPC.downedGolemBoss = false;
        NPC.downedEmpressOfLight = false;
        NPC.downedQueenSlime = false;
        NPC.downedDeerclops = false;
        NPC.combatBookWasUsed = false;
        NPC.combatBookVolumeTwoWasUsed = false;
        NPC.peddlersSatchelWasUsed = false;
        NPC.savedStylist = false;
        NPC.savedGoblin = false;
        NPC.savedWizard = false;
        NPC.savedMech = false;
        NPC.savedTaxCollector = false;
        NPC.savedAngler = false;
        NPC.savedBartender = false;
        NPC.savedGolfer = false;
        NPC.boughtCat = false;
        NPC.boughtDog = false;
        NPC.boughtBunny = false;
        NPC.unlockedSlimeBlueSpawn = false;
        NPC.unlockedSlimeGreenSpawn = false;
        NPC.unlockedSlimeOldSpawn = false;
        NPC.unlockedSlimePurpleSpawn = false;
        NPC.unlockedSlimeRainbowSpawn = false;
        NPC.unlockedSlimeRedSpawn = false;
        NPC.unlockedSlimeYellowSpawn = false;
        NPC.unlockedSlimeCopperSpawn = false;
        NPC.unlockedMerchantSpawn = false;
        NPC.unlockedDemolitionistSpawn = false;
        NPC.unlockedPartyGirlSpawn = false;
        NPC.unlockedDyeTraderSpawn = false;
        NPC.unlockedTruffleSpawn = false;
        NPC.unlockedArmsDealerSpawn = false;
        NPC.unlockedNurseSpawn = false;
        NPC.unlockedPrincessSpawn = false;
        Array.Clear(NPC.ShimmeredTownNPCs, 0, NPC.ShimmeredTownNPCs.Length);
        NPC.downedGoblins = false;
        NPC.downedClown = false;
        NPC.downedFrost = false;
        NPC.downedPirates = false;
        NPC.downedMartians = false;
        NPC.downedTowerSolar = (NPC.downedTowerVortex = (NPC.downedTowerNebula = (NPC.downedTowerStardust = (NPC.LunarApocalypseIsUp = false))));
        NPC.TowerActiveSolar = (NPC.TowerActiveVortex = (NPC.TowerActiveNebula = (NPC.TowerActiveStardust = false)));
        DD2Event.ResetProgressEntirely();
        NPC.ClearFoundActiveNPCs();
        Main.BestiaryTracker.Reset();
        Main.PylonSystem.Reset();
        CreativePowerManager.Instance.Reset();
        Main.CreativeMenu.Reset();
        shadowOrbSmashed = false;
        spawnMeteor = false;
        stopDrops = false;
        Main.invasionDelay = 0;
        Main.invasionType = 0;
        Main.invasionSize = 0;
        Main.invasionWarn = 0;
        Main.invasionX = 0.0;
        Main.invasionSizeStart = 0;
        Main.treeX[0] = Main.maxTilesX;
        Main.treeX[1] = Main.maxTilesX;
        Main.treeX[2] = Main.maxTilesX;
        Main.treeStyle[0] = 0;
        Main.treeStyle[1] = 0;
        Main.treeStyle[2] = 0;
        Main.treeStyle[3] = 0;
        noLiquidCheck = false;
        Liquid.numLiquid = 0;
        LiquidBuffer.numLiquidBuffer = 0;
        if (Main.netMode == 1 || lastMaxTilesX > Main.maxTilesX || lastMaxTilesY > Main.maxTilesY)
        {
            for (int j = 0; j < lastMaxTilesX; j++)
            {
                float num = (float)j / (float)lastMaxTilesX;
                Main.statusText = Lang.gen[46].Value + " " + (int)(num * 100f + 1f) + "%";
                for (int k = 0; k < lastMaxTilesY; k++)
                {
                    Main.tile[j, k] = new();
                }
            }
        }
        lastMaxTilesX = Main.maxTilesX;
        lastMaxTilesY = Main.maxTilesY;
        if (Main.netMode != 2)
        {
            Main.sectionManager = new WorldSections(Main.maxTilesX / 200, Main.maxTilesY / 150);
        }
        if (Main.netMode != 1)
        {
            for (int l = 0; l < Main.maxTilesX; l++)
            {
                float num2 = (float)l / (float)Main.maxTilesX;
                Main.statusText = Lang.gen[47].Value + " " + (int)(num2 * 100f + 1f) + "%";
                for (int m = 0; m < Main.maxTilesY; m++)
                {
                    if (Main.tile[l, m] == null)
                    {
                        Main.tile[l, m] = new Tile();
                    }
                    else
                    {
                        Main.tile[l, m].ClearEverything();
                    }
                }
            }
        }
        for (int n = 0; n < Main.countsAsHostForGameplay.Length; n++)
        {
            Main.countsAsHostForGameplay[n] = false;
        }
        CombatText.clearAll();
        PopupText.ClearAll();
        for (int num3 = 0; num3 < 6000; num3++)
        {
            Main.dust[num3] = new Dust();
            Main.dust[num3].dustIndex = num3;
        }
        for (int num4 = 0; num4 < 600; num4++)
        {
            Main.gore[num4] = new Gore();
        }
        for (int num5 = 0; num5 < 400; num5++)
        {
            Main.item[num5] = new Item();
            Main.item[num5].whoAmI = num5;
            Main.timeItemSlotCannotBeReusedFor[num5] = 0;
        }
        for (int num6 = 0; num6 < 200; num6++)
        {
            Main.npc[num6] = new NPC();
            Main.npc[num6].whoAmI = num6;
        }
        for (int num7 = 0; num7 < 1000; num7++)
        {
            Main.projectile[num7] = new Projectile();
            Main.projectile[num7].whoAmI = num7;
        }
        for (int num8 = 0; num8 < 8000; num8++)
        {
            Main.chest[num8] = null;
        }
        for (int num9 = 0; num9 < 1000; num9++)
        {
            Main.sign[num9] = null;
        }
        for (int num10 = 0; num10 < Liquid.maxLiquid; num10++)
        {
            Main.liquid[num10] = new Liquid();
        }
        for (int num11 = 0; num11 < 50000; num11++)
        {
            Main.liquidBuffer[num11] = new LiquidBuffer();
        }
        setWorldSize();
        Star.SpawnStars();
        worldCleared = true;
    }

    private static int SaveWorldTiles(On.Terraria.IO.WorldFile.orig_SaveWorldTiles orig, object writer)
    {
        try
        {
            return SaveWorldTiles((BinaryWriter)writer);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return (int)((BinaryWriter)writer).BaseStream.Position;
    }

    static unsafe int SaveWorldTiles(BinaryWriter writer)
    {
        byte[] array = new byte[16];
        for (int i = 0; i < Main.maxTilesX; i++)
        {
            float num = (float)i / (float)Main.maxTilesX;
            Main.statusText = Lang.gen[49].Value + " " + (int)(num * 100f + 1f) + "%";
            int num2;
            for (num2 = 0; num2 < Main.maxTilesY; num2++)
            {
                TileData* tile = Main.tile[i, num2].ptr;
                int position = 4;
                byte b2;
                byte b;
                byte b3;
                byte b4 = (b3 = (b2 = (b = 0)));
                bool flag = false;
                if (tile->active())
                {
                    flag = true;
                }
                if (flag)
                {
                    b4 = (byte)(b4 | 2u);
                    array[position] = (byte)tile->type;
                    position++;
                    if (tile->type > 255)
                    {
                        array[position] = (byte)(tile->type >> 8);
                        position++;
                        b4 = (byte)(b4 | 0x20u);
                    }
                    if (Main.tileFrameImportant[tile->type])
                    {
                        array[position] = (byte)((uint)tile->frameX & 0xFFu);
                        position++;
                        array[position] = (byte)((tile->frameX & 0xFF00) >> 8);
                        position++;
                        array[position] = (byte)((uint)tile->frameY & 0xFFu);
                        position++;
                        array[position] = (byte)((tile->frameY & 0xFF00) >> 8);
                        position++;
                    }
                    if (tile->color() != 0)
                    {
                        b2 = (byte)(b2 | 8u);
                        array[position] = tile->color();
                        position++;
                    }
                }
                if (tile->wall != 0)
                {
                    b4 = (byte)(b4 | 4u);
                    array[position] = (byte)tile->wall;
                    position++;
                    if (tile->wallColor() != 0)
                    {
                        b2 = (byte)(b2 | 0x10u);
                        array[position] = tile->wallColor();
                        position++;
                    }
                }
                if (tile->liquid != 0)
                {
                    if (!tile->shimmer())
                    {
                        b4 = (tile->lava() ? ((byte)(b4 | 0x10u)) : ((!tile->honey()) ? ((byte)(b4 | 8u)) : ((byte)(b4 | 0x18u))));
                    }
                    else
                    {
                        b2 = (byte)(b2 | 0x80u);
                        b4 = (byte)(b4 | 8u);
                    }
                    array[position] = tile->liquid;
                    position++;
                }
                if (tile->wire())
                {
                    b3 = (byte)(b3 | 2u);
                }
                if (tile->wire2())
                {
                    b3 = (byte)(b3 | 4u);
                }
                if (tile->wire3())
                {
                    b3 = (byte)(b3 | 8u);
                }
                int num4 = (tile->halfBrick() ? 16 : ((tile->slope() != 0) ? (tile->slope() + 1 << 4) : 0));
                b3 = (byte)(b3 | (byte)num4);
                if (tile->actuator())
                {
                    b2 = (byte)(b2 | 2u);
                }
                if (tile->inActive())
                {
                    b2 = (byte)(b2 | 4u);
                }
                if (tile->wire4())
                {
                    b2 = (byte)(b2 | 0x20u);
                }
                if (tile->wall > 255)
                {
                    array[position] = (byte)(tile->wall >> 8);
                    position++;
                    b2 = (byte)(b2 | 0x40u);
                }
                if (tile->invisibleBlock())
                {
                    b = (byte)(b | 2u);
                }
                if (tile->invisibleWall())
                {
                    b = (byte)(b | 4u);
                }
                if (tile->fullbrightBlock())
                {
                    b = (byte)(b | 8u);
                }
                if (tile->fullbrightWall())
                {
                    b = (byte)(b | 0x10u);
                }
                int num5 = 3;
                if (b != 0)
                {
                    b2 = (byte)(b2 | 1u);
                    array[num5] = b;
                    num5--;
                }
                if (b2 != 0)
                {
                    b3 = (byte)(b3 | 1u);
                    array[num5] = b2;
                    num5--;
                }
                if (b3 != 0)
                {
                    b4 = (byte)(b4 | 1u);
                    array[num5] = b3;
                    num5--;
                }
                short num6 = 0;
                int num7 = num2 + 1;
                int num8 = Main.maxTilesY - num2 - 1;
                /*
                while (num8 > 0 && tile->isTheSameAs(Main.tile[i, num7]) && TileID.Sets.AllowsSaveCompressionBatching[tile->type])
                {
                  num6 = (short)(num6 + 1);
                  num8--;
                  num7++;
                }
                */
                num2 += num6;
                if (num6 > 0)
                {
                    array[position] = (byte)((uint)num6 & 0xFFu);
                    position++;
                    if (num6 > 255)
                    {
                        b4 = (byte)(b4 | 0x80u);
                        array[position] = (byte)((num6 & 0xFF00) >> 8);
                        position++;
                    }
                    else
                    {
                        b4 = (byte)(b4 | 0x40u);
                    }
                }
                array[num5] = b4;
                writer.Write(array, num5, position - num5);
            }
        }
        return (int)writer.BaseStream.Position;
    }
}
