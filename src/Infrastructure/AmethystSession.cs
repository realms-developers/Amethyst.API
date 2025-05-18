using System.Diagnostics;
using Amethyst.Extensions;
using Amethyst.Extensions.Base;
using Amethyst.Extensions.Base.Result;
using Amethyst.Gameplay.Players;
using Amethyst.Gameplay.Players.Auth;
using Amethyst.Infrastructure.CLI;
using Amethyst.Infrastructure.Kernel;
using Amethyst.Infrastructure.Profiles;
using Amethyst.Infrastructure.Server;
using Amethyst.Security;
using Amethyst.Systems.Commands;
using Amethyst.Systems.Permissions;
using Amethyst.Text;
using Terraria.IO;

namespace Amethyst.Infrastructure;

public static class AmethystSession
{
    static AmethystSession()
    {
        Profile = AmethystKernel.Profile!;
        OfflinePermissions = new PermissionsNode<ICommandSender>();
        PlayerPermissions = new PermissionsNode<NetPlayer>();
    }

    public static ServerProfile Profile { get; }

    public static PermissionsNode<ICommandSender> OfflinePermissions { get; }

    public static PermissionsNode<NetPlayer> PlayerPermissions { get; }

    internal static void StartServer()
    {
        ServerLauncher.Initialize();

        AmethystHooks.Initialize();

        Localization.Load();

        AuthManager.Initialize();

        CommandsManager.Initialize();

        PlayerManager.Initialize();

        SecurityManager.Initialize();

        //ExtensionsOrganizer.Initialize();

        ExtensionsOrganizer.LoadModules();
        ExtensionsOrganizer.LoadPlugins();

        PrintWelcome();

        ServerLauncher.Start();
    }

    internal static void StopServer(bool force = false)
    {
        AmethystLog.System.Critical(nameof(AmethystSession), "Server is stopping...");

        if (!ServerLauncher.IsStarted || force)
        {
            AmethystLog.System.Critical(nameof(AmethystSession),
                $"{(ServerLauncher.IsStarted ? string.Empty : "Server was not fully loaded -> ")}Stopping forcefully...");

            Environment.Exit(0);

            return;
        }

        Stopwatch sw = new();

        sw.Start();

        WorldFile.SaveWorld();

        sw.Stop();

        AmethystLog.System.Info(nameof(AmethystSession), $"Saved world in {sw.Elapsed.TotalSeconds}s ({sw.ElapsedMilliseconds}ms).");

        DeinitializeServer();

        AmethystLog.System.Info(nameof(AmethystSession), "Exiting server...");

        Environment.Exit(0);
    }

    internal static void DeinitializeServer()
    {
        foreach (NetPlayer plr in PlayerManager.Tracker.NonNullable)
        {
            plr.Kick("amethyst.serverStopped");
            plr.Character?.Save();
            plr.UnloadExtensions();

            AmethystLog.System.Info(nameof(AmethystSession), $"Player {plr.Name} was deinitialized.");
        }
    }

