using System.Diagnostics;
using Amethyst.Extensions;
using Amethyst.Kernel;
using Amethyst.Kernel.Console;
using Amethyst.Network;
using Amethyst.Server.World;
using Amethyst.Systems.Console;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Initializers;
using Terraria.IO;
using Terraria.Localization;
using Terraria.Utilities;

namespace Amethyst.Server;

public sealed class TAPILauncher : IServerLauncher
{
    public static int TPS { get; private set; }

    public bool IsStarted => _isStarted;

    private bool _isStarted;

    public void Initialize()
    {
        Thread.CurrentThread.Name = "Server Thread";
        Program.SavePath = Path.Combine("data");
        ThreadPool.SetMinThreads(8, 8);
        Program.InitializeConsoleOutput();
        Program.SetupLogging();
    }

    public void StartServer()
    {
        using var main = new Main();

        LaunchInitializer.LoadParameters(main);
        LanguageManager.Instance.SetLanguage(GameCulture.DefaultCulture);
        Lang.InitializeLegacyLocalization();

        Localization.Items.Initialize();

        ServerTask();
    }


    private void ServerTask()
    {
        Main.rand = new UnifiedRandom();
        Main.dedServ = true;
        Main.showSplash = false;
        Main.instance.Initialize();

        ServerUtils.UpdateTitle();

        AmethystLog.Startup.Verbose("TAPI", "Loading worlds...");
        Main.LoadWorlds();

        string? worldPath = AmethystSession.Profile.WorldToLoad;
        if (worldPath == null)
        {
            ServerUtils.PrintWorlds();
            return;
        }

        TileFix.AttachHooks();

        if (!File.Exists(worldPath) || AmethystSession.Profile.WorldRecreate)
        {
            AmethystLog.Startup.Verbose("TAPI", $"Requesting world-generation {worldPath}...");

            ServerUtils.GenerateWorld();
            ServerUtils.CheckWorldExceptions();
        }

        ServerUtils.LoadWorld(worldPath);
        ServerUtils.CheckWorldExceptions();

        AmethystLog.Startup.Verbose("TAPI", $"Starting server...");

        ServerUtils.InitializeServer();

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        _isStarted = true;
        ServerUtils.UpdateTitle();

        double num6 = 16.666666666666668;
        double num7 = 0.0;
        int num8 = 0;

        Stopwatch updateSw = new();

        List<double> timings = [];
        AmethystLog.Startup.Verbose("TAPI", $"Server started.");

        ConsoleCommandHandler.Attach();
        _ = Task.Run(ConsoleHooks.InputTask);

        int tpsCounter = 0;
        Timer? tpsTimer = null;

        while (!Netplay.Disconnect)
        {
            double totalMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
            if (totalMilliseconds + num7 >= num6)
            {
                num8++;
                num7 += totalMilliseconds - num6;
                stopwatch.Reset();
                stopwatch.Start();
                
                if (AmethystSession.Profile.DebugMode)
                {
                    tpsCounter++;
                    updateSw.Start();
                    Main.instance.Update(new GameTime());
                    updateSw.Stop();

                    timings.Add(updateSw.Elapsed.TotalMilliseconds);
                    updateSw.Reset();
                    tpsTimer ??= new Timer(_ =>
                    {
                        int tps = Interlocked.Exchange(ref tpsCounter, 0);
                        TPS = tps;

                        if (!AmethystSession.Profile.DisableFrameDebug)
                        {
                            IOrderedEnumerable<double> ordered = timings.OrderBy(p => p);

                            double totalMs = 0;
                            foreach (double ms in ordered)
                            {
                                totalMs += ms;
                            }
                            AmethystLog.Main.Debug("TAPI", $"TPS: {tps} Game Update: [Min-Max range: {Math.Ceiling(ordered.First())}-{Math.Ceiling(ordered.Last())}ms] average: {Math.Ceiling(totalMs / 180)}ms, total: {(int)totalMs}ms");
                        }

                        timings.Clear();
                    }, null, 1000, 1000);
                }
                else
                {
                    Main.instance.Update(new GameTime());
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
                        // if (!Netplay.HasClients)
                        // {
                        //     num7 = 0.0;
                        //     Thread.Sleep(10);
                        // }
                    }
                }
            }
            Thread.Sleep(1);
        }
    }

    public void StopServer(bool force = false)
    {
        AmethystLog.Main.Critical("TAPI", "Server is stopping...");

        if (!IsStarted || force)
        {
            AmethystLog.Main.Critical("TAPI",
                $"{(IsStarted ? string.Empty : "Server was not fully loaded -> ")}Stopping forcefully...");

            Environment.Exit(0);

            return;
        }

        ServerUtils.DeinitializePlayers();

        ExtensionsOrganizer.UnloadPlugins();

        WorldFile.SaveWorld();

        AmethystLog.Main.Critical("TAPI", "Server is stopped.");
        Environment.Exit(0);
    }
}
