using System.CommandLine;
using System.Diagnostics;
using System.Reflection;
using Amethyst.Core.Profiles;
using Amethyst.Core.Server;
using Amethyst.Players;
using Terraria.IO;

namespace Amethyst.Core;

internal static class AmethystKernel
{
    internal static ServerProfile? Profile;

    internal static void Main(string[] args)
    {
        Console.CancelKeyPress += OnCancelKeyPress;

        AppDomain.CurrentDomain.AssemblyResolve += delegate (object? sender, ResolveEventArgs sargs)
        {
            string resourceName = new AssemblyName(sargs.Name).Name + ".dll";

            string path = Path.Combine("deps", resourceName);

            return File.Exists(path) ? Assembly.LoadFrom(path) : null;
        };

        RootCommand rootCommand = CommandConfiguration.BuildRootCommand();

        rootCommand.Invoke(args);

        if (Profile != null)
        {
            InitializeServer(Profile);
            return;
        }
    }

    private static void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
    {
        e.Cancel = true;
        StopServer();
    }

    internal static void StopServer()
    {
        AmethystLog.System.Critical("AmethystKernel.StopServer", $"Server is stopping...");

        if (!ServerLauncher.IsStarted)
        {
            AmethystLog.System.Critical("AmethystKernel.StopServer", $"Server was not fully loaded -> direct stopping...");

            Environment.Exit(0);

            return;
        }

        Stopwatch sw = new();

        sw.Start();

        WorldFile.SaveWorld();

        sw.Stop();

        AmethystLog.System.Info("AmethystKernel.StopServer", $"Saved world in {sw.Elapsed.TotalSeconds}s ({sw.ElapsedMilliseconds}ms).");

        DeinitializeServer();

        AmethystLog.System.Info("AmethystKernel.StopServer", $"Exiting server...");
        Environment.Exit(0);
    }

    internal static void DeinitializeServer()
    {
        foreach (NetPlayer plr in PlayerManager.Tracker.NonNullable)
        {
            plr.Kick("amethyst.serverStopped");
            plr.Character?.Save();
            plr.UnloadExtensions();

            AmethystLog.System.Info("AmethystKernel.StopServer", $"Player {plr.Name} was deinitialized.");
        }
    }

    private static void InitializeServer(ServerProfile profile)
    {
        if (!Directory.Exists(profile.SavePath))
        {
            Directory.CreateDirectory(profile.SavePath);
        }

        AppDomain.CurrentDomain.FirstChanceException += (sender, ex) =>
        {
            AmethystLog.Startup.Error("ModuleLoader", $"Caught first-chance exception:");
            AmethystLog.Startup.Error("ModuleLoader", ex.Exception.ToString() ?? "No data");
        };
        AppDomain.CurrentDomain.UnhandledException += (sender, ex) =>
        {
            AmethystLog.Startup.Critical("ModuleLoader", $"Caught unhandled exception:");
            AmethystLog.Startup.Critical("ModuleLoader", ex.ExceptionObject.ToString() ?? "No data");
            AmethystLog.Startup.Critical("ModuleLoader", $"Server is terminated.");

            DeinitializeServer();

            Thread.Sleep(-1);
        };

        AmethystSession.StartServer();
    }
}
