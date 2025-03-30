using Amethyst.Commands;
using Amethyst.Core;
using Amethyst.Network;
using Amethyst.Network.Managing;
using Amethyst.Network.Packets;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.Social;

namespace Amethyst.Players;

internal static class PlayerNetworking
{
    internal static void Initialize()
    {
        InPacket.Initialize();
        OutPacket.Initialize();

        InModules.Initialize();
    }

    private static class InPacket
    {
        internal static void Initialize()
        {
            // Authorization
            NetworkManager.Binding.AddInPacket(PacketTypes.ClientUUID, OnPlayerUUID);
            NetworkManager.Binding.AddInPacket(PacketTypes.PlayerInfo, OnPlayerInfo);

            // SSC
            if (PlayerManager.IsSSCEnabled)
            {
                NetworkManager.Binding.AddInPacket(PacketTypes.PlayerSlot, OnPlayerSlot);
                NetworkManager.Binding.AddInPacket(PacketTypes.PlayerHp, OnPlayerHp);
                NetworkManager.Binding.AddInPacket(PacketTypes.PlayerMana, OnPlayerMana);
                NetworkManager.Binding.AddInPacket(PacketTypes.CompleteAnglerQuest, OnPlayerQuests);
                NetworkManager.Binding.AddInPacket(PacketTypes.RequestWorldInfo, OnRequestWorldInfo);
            }

            // Greeting

            NetworkManager.Binding.AddInPacket(PacketTypes.PlayerSpawn, OnPlayerSpawn);
        }

        private static void OnPlayerSpawn(in IncomingPacket packet, PacketHandleResult result)
        {
            if (packet.Player._sentSpawnPacket)
            {
                return;
            }

            packet.Player._sentSpawnPacket = true;

            if (Netplay.Clients[packet.Player.Index].State < 3)
            {
                return;
            }

            packet.Player._wasSpawned = true;
        }

        private static void OnRequestWorldInfo(in IncomingPacket packet, PacketHandleResult result)
        {
            packet.Player.Character!.SyncCharacter();
            packet.Player.Character.SetLife(SSC.Enums.SyncType.Broadcast, packet.Player.Character.LifeMax, null);
            packet.Player.Character.SetMana(SSC.Enums.SyncType.Broadcast, packet.Player.Character.ManaMax, null);
            packet.Player.Character.IsReadonly = false;
        }

        private static void OnPlayerQuests(in IncomingPacket packet, PacketHandleResult result) => packet.Player.Character!.ReceiveQuests(packet);

        private static void OnPlayerMana(in IncomingPacket packet, PacketHandleResult result) => packet.Player.Character!.ReceiveSetMana(packet);

        private static void OnPlayerHp(in IncomingPacket packet, PacketHandleResult result) => packet.Player.Character!.ReceiveSetLife(packet);

        private static void OnPlayerSlot(in IncomingPacket packet, PacketHandleResult result) => packet.Player.Character!.ReceiveSlot(packet);

        private static void OnPlayerInfo(in IncomingPacket packet, PacketHandleResult result)
        {
            BinaryReader reader = packet.GetReader();

            reader.BaseStream.Position += 3;
            string name = reader.ReadString();
            if (packet.Player.Name != "" && packet.Player.Name != name)
            {
                packet.Player.Kick(Localization.Get("network.invalidNameChange", packet.Player.Language));
                result.Ignore(Localization.Get("network.invalidNameChange", "en"));
                return;
            }

            if (packet.Player.Name == "")
            {
                packet.Player.SetName(name, false);
                if (PlayerManager.IsSSCEnabled)
                {
                    NetMessage.SendData(7, packet.Player.Index);
                    packet.Player.Character = PlayerManager.SSCProvider.CreateServersideWrapper(packet.Player);
                    packet.Player.Character.IsReadonly = true;
                }
                AmethystLog.Network.Verbose("Players", $"Player '{name}' is joining to server...");
            }

            packet.Player.Character?.ReceivePlayerInfo(packet);
        }

