using System.CommandLine;
using System.Diagnostics;
using System.Reflection;
using Amethyst.Core.Profiles;
using Amethyst.Core.Server;
using Amethyst.Extensions.Modules;
using Amethyst.Players;
using Terraria.IO;

namespace Amethyst.Core;

internal static class AmethystKernel
{
    internal static ServerProfile? Profile;

    internal static void Main(string[] args)
    {
        Console.CancelKeyPress += OnCancelKeyPress;

        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
        {
            string assemblyName = new AssemblyName(args.Name).Name + ".dll";
            string path = Path.Combine(Directory.GetCurrentDirectory(), "deps", assemblyName);
            return File.Exists(path) ? Assembly.LoadFile(path) : null;
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

    internal static void StopServer(bool force = false)
    {
        AmethystLog.System.Critical(nameof(StopServer), "Server is stopping...");

        if (!ServerLauncher.IsStarted || force)
        {
            AmethystLog.System.Critical(nameof(StopServer),
                $"{(ServerLauncher.IsStarted ? string.Empty : "Server was not fully loaded -> ")}Stopping forcefully...");

            Environment.Exit(0);

            return;
        }

        Stopwatch sw = new();

        sw.Start();

        WorldFile.SaveWorld();

        sw.Stop();

        AmethystLog.System.Info(nameof(StopServer), $"Saved world in {sw.Elapsed.TotalSeconds}s ({sw.ElapsedMilliseconds}ms).");

        DeinitializeServer();

        AmethystLog.System.Info(nameof(StopServer), "Exiting server...");

        Environment.Exit(0);
    }

    internal static void DeinitializeServer()
    {
        foreach (NetPlayer plr in PlayerManager.Tracker.NonNullable)
        {
            plr.Kick("amethyst.serverStopped");
            plr.Character?.Save();
            plr.UnloadExtensions();

            AmethystLog.System.Info(nameof(StopServer), $"Player {plr.Name} was deinitialized.");
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
            AmethystLog.Startup.Error(nameof(ModuleLoader), "Caught first-chance exception:");
            AmethystLog.Startup.Error(nameof(ModuleLoader), ex.Exception.ToString() ?? "No data");
        };
        AppDomain.CurrentDomain.UnhandledException += (sender, ex) =>
        {
            AmethystLog.Startup.Critical(nameof(ModuleLoader), "Caught unhandled exception:");
            AmethystLog.Startup.Critical(nameof(ModuleLoader), ex.ExceptionObject.ToString() ?? "No data");
            AmethystLog.Startup.Critical(nameof(ModuleLoader), "Server is terminated.");

            DeinitializeServer();

            Environment.Exit(-1);
        };

        AmethystSession.StartServer();
    }
}
