using Amethyst.Extensions;
using Amethyst.Extensions.Base;
using Amethyst.Extensions.Base.Result;
using Amethyst.Hooks;
using Amethyst.Hooks.Args.Utility;
using Amethyst.Hooks.Base;
using Amethyst.Kernel.Console;
using Amethyst.Kernel.Profiles;
using Amethyst.Network;
using Amethyst.Server;
using Amethyst.Systems.Characters;
using Amethyst.Systems.Commands.Dynamic.Utilities;
using Amethyst.Text;

namespace Amethyst.Kernel;

public static class AmethystSession
{
    static AmethystSession() => Profile = AmethystKernel.Profile!;

    public static ServerProfile Profile { get; }

    public static IServerLauncher Launcher { get; set; } = new TAPILauncher();

    internal static void StartServer()
    {
        Launcher.Initialize();

        NetworkManager.Initialize();

        ExtensionsOrganizer.LoadModules();
        ExtensionsOrganizer.LoadPlugins();

        PrintWelcome();

        Localization.Load();

        ImportUtility.ImportFrom(typeof(AmethystSession).Assembly, ImportUtility.CoreIdentifier);

        CharactersSaver.Setup(TimeSpan.FromMinutes(1));

        _secondTickTimer = new Timer(OnSecondTick, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

        Launcher.StartServer();
    }

    private static Timer? _secondTickTimer;
    private static readonly AmethystHook<SecondTickArgs> _secondTickHook = HookRegistry.GetHook<SecondTickArgs>();
    private static void OnSecondTick(object? state)
    {
        _secondTickHook?.Invoke(new SecondTickArgs());
    }

    private static void PrintWelcome()
    {
        if (!Profile.DebugMode)
        {
            System.Console.Clear();

            ModernConsole.WriteLine(@"
$!b$m      __                   _   _               _
$!b$m     / /_ _ _ __ ___   ___| |_| |__  _   _ ___| |_
$!b$m    / / _` | '_ ` _ \ / _ \ __| '_ \| | | / __| __|
$!b$m _ / / (_| | | | | | |  __/ |_| | | | |_| \__ \ |_
$!b$m(_)_/ \__,_|_| |_| |_|\___|\__|_| |_|\__, |___/\__|
$!b$m                                     |___/         ");

            ModernConsole.WriteLine($"\n🛡️  $!b$mAmethyst v{typeof(AmethystSession).Assembly.GetName().Version} $!bis distributed under the MIT License.");
            ModernConsole.WriteLine($"🛡️  You are free to use, modify and distribute the code, provided that the author is attributed.");

            ModernConsole.WriteLine($"\n🛡️  Repository: $!b$mhttps://github.com/realms-developers/Amethyst.API");
        }
        else
        {
            ModernConsole.WriteLine($"\n🛡️  $!b$mAmethyst v{typeof(AmethystSession).Assembly.GetName().Version}");
            ModernConsole.WriteLine($"❗ $rDebug-mode provides $!b/groot $!r$rcommand that gives all permissions to player.");
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

            foreach (string line in PagesCollection.ToList(loadedModSuccess, 100))
            {
                ModernConsole.WriteLine($"$b   {line}");
            }

            IEnumerable<string> loadedPlugSuccess = loadedPlug
                .Where(d => d.Value.State == ExtensionResult.SuccessOperation)
                .Select(d => d.Key.Metadata.Name);

            foreach (string line in PagesCollection.ToList(loadedPlugSuccess, 100))
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

                foreach (string line in PagesCollection.ToList(loadedModSkipped, 100))
                {
                    ModernConsole.WriteLine($"$!d$b   {line}");
                }

                foreach (string line in PagesCollection.ToList(loadedPlugSkipped, 100))
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
                ModernConsole.WriteLine("\n");

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

        ModernConsole.WriteLine("\n");
    }
}