        private static void OnPlayerUUID(in IncomingPacket packet, PacketHandleResult result)
        {
            if (packet.Player.UUID != "")
            {
                result.Ignore(Localization.Get("network.invalidUuidChange", "en"));
                return;
            }

            BinaryReader reader = packet.GetReader();

            string uuid = reader.ReadString();
            if (!Guid.TryParse(uuid, out _))
            {
                packet.Player.Kick(Localization.Get("network.invalidUUID", packet.Player.Language));
                result.Ignore(Localization.Get("network.invalidUUID", "en"));
                return;
            }

            packet.Player.UUID = uuid;
        }
    }

    private static class OutPacket
    {
        internal static void Initialize()
        {
            NetworkManager.Binding.ReplaceOutPacket(PacketTypes.WorldInfo, OnWorldInfoSSC);
        }

        private static void OnWorldInfoSSC(in OutcomingPacket packet, PacketHandleResult result)
        {
            Main.ServerSideCharacter = AmethystSession.Profile.SSCMode;

            PacketWriter writer = new PacketWriter().SetType(7);
            BinaryWriter binaryWriter = writer.writer;

            binaryWriter.Write((int)Main.time);
            BitsByte bitsByte5 = (byte)0;
            bitsByte5[0] = Main.dayTime;
            bitsByte5[1] = Main.bloodMoon;
            bitsByte5[2] = Main.eclipse;
            binaryWriter.Write(bitsByte5);
            binaryWriter.Write((byte)Main.moonPhase);
            binaryWriter.Write((short)Main.maxTilesX);
            binaryWriter.Write((short)Main.maxTilesY);
            binaryWriter.Write((short)Main.spawnTileX);
            binaryWriter.Write((short)Main.spawnTileY);
            binaryWriter.Write((short)Main.worldSurface);
            binaryWriter.Write((short)Main.rockLayer);
            binaryWriter.Write(Main.worldID);
            binaryWriter.Write(Main.worldName);
            binaryWriter.Write((byte)Main.GameMode);
            binaryWriter.Write(Main.ActiveWorldFileData.UniqueId.ToByteArray());
            binaryWriter.Write(Main.ActiveWorldFileData.WorldGeneratorVersion);
            binaryWriter.Write((byte)Main.moonType);
            binaryWriter.Write((byte)WorldGen.treeBG1);
            binaryWriter.Write((byte)WorldGen.treeBG2);
            binaryWriter.Write((byte)WorldGen.treeBG3);
            binaryWriter.Write((byte)WorldGen.treeBG4);
            binaryWriter.Write((byte)WorldGen.corruptBG);
            binaryWriter.Write((byte)WorldGen.jungleBG);
            binaryWriter.Write((byte)WorldGen.snowBG);
            binaryWriter.Write((byte)WorldGen.hallowBG);
            binaryWriter.Write((byte)WorldGen.crimsonBG);
            binaryWriter.Write((byte)WorldGen.desertBG);
            binaryWriter.Write((byte)WorldGen.oceanBG);
            binaryWriter.Write((byte)WorldGen.mushroomBG);
            binaryWriter.Write((byte)WorldGen.underworldBG);
            binaryWriter.Write((byte)Main.iceBackStyle);
            binaryWriter.Write((byte)Main.jungleBackStyle);
            binaryWriter.Write((byte)Main.hellBackStyle);
            binaryWriter.Write(Main.windSpeedTarget);
            binaryWriter.Write((byte)Main.numClouds);
            for (int n = 0; n < 3; n++)
            {
                binaryWriter.Write(Main.treeX[n]);
            }
            for (int num8 = 0; num8 < 4; num8++)
            {
                binaryWriter.Write((byte)Main.treeStyle[num8]);
            }
            for (int num9 = 0; num9 < 3; num9++)
            {
                binaryWriter.Write(Main.caveBackX[num9]);
            }
            for (int num10 = 0; num10 < 4; num10++)
            {
                binaryWriter.Write((byte)Main.caveBackStyle[num10]);
            }
            WorldGen.TreeTops.SyncSend(binaryWriter);
            if (!Main.raining)
            {
                Main.maxRaining = 0f;
            }
            binaryWriter.Write(Main.maxRaining);
            BitsByte bitsByte6 = (byte)0;
            bitsByte6[0] = WorldGen.shadowOrbSmashed;
            bitsByte6[1] = NPC.downedBoss1;
            bitsByte6[2] = NPC.downedBoss2;
            bitsByte6[3] = NPC.downedBoss3;
            bitsByte6[4] = Main.hardMode;
            bitsByte6[5] = NPC.downedClown;
            bitsByte6[6] = Main.ServerSideCharacter;
            bitsByte6[7] = NPC.downedPlantBoss;
            binaryWriter.Write(bitsByte6);
            BitsByte bitsByte7 = (byte)0;
            bitsByte7[0] = NPC.downedMechBoss1;
            bitsByte7[1] = NPC.downedMechBoss2;
            bitsByte7[2] = NPC.downedMechBoss3;
            bitsByte7[3] = NPC.downedMechBossAny;
            bitsByte7[4] = Main.cloudBGActive >= 1f;
            bitsByte7[5] = WorldGen.crimson;
            bitsByte7[6] = Main.pumpkinMoon;
            bitsByte7[7] = Main.snowMoon;
            binaryWriter.Write(bitsByte7);
            BitsByte bitsByte8 = (byte)0;
            bitsByte8[1] = Main.fastForwardTimeToDawn;
            bitsByte8[2] = Main.slimeRain;
            bitsByte8[3] = NPC.downedSlimeKing;
            bitsByte8[4] = NPC.downedQueenBee;
            bitsByte8[5] = NPC.downedFishron;
            bitsByte8[6] = NPC.downedMartians;
            bitsByte8[7] = NPC.downedAncientCultist;
            binaryWriter.Write(bitsByte8);
            BitsByte bitsByte9 = (byte)0;
            bitsByte9[0] = NPC.downedMoonlord;
            bitsByte9[1] = NPC.downedHalloweenKing;
            bitsByte9[2] = NPC.downedHalloweenTree;
            bitsByte9[3] = NPC.downedChristmasIceQueen;
            bitsByte9[4] = NPC.downedChristmasSantank;
            bitsByte9[5] = NPC.downedChristmasTree;
            bitsByte9[6] = NPC.downedGolemBoss;
            bitsByte9[7] = BirthdayParty.PartyIsUp;
            binaryWriter.Write(bitsByte9);
            BitsByte bitsByte10 = (byte)0;
            bitsByte10[0] = NPC.downedPirates;
            bitsByte10[1] = NPC.downedFrost;
            bitsByte10[2] = NPC.downedGoblins;
            bitsByte10[3] = Sandstorm.Happening;
            bitsByte10[4] = DD2Event.Ongoing;
            bitsByte10[5] = DD2Event.DownedInvasionT1;
            bitsByte10[6] = DD2Event.DownedInvasionT2;
            bitsByte10[7] = DD2Event.DownedInvasionT3;
            binaryWriter.Write(bitsByte10);
            BitsByte bitsByte11 = (byte)0;
            bitsByte11[0] = NPC.combatBookWasUsed;
            bitsByte11[1] = LanternNight.LanternsUp;
            bitsByte11[2] = NPC.downedTowerSolar;
            bitsByte11[3] = NPC.downedTowerVortex;
            bitsByte11[4] = NPC.downedTowerNebula;
            bitsByte11[5] = NPC.downedTowerStardust;
            bitsByte11[6] = Main.forceHalloweenForToday;
            bitsByte11[7] = Main.forceXMasForToday;
            binaryWriter.Write(bitsByte11);
            BitsByte bitsByte12 = (byte)0;
            bitsByte12[0] = NPC.boughtCat;
            bitsByte12[1] = NPC.boughtDog;
            bitsByte12[2] = NPC.boughtBunny;
            bitsByte12[3] = NPC.freeCake;
            bitsByte12[4] = Main.drunkWorld;
            bitsByte12[5] = NPC.downedEmpressOfLight;
            bitsByte12[6] = NPC.downedQueenSlime;
            bitsByte12[7] = Main.getGoodWorld;
            binaryWriter.Write(bitsByte12);
            BitsByte bitsByte13 = (byte)0;
            bitsByte13[0] = Main.tenthAnniversaryWorld;
            bitsByte13[1] = Main.dontStarveWorld;
            bitsByte13[2] = NPC.downedDeerclops;
            bitsByte13[3] = Main.notTheBeesWorld;
            bitsByte13[4] = Main.remixWorld;
            bitsByte13[5] = NPC.unlockedSlimeBlueSpawn;
            bitsByte13[6] = NPC.combatBookVolumeTwoWasUsed;
            bitsByte13[7] = NPC.peddlersSatchelWasUsed;
            binaryWriter.Write(bitsByte13);
            BitsByte bitsByte14 = (byte)0;
            bitsByte14[0] = NPC.unlockedSlimeGreenSpawn;
            bitsByte14[1] = NPC.unlockedSlimeOldSpawn;
            bitsByte14[2] = NPC.unlockedSlimePurpleSpawn;
            bitsByte14[3] = NPC.unlockedSlimeRainbowSpawn;
            bitsByte14[4] = NPC.unlockedSlimeRedSpawn;
            bitsByte14[5] = NPC.unlockedSlimeYellowSpawn;
            bitsByte14[6] = NPC.unlockedSlimeCopperSpawn;
            bitsByte14[7] = Main.fastForwardTimeToDusk;
            binaryWriter.Write(bitsByte14);
            BitsByte bitsByte15 = (byte)0;
            bitsByte15[0] = Main.noTrapsWorld;
            bitsByte15[1] = Main.zenithWorld;
            bitsByte15[2] = NPC.unlockedTruffleSpawn;
            binaryWriter.Write(bitsByte15);
            binaryWriter.Write((byte)Main.sundialCooldown);
            binaryWriter.Write((byte)Main.moondialCooldown);
            binaryWriter.Write((short)WorldGen.SavedOreTiers.Copper);
            binaryWriter.Write((short)WorldGen.SavedOreTiers.Iron);
            binaryWriter.Write((short)WorldGen.SavedOreTiers.Silver);
            binaryWriter.Write((short)WorldGen.SavedOreTiers.Gold);
            binaryWriter.Write((short)WorldGen.SavedOreTiers.Cobalt);
            binaryWriter.Write((short)WorldGen.SavedOreTiers.Mythril);
            binaryWriter.Write((short)WorldGen.SavedOreTiers.Adamantite);
            binaryWriter.Write((sbyte)Main.invasionType);
            if (SocialAPI.Network != null)
            {
                binaryWriter.Write(SocialAPI.Network.GetLobbyId());
            }
            else
            {
                binaryWriter.Write(0uL);
            }
            binaryWriter.Write(Sandstorm.IntendedSeverity);

            int ignoreClient = packet.IgnoreClient;
            byte[] bytes = writer.BuildPacket();
            if (packet.RemoteClient != -1)
            {
                PlayerManager.Tracker[packet.RemoteClient].Socket.SendPacket(bytes);
            }
            else
            {
                PlayerUtilities.BroadcastPacket(bytes, p => p.Index != ignoreClient);
            }
        }
    }

    private static class InModules
    {
        internal static void Initialize()
        {
            NetworkManager.Binding.AddInModule(ModuleTypes.NetText, OnPlayerCommands);
        }

        private static void OnPlayerCommands(in IncomingModule packet, PacketHandleResult result)
        {
            if (!packet.Player.IsCapable)
            {
                result.Ignore("network.playerIsNotCapable");
                return;
            }

            BinaryReader reader = packet.GetReader();

            string command = reader.ReadString();
            string commandAdditional = reader.ReadString();

            string message = command == "Say" ? commandAdditional : $"/{command.ToLowerInvariant()} {commandAdditional}";

            if (message.StartsWith('/'))
            {
                CommandsManager.RequestRun(packet.Player, message);
                result.Ignore("network.commandUse");
                return;
            }

            AmethystLog.Network.Verbose($"Network", $"<{packet.Player.Name}> {message}");
        }
    }
}
