using System.Diagnostics;
using System.Net;
using Amethyst.Network.Managing;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.States;
using Terraria.Initializers;
using Terraria.IO;
using Terraria.Localization;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace Amethyst.Core.Server;

internal static class ServerLauncher
{
    internal static bool IsStarted;

    internal static void Initialize()
    {
        Thread.CurrentThread.Name = "Server Thread";
        Program.SavePath = Path.Combine("data");
        ThreadPool.SetMinThreads(8, 8);
        Program.InitializeConsoleOutput();
        Program.SetupLogging();
    }

    internal static void Start()
    {
        using var main = new Main();

        LaunchInitializer.LoadParameters(main);
        LanguageManager.Instance.SetLanguage(GameCulture.DefaultCulture);
        Lang.InitializeLegacyLocalization();

        Localization.Items.Initialize();

        // On.Terraria.Netplay.StartBroadCasting += (orig) => {};
        // On.Terraria.Netplay.StopBroadCasting += (orig) => {};

        ServerTask();
    }

    private static void ServerTask()
    {
        Main.rand = new UnifiedRandom();
        Main.dedServ = true;
        Main.showSplash = false;
        Main.instance.Initialize();

        UpdateTitle();

        AmethystLog.Startup.Verbose(nameof(ServerLauncher), "Loading worlds...");
        Main.LoadWorlds();

        string? worldPath = AmethystSession.Profile.WorldToLoad;
        if (worldPath == null)
        {
            PrintWorlds();
            return;
        }

        TileFix.Initialize();

        if (!File.Exists(worldPath) || AmethystSession.Profile.WorldRecreate)
        {
            AmethystLog.Startup.Verbose(nameof(ServerLauncher), $"Requesting world-generation {worldPath}...");

            GenerateWorld();
            CheckWorldExceptions();
        }

        LoadWorld(worldPath);
        CheckWorldExceptions();

        AmethystLog.Startup.Verbose(nameof(ServerLauncher), $"Starting server...");

        InitializeNetwork();

        ConsoleInput.Initialize();

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        Netplay._serverThread = new Thread(Netplay.ServerLoop)
        {
            IsBackground = true,
            Name = "Server Loop Thread"
        };
        Netplay._serverThread.Start();

        IsStarted = true;
        UpdateTitle();

        double num6 = 16.666666666666668;
        double num7 = 0.0;
        int num8 = 0;

        Stopwatch updateSw = new();

        List<double> timings = [];

        while (!Netplay.Disconnect)
        {
            double totalMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
            if (totalMilliseconds + num7 >= num6)
            {
                num8++;
                num7 += totalMilliseconds - num6;
                stopwatch.Reset();
                stopwatch.Start();
                if (Netplay.HasClients || AmethystKernel.Profile!.ForceUpdate)
                {
                    if (AmethystSession.Profile.DebugMode)
                    {
                        updateSw.Start();
                        Main.instance.Update(new GameTime());
                        updateSw.Stop();

                        timings.Add(updateSw.Elapsed.TotalMilliseconds);
                        updateSw.Reset();

                        if (timings.Count == 180)
                        {
                            IOrderedEnumerable<double> ordered = timings.OrderBy(p => p);

                            double totalMs = 0;
                            foreach (double ms in ordered)
                            {
                                totalMs += ms;
                            }

                            if (!AmethystKernel.Profile!.DisableFrameDebug)
                                AmethystLog.Main.Debug(
                                    nameof(ServerLauncher), $"Game Update: [Min-Max range: {Math.Ceiling(ordered.First())}-{Math.Ceiling(ordered.Last())}ms] average: {Math.Ceiling(totalMs / 180)}ms, total: {(int)totalMs}ms");

                            timings.Clear();
                        }
                    }
                    else
                    {
                        Main.instance.Update(new GameTime());
                    }
                }
                else if (Main.saveTime.IsRunning)
                {
                    Main.saveTime.Stop();
                }
                // if (Main.OnTickForThirdPartySoftwareOnly != null)
                // {
                // 	Main.OnTickForThirdPartySoftwareOnly();
                // }
                double num9 = stopwatch.Elapsed.TotalMilliseconds + num7;
                if (num9 < num6)
                {
                    int num10 = (int)(num6 - num9) - 1;
                    if (num10 > 1)
                    {
                        Thread.Sleep(num10 - 1);
                        if (!Netplay.HasClients)
                        {
                            num7 = 0.0;
                            Thread.Sleep(10);
                        }
                    }
                }
            }
            Thread.Sleep(1);
        }
    }

    private static void InitializeNetwork()
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
        NetworkManager.Initialize();
        NetworkManager.RequestListening();

        Main.maxNetPlayers = AmethystSession.Profile.MaxPlayers;
    }

    private static void UpdateTitle()
    {
        string title = $"Amethyst API (Profile: {AmethystSession.Profile.Name}) - ";

        if (IsStarted)
        {
            title += $"Running {Main.worldName}";
        }
        else
        {
            title += $"Inactive";
        }

        Console.Title = title;
    }

    private static void PrintWorlds()
    {
        AmethystLog.Startup.Info(nameof(ServerLauncher), "Available worlds:");
        foreach (WorldFileData wld in Main.WorldList)
        {
            AmethystLog.Startup.Info(nameof(ServerLauncher), $"World '{wld.Name}' from '{wld.Path}' ({wld.WorldSizeX}x{wld.WorldSizeY}, built at {wld.WorldGeneratorVersion})");
        }
        AmethystLog.Startup.Info(nameof(ServerLauncher), "Load server with '-worldpath <path>' argument.");
    }

    private static void LoadWorld(string path)
    {
        Main.instance.SetWorld(path, false);

        Task task = WorldGen.serverLoadWorld();
        string oldStatusText = "None...";
        while (!task.IsCompleted)
        {
            if (WorldFile.LastThrownLoadException == null)
            {
                if (oldStatusText != Main.AutogenProgress.Message)
                {
                    oldStatusText = Main.AutogenProgress.Message;

                    AmethystLog.Startup.Verbose(nameof(ServerLauncher), $"Loading world '{path}': '{Main.AutogenProgress._totalProgress}'");
                }
            }
            Thread.Sleep(100);
        }
    }

    private static void GenerateWorld()
    {
        Profiles.ServerProfile profile = AmethystSession.Profile;

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
                    nameof(ServerLauncher), $"Generating world '{Main.worldName}': {Math.Ceiling(generationProgress.TotalProgress * 100),5}%, {generationProgress.Message,30}");
            }
            Thread.Sleep(100);
        }
    }

    private static void CheckWorldExceptions()
    {
        if (WorldFile.LastThrownLoadException != null)
        {
            AmethystLog.Startup.Critical(nameof(ServerLauncher), $"Failed to load world '{Main.worldName}':");
            AmethystLog.Startup.Critical(nameof(ServerLauncher), WorldFile.LastThrownLoadException.ToString());
        }
    }
}
