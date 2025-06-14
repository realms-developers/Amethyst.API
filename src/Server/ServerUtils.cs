using System.Net;
using Amethyst.Kernel.Profiles;
using Amethyst.Kernel;
using Amethyst.Server.Entities;
using Amethyst.Server.Entities.Players;
using Amethyst.Network;
using Terraria;
using Terraria.GameContent.UI.States;
using Terraria.IO;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace Amethyst.Server;

internal static class ServerUtils
{
    internal static void InitializeServer()
    {
        Netplay.Connection.ResetSpecialFlags();
        Netplay.ResetNetDiag();
        Main.rand ??= new UnifiedRandom((int)DateTime.Now.Ticks);
        Main.myPlayer = 255;
        Netplay.ServerIP = IPAddress.Any;
        Main.menuMode = 14;
        Main.statusText = Lang.menu[8].Value;
        Main.netMode = 2;
        Netplay.Disconnect = false;
        for (int i = 0; i < 256; i++)
        {
            Netplay.Clients[i] = new RemoteClient();
            Netplay.Clients[i].Reset();
            Netplay.Clients[i].Id = i;
            Netplay.Clients[i].ReadBuffer = new byte[1024];
        }

        // Netplay.TcpListener = new RemadeTcpSocket();
        NetworkManager.IsLocked = false;

        Main.maxNetPlayers = AmethystSession.Profile.MaxPlayers;
    }

    internal static void DeinitializePlayers()
    {
        foreach (PlayerEntity plr in EntityTrackers.Players)
        {
            try
            {
                plr.Kick("amethyst.serverStopped");
                plr.Dispose();
            }
            catch (Exception ex)
            {
                AmethystLog.Main.Error(nameof(AmethystSession), $"Shutdown -> Failed to deinitialize player {plr.Name}: {ex.Message}");
            }
        }
    }

    internal static void UpdateTitle()
    {
        string title = $"Amethyst API (Profile: {AmethystSession.Profile.Name}) on TerrariaAPI {Main.versionNumber} - ";

        if (AmethystSession.Launcher.IsStarted)
        {
            title += $"Running {Main.worldName}";
        }
        else
        {
            title += $"Inactive";
        }

        Console.Title = title;
    }

    internal static void PrintWorlds()
    {
        AmethystLog.Startup.Info("TAPI", "Available worlds:");
        foreach (WorldFileData wld in Main.WorldList)
        {
            AmethystLog.Startup.Info("TAPI", $"World '{wld.Name}' from '{wld.Path}' ({wld.WorldSizeX}x{wld.WorldSizeY}, built at {wld.WorldGeneratorVersion})");
        }
        AmethystLog.Startup.Info("TAPI", "Load server with '-worldpath <path>' argument.");
    }

    internal static void LoadWorld(string path)
    {
        Main.instance.SetWorld(path, false);

        AmethystLog.Startup.Verbose("TAPI", $"Loading world '{path}'...");
        WorldGen.serverLoadWorld().Wait();
    }

    internal static void GenerateWorld()
    {
        ServerProfile profile = AmethystSession.Profile;

        for (int x = 0; x < Main.maxTilesX; x++)
        {
            for (int y = 0; y < Main.maxTilesY; y++)
            {
                Main.tile.Tiles[y * 8401 + x] = new TileData();
            }
        }

        Main.maxTilesX = profile.GenerationRules.Width ?? 8400;
        Main.maxTilesY = profile.GenerationRules.Height ?? 2400;

        Main.GameMode = profile.GenerationRules.GameMode;
        WorldGen.WorldGenParam_Evil = profile.GenerationRules.Evil;
        Main.worldName = Main.newWorldName = profile.GenerationRules.Name ?? profile.Name;
        string? seed = profile.GenerationRules.Seed?.Trim() ?? null;

        Main.ActiveWorldFileData = WorldFile.CreateMetadata(Main.worldName, false, Main.GameMode);

        if (string.IsNullOrEmpty(seed) || seed.Length == 0)
        {
            Main.ActiveWorldFileData.SetSeedToRandom();
        }
        else
        {
            Main.ActiveWorldFileData.SetSeed(seed);
        }

        UIWorldCreation.ProcessSpecialWorldSeeds(seed ?? "");

        Main.menuMode = 10;

        GenerationProgress generationProgress = new();
        Task task = WorldGen.CreateNewWorld(generationProgress);

        string oldStatusText = "None...";

        Thread.Sleep(2000);

        while (!task.IsCompleted)
        {
            if (oldStatusText != generationProgress.Message)
            {
                oldStatusText = generationProgress.Message;

                Console.CursorTop--;
                AmethystLog.Startup.Verbose(
                    nameof(ServerUtils), $"Generating world '{Main.worldName}': {Math.Ceiling(generationProgress.TotalProgress * 100),5}%, {generationProgress.Message,30}");
            }
            Thread.Sleep(100);
        }
    }

    internal static void CheckWorldExceptions()
    {
        if (WorldFile.LastThrownLoadException != null)
        {
            AmethystLog.Startup.Critical(nameof(ServerUtils), $"Failed to load world '{Main.worldName}':");
            AmethystLog.Startup.Critical(nameof(ServerUtils), WorldFile.LastThrownLoadException.ToString());
        }
    }
}