    private static void PrintWelcome()
    {
        Console.Clear();

        ModernConsole.WriteLine(@"
$!b$m      __                   _   _               _   
$!b$m     / /_ _ _ __ ___   ___| |_| |__  _   _ ___| |_ 
$!b$m    / / _` | '_ ` _ \ / _ \ __| '_ \| | | / __| __|
$!b$m _ / / (_| | | | | | |  __/ |_| | | | |_| \__ \ |_ 
$!b$m(_)_/ \__,_|_| |_| |_|\___|\__|_| |_|\__, |___/\__|
$!b$m                                     |___/         ");

        ModernConsole.WriteLine($"\n🛡️  $!b$mAmethyst v{typeof(AmethystSession).Assembly.GetName().Version} $!r$!bis distributed under the MIT License.");
        ModernConsole.WriteLine($"🛡️  You are free to use, modify and distribute the code, provided that the author is attributed.");

        ModernConsole.WriteLine($"\n💾 Server with profile $!d$m'$!r$m{Profile.Name}$!d'$!r runs in {(Profile.DebugMode ? "$!b$rDebug" : "$!b$!gSafe")} $!rmode{(Profile.DisableFrameDebug ? " without frame debugging" : "")}.");

        if (Profile.DebugMode)
        {
            ModernConsole.WriteLine("❗ $rThis means that more data needed for development will be logged to the console.");
            ModernConsole.WriteLine("❗ $rDebug-mode also provides $!b/grantroot $!r$rcommand that gives all permissions to player.");
            ModernConsole.WriteLine("❗ $r$!bDo not use this mode on public servers!");
        }

        ModernConsole.WriteLine($"\n⚙️  Using {(Profile.ForceUpdate ? "$gforced$!r game update cycle" : "free game update cycle")} with slots: $!b$y{Profile.MaxPlayers}$!r on $!b$y{Profile.Port}$!r");
        ModernConsole.WriteLine($"⚙️  Player characters is {(Profile.SSCMode ? "$g$!bserver-side" : "$m$!bclient-side")} $!rand default language for players is: $!d$m'$!r$m{Profile.DefaultLanguage}$!d'");

        if (Profile.WorldRecreate)
        {
            ModernConsole.WriteLine($"⚙️  $rServer will create $!b$m{Profile.GenerationRules.Width}$!rx$!b$m{Profile.GenerationRules.Height}$!r world with name $!d$m'$!r$m{Profile.GenerationRules.Name}$!d', evil: $g{Profile.GenerationRules.Evil}$!r, game mode: $!g{Profile.GenerationRules.GameMode}.\n");
        }
        else
        {
            ModernConsole.WriteLine($"⚙️  $gServer will load world located in $!d$m'$!r$m{Profile.WorldToLoad}$!d'.\n");
        }


        ModernConsole.WriteLine($"✔️  Loaded security rules:");
        foreach (string line in PagesCollection.PageifyItems(SecurityManager.Rules.Select(p => p.Key), 100))
        {
            ModernConsole.WriteLine($"$!d   {line}");
        }

        IReadOnlyDictionary<IExtension, ExtensionHandleResult> loadedMod = ExtensionsOrganizer.Modules.Repositories
            .SelectMany(r => r.ExtensionMap)
            .GroupBy(kvp => kvp.Key)
            .ToDictionary(
                group => group.Key,
                group => group.First().Value)
            .AsReadOnly();

        IReadOnlyDictionary<IExtension, ExtensionHandleResult> loadedPlug = ExtensionsOrganizer.Plugins.Repositories
            .SelectMany(r => r.ExtensionMap)
            .GroupBy(kvp => kvp.Key)
            .ToDictionary(
                group => group.Key,
                group => group.First().Value)
            .AsReadOnly();

        if (loadedMod.Any() || loadedPlug.Any())
        {
            ModernConsole.WriteLine($"\n✔️  $gLoaded$!r extensions: ($bmodules$!r, $gplugins$!r)");

            IEnumerable<string> loadedModSuccess = loadedMod
                .Where(d => d.Value.State == ExtensionResult.SuccessOperation)
                .Select(d => d.Key.Metadata.Name);

            foreach (string line in PagesCollection.PageifyItems(loadedModSuccess, 100))
            {
                ModernConsole.WriteLine($"$b   {line}");
            }

            IEnumerable<string> loadedPlugSuccess = loadedPlug
                .Where(d => d.Value.State == ExtensionResult.SuccessOperation)
                .Select(d => d.Key.Metadata.Name);

            foreach (string line in PagesCollection.PageifyItems(loadedPlugSuccess, 100))
            {
                ModernConsole.WriteLine($"$g   {line}");
            }

            IEnumerable<string> loadedModSkipped = loadedMod
                .Where(d => d.Value.State == ExtensionResult.NotAllowed)
                .Select(d => d.Key.Metadata.Name);

            IEnumerable<string> loadedPlugSkipped = loadedPlug
                .Where(d => d.Value.State == ExtensionResult.NotAllowed)
                .Select(d => d.Key.Metadata.Name);

            if (loadedModSkipped.Any() || loadedPlugSkipped.Any())
            {
                ModernConsole.WriteLine($"\n⭕ $!gSkipped$!r extensions: ($bmodules$!r, $gplugins$!r)");

                foreach (string line in PagesCollection.PageifyItems(loadedModSkipped, 100))
                {
                    ModernConsole.WriteLine($"$!d$b   {line}");
                }

                foreach (string line in PagesCollection.PageifyItems(loadedPlugSkipped, 100))
                {
                    ModernConsole.WriteLine($"$!d$g   {line}");
                }
            }

            IEnumerable<KeyValuePair<IExtension, ExtensionHandleResult>> loadedModFailed = loadedMod
                .Where(d => d.Value.State == ExtensionResult.InternalError || d.Value.State == ExtensionResult.ExternalError);

            IEnumerable<KeyValuePair<IExtension, ExtensionHandleResult>> loadedPlugFailed = loadedPlug
                .Where(d => d.Value.State == ExtensionResult.InternalError || d.Value.State == ExtensionResult.ExternalError);

            if (loadedModFailed.Any() || loadedPlugFailed.Any())
            {
                Console.WriteLine('\n');

            }

            foreach (KeyValuePair<IExtension, ExtensionHandleResult> kvp in loadedModFailed)
            {
                ModernConsole.WriteLine($"Failed to load $bmodule$!r $!d$b'$!r$b{kvp.Key.Metadata.Name}$!d'$!r: $r{kvp.Value.ErrorMessage}");
            }

            foreach (KeyValuePair<IExtension, ExtensionHandleResult> kvp in loadedPlugFailed)
            {
                ModernConsole.WriteLine($"Failed to load $gplugin$!r $!d$g'$!r$g{kvp.Key.Metadata.Name}$!d'$!r: $r{kvp.Value.ErrorMessage}");
            }
        }

        Console.WriteLine('\n');
    }
}
