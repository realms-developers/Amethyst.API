using System.Diagnostics;
using Amethyst.Infrastructure.Server;
using Amethyst.Players;
using Terraria.IO;

namespace Amethyst.Infrastructure.CLI.Handlers;

internal static class CancelKeyHandler
{
    internal static void Initialize()
    {
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;

            AmethystLog.System.Critical(nameof(CancelKeyHandler), "Server is stopping...");

            if (!ServerLauncher.IsStarted)
            {
                AmethystLog.System.Critical(nameof(CancelKeyHandler),
                    $"{(ServerLauncher.IsStarted ? string.Empty : "Server was not fully loaded -> ")}Stopping forcefully...");

                Environment.Exit(0);

                return;
            }

            Stopwatch sw = new();

            sw.Start();

            WorldFile.SaveWorld();

            sw.Stop();

            AmethystLog.System.Info(nameof(CancelKeyHandler), $"Saved world in {sw.Elapsed.TotalSeconds}s ({sw.ElapsedMilliseconds}ms).");

            DeinitializeServer();

            AmethystLog.System.Info(nameof(CancelKeyHandler), "Exiting server...");
            Environment.Exit(0);
        };
    }

    internal static void DeinitializeServer()
    {
        foreach (NetPlayer plr in PlayerManager.Tracker.NonNullable)
        {
            plr.Kick("amethyst.serverStopped");
            plr.Character?.Save();
            plr.UnloadExtensions();

            AmethystLog.System.Info(nameof(CancelKeyHandler), $"Player {plr.Name} was deinitialized.");
        }
    }
}
