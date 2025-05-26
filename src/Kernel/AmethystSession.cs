using Amethyst.Extensions;
using Amethyst.Extensions.Base;
using Amethyst.Extensions.Base.Result;
using Amethyst.Infrastructure.CLI;
using Amethyst.Infrastructure.Kernel;
using Amethyst.Infrastructure.Profiles;
using Amethyst.Server.Base;
using Amethyst.Server.TerrariaAPI;
using Amethyst.Text;

namespace Amethyst.Kernel;

public static class AmethystSession
{
    static AmethystSession()
    {
        Profile = AmethystKernel.Profile!;
    }

    public static ServerProfile Profile { get; }

    public static IServerLauncher Launcher { get; set; } = new TAPILauncher();

    internal static void StartServer()
    {
        Launcher.Initialize();

        Localization.Load();

        ExtensionsOrganizer.LoadModules();
        ExtensionsOrganizer.LoadPlugins();

        PrintWelcome();

        Launcher.StartServer();
    }

    private static void PrintWelcome()
    {
        System.Console.Clear();

        ModernConsole.WriteLine(@"
$!b$m      __                   _   _               _
$!b$m     / /_ _ _ __ ___   ___| |_| |__  _   _ ___| |_
$!b$m    / / _` | '_ ` _ \ / _ \ __| '_ \| | | / __| __|
$!b$m _ / / (_| | | | | | |  __/ |_| | | | |_| \__ \ |_
$!b$m(_)_/ \__,_|_| |_| |_|\___|\__|_| |_|\__, |___/\__|
$!b$m                                     |___/         ");

        ModernConsole.WriteLine($"\n🛡️  $!b$mAmethyst v{typeof(AmethystSession).Assembly.GetName().Version} $!r$!bis distributed under the MIT License.");
        ModernConsole.WriteLine($"🛡️  You are free to use, modify and distribute the code, provided that the author is attributed.");

        ModernConsole.WriteLine($"\n💾 Server with profile $!d$m'$!r$m{Profile.Name}$!d'$!r runs in {(Profile.DebugMode ? "$!b$rDebug" : "$!b$!gDefault")} $!rmode.");

        if (Profile.DebugMode)
        {
            ModernConsole.WriteLine("❗ $rThis means that more data needed for development will be logged to the console.");
            ModernConsole.WriteLine("❗ $rDebug-mode also provides $!b/grantroot $!r$rcommand that gives all permissions to player.");
            ModernConsole.WriteLine("❗ $r$!bDo not use this mode on public servers!");
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
