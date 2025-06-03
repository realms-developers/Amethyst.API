using Amethyst.Kernel;
using Amethyst.Network.Handling.Packets;
using Amethyst.Network.Packets;
using Amethyst.Network.Structures;
using Terraria;
using Terraria.GameContent.Events;

namespace Amethyst.Network.Handling;

public static class PacketSendingUtility
{
    public static byte[] CreateWorldInfoPacket()
    {
        NetBitsByte flags = 0;
        flags[0] = Main.dayTime;
        flags[1] = Main.bloodMoon;
        flags[2] = Main.eclipse;

        BitsByte flags1 = (byte)0;
        flags1[0] = WorldGen.shadowOrbSmashed;
        flags1[1] = NPC.downedBoss1;
        flags1[2] = NPC.downedBoss2;
        flags1[3] = NPC.downedBoss3;
        flags1[4] = Main.hardMode;
        flags1[5] = NPC.downedClown;
        flags1[6] = AmethystSession.Profile.SSCMode;
        flags1[7] = NPC.downedPlantBoss;

        BitsByte flags2 = (byte)0;
        flags2[0] = NPC.downedMechBoss1;
        flags2[1] = NPC.downedMechBoss2;
        flags2[2] = NPC.downedMechBoss3;
        flags2[3] = NPC.downedMechBossAny;
        flags2[4] = Main.cloudBGActive >= 1f;
        flags2[5] = WorldGen.crimson;
        flags2[6] = Main.pumpkinMoon;
        flags2[7] = Main.snowMoon;

        BitsByte flags3 = (byte)0;
        flags3[1] = Main.fastForwardTimeToDawn;
        flags3[2] = Main.slimeRain;
        flags3[3] = NPC.downedSlimeKing;
        flags3[4] = NPC.downedQueenBee;
        flags3[5] = NPC.downedFishron;
        flags3[6] = NPC.downedMartians;
        flags3[7] = NPC.downedAncientCultist;

        BitsByte flags4 = (byte)0;
        flags4[0] = NPC.downedMoonlord;
        flags4[1] = NPC.downedHalloweenKing;
        flags4[2] = NPC.downedHalloweenTree;
        flags4[3] = NPC.downedChristmasIceQueen;
        flags4[4] = NPC.downedChristmasSantank;
        flags4[5] = NPC.downedChristmasTree;
        flags4[6] = NPC.downedGolemBoss;
        flags4[7] = BirthdayParty.PartyIsUp;

        BitsByte flags5 = (byte)0;
        flags5[0] = NPC.downedPirates;
        flags5[1] = NPC.downedFrost;
        flags5[2] = NPC.downedGoblins;
        flags5[3] = Sandstorm.Happening;
        flags5[4] = DD2Event.Ongoing;
        flags5[5] = DD2Event.DownedInvasionT1;
        flags5[6] = DD2Event.DownedInvasionT2;
        flags5[7] = DD2Event.DownedInvasionT3;

        BitsByte flags6 = (byte)0;
        flags6[0] = NPC.combatBookWasUsed;
        flags6[1] = LanternNight.LanternsUp;
        flags6[2] = NPC.downedTowerSolar;
        flags6[3] = NPC.downedTowerVortex;
        flags6[4] = NPC.downedTowerNebula;
        flags6[5] = NPC.downedTowerStardust;
        flags6[6] = Main.forceHalloweenForToday;
        flags6[7] = Main.forceXMasForToday;

        BitsByte flags7 = (byte)0;
        flags7[0] = NPC.boughtCat;
        flags7[1] = NPC.boughtDog;
        flags7[2] = NPC.boughtBunny;
        flags7[3] = NPC.freeCake;
        flags7[4] = Main.drunkWorld;
        flags7[5] = NPC.downedEmpressOfLight;
        flags7[6] = NPC.downedQueenSlime;
        flags7[7] = Main.getGoodWorld;

        BitsByte flags8 = (byte)0;
        flags8[0] = Main.tenthAnniversaryWorld;
        flags8[1] = Main.dontStarveWorld;
        flags8[2] = NPC.downedDeerclops;
        flags8[3] = Main.notTheBeesWorld;
        flags8[4] = Main.remixWorld;
        flags8[5] = NPC.unlockedSlimeBlueSpawn;
        flags8[6] = NPC.combatBookVolumeTwoWasUsed;
        flags8[7] = NPC.peddlersSatchelWasUsed;

        BitsByte flags9 = (byte)0;
        flags9[0] = NPC.unlockedSlimeGreenSpawn;
        flags9[1] = NPC.unlockedSlimeOldSpawn;
        flags9[2] = NPC.unlockedSlimePurpleSpawn;
        flags9[3] = NPC.unlockedSlimeRainbowSpawn;
        flags9[4] = NPC.unlockedSlimeRedSpawn;
        flags9[5] = NPC.unlockedSlimeYellowSpawn;
        flags9[6] = NPC.unlockedSlimeCopperSpawn;
        flags9[7] = Main.fastForwardTimeToDusk;

        BitsByte flags10 = (byte)0;
        flags10[0] = Main.noTrapsWorld;
        flags10[1] = Main.zenithWorld;
        flags10[2] = NPC.unlockedTruffleSpawn;

        WorldInfo wldInfo = new WorldInfo()
        {
            Time = (int)Main.time,
            MoonPhase = (byte)Main.moonPhase,
            MoonType = (byte)Main.moonType,

            Background0 = (byte)WorldGen.treeBG1,
            Background10 = (byte)WorldGen.treeBG2,
            Background11 = (byte)WorldGen.treeBG3,
            Background12 = (byte)WorldGen.treeBG4,
            Background1 = (byte)WorldGen.corruptBG,
            Background2 = (byte)WorldGen.jungleBG,
            Background3 = (byte)WorldGen.snowBG,
            Background4 = (byte)WorldGen.hallowBG,
            Background5 = (byte)WorldGen.crimsonBG,
            Background6 = (byte)WorldGen.desertBG,
            Background7 = (byte)WorldGen.oceanBG,
            Background8 = (byte)WorldGen.mushroomBG,
            Background9 = (byte)WorldGen.underworldBG,
            BackgroundIceStyle = (byte)Main.iceBackStyle,
            BackgroundJungleStyle = (byte)Main.jungleBackStyle,
            BackgroundHellStyle = (byte)Main.hellBackStyle,

            WindSpeedTarget = Main.windSpeedTarget,
            CloudsCount = (byte)Main.numClouds,

            TreeX1 = Main.treeX[0],
            TreeX2 = Main.treeX[1],
            TreeX3 = Main.treeX[2],
            TreeStyle1 = (byte)Main.treeStyle[0],
            TreeStyle2 = (byte)Main.treeStyle[1],
            TreeStyle3 = (byte)Main.treeStyle[2],
            TreeStyle4 = (byte)Main.treeStyle[3],
            BackgroundCaveX1 = Main.caveBackX[0],
            BackgroundCaveX2 = Main.caveBackX[1],
            BackgroundCaveX3 = Main.caveBackX[2],
            BackgroundCaveStyle1 = (byte)Main.caveBackStyle[0],
            BackgroundCaveStyle2 = (byte)Main.caveBackStyle[1],
            BackgroundCaveStyle3 = (byte)Main.caveBackStyle[2],

            TreeTops = WorldGen.TreeTops._variations.Select(p => (byte)p).ToArray(),

            MaxRaining = Main.maxRaining,

            Flags = flags,
            Flags1 = flags1,
            Flags2 = flags2,
            Flags3 = flags3,
            Flags4 = flags4,
            Flags5 = flags5,
            Flags6 = flags6,
            Flags7 = flags7,
            Flags8 = flags8,
            Flags9 = flags9,
            Flags10 = flags10,

            SundialCooldown = (byte)Main.sundialCooldown,
            MoondialCooldown = (byte)Main.moondialCooldown,

            TierCopper = (short)WorldGen.SavedOreTiers.Copper,
            TierIron = (short)WorldGen.SavedOreTiers.Iron,
            TierSilver = (short)WorldGen.SavedOreTiers.Silver,
            TierGold = (short)WorldGen.SavedOreTiers.Gold,
            TierCobalt = (short)WorldGen.SavedOreTiers.Cobalt,
            TierMythril = (short)WorldGen.SavedOreTiers.Mythril,
            TierAdamantite = (short)WorldGen.SavedOreTiers.Adamantite,

            InvasionType = (sbyte)Main.invasionType,
            LobbyID = Main.LobbyId,
            IntendedSeverity = Sandstorm.IntendedSeverity,

            WorldWidth = (short)Main.maxTilesX,
            WorldHeight = (short)Main.maxTilesY,

            SpawnX = (short)Main.spawnTileX,
            SpawnY = (short)Main.spawnTileY,

            SurfaceLevel = (short)Main.worldSurface,
            RockLevel = (short)Main.rockLayer,

            WorldID = PacketsNetworkConfiguration.Instance.FakeWorldID ?? Main.worldID,
            WorldName = PacketsNetworkConfiguration.Instance.FakeWorldName ?? Main.worldName,
            WorldUID = Main.ActiveWorldFileData?.UniqueId.ToByteArray() ?? new byte[16],
            GeneratorVersion = Main.ActiveWorldFileData?.WorldGeneratorVersion ?? 0,

            GameMode = (byte)Main.GameMode,
        };

        return WorldInfoPacket.Serialize(wldInfo);
    }
}
