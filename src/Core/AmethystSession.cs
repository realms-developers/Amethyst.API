using Amethyst.Commands;
using Amethyst.Core.Profiles;
using Amethyst.Core.Server;
using Amethyst.Extensions.Modules;
using Amethyst.Extensions.Plugins;
using Amethyst.Logging;
using Amethyst.Permissions;
using Amethyst.Players;
using Amethyst.Players.Auth;
using Amethyst.Security;
using Amethyst.Text;

namespace Amethyst.Core;

public static class AmethystSession
{
    static AmethystSession()
    {
        // Session = this;

        Profile = AmethystKernel.Profile!;
        OfflinePermissions = new PermissionsNode<ICommandSender>();
        PlayerPermissions = new PermissionsNode<NetPlayer>();
    }

    public static ServerProfile Profile { get; }

    public static StorageConfiguration StorageConfiguration => Profile.Config.Get<StorageConfiguration>().Data;

    public static ExtensionsConfiguration ExtensionsConfiguration => Profile.Config.Get<ExtensionsConfiguration>().Data;

    public static PermissionsNode<ICommandSender> OfflinePermissions { get; }

    public static PermissionsNode<NetPlayer> PlayerPermissions { get; }

    internal static void StartServer()
    {
        InitializeConfig();

        ServerLauncher.Initialize();

        AmethystHooks.Initialize();

        Localization.Load();

        AuthManager.Initialize();

        CommandsManager.Initialize();
        PlayerManager.Initialize();

        SecurityManager.Initialize();

        ModuleLoader.LoadModules();
        PluginLoader.LoadPlugins();

        PrintWelcome();

        ServerLauncher.Start();
    }

    private static void InitializeConfig()
    {
        Profile.Config.Get<StorageConfiguration>().Load();
        Profile.Config.Get<StorageConfiguration>().Modify(SetupStorageConfiguration, true);

        Profile.Config.Get<ExtensionsConfiguration>().Load();
        Profile.Config.Get<ExtensionsConfiguration>().Modify(SetupExtensionsConfiguration, true);
    }

    private static void SetupStorageConfiguration(ref StorageConfiguration configuration)
    {
        // MongoDB
        configuration.MongoConnection ??= "mongodb://localhost:27017/";

        configuration.MongoDatabaseName ??= Profile.Name;

        // MySQL
        configuration.MySQLServer ??= "localhost";
        configuration.MySQLDatabase ??= Profile.Name;
        configuration.MySQLUid ??= "root";
        configuration.MySQLPwd ??= string.Empty;
    }

    private static void SetupExtensionsConfiguration(ref ExtensionsConfiguration configuration)
    {
        configuration.AllowedModules ??= ["Amethyst.Environment.dll"];

        configuration.AllowedPlugins ??= [string.Empty];
    }

    private static void PrintWelcome()
    {
        Console.Clear();

        ModernConsole.WriteLine("$!b$m      __                   _   _               _   ");
        ModernConsole.WriteLine("$!b$m     / /_ _ _ __ ___   ___| |_| |__  _   _ ___| |_ ");
        ModernConsole.WriteLine("$!b$m    / / _` | '_ ` _ \\ / _ \\ __| '_ \\| | | / __| __|");
        ModernConsole.WriteLine("$!b$m _ / / (_| | | | | | |  __/ |_| | | | |_| \\__ \\ |_ ");
        ModernConsole.WriteLine("$!b$m(_)_/ \\__,_|_| |_| |_|\\___|\\__|_| |_|\\__, |___/\\__|");
        ModernConsole.WriteLine("$!b$m                                     |___/         ");

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

        if (ModuleLoader.LogLoaded.Count > 1 || PluginLoader.LogLoaded.Count > 1)
        {
            ModernConsole.WriteLine($"\n✔️  $gLoaded$!r extensions: ($bmodules$!r, $gplugins)");
            foreach (string line in PagesCollection.PageifyItems(ModuleLoader.LogLoaded, 100))
            {
                ModernConsole.WriteLine($"$b   {line}");
            }
            foreach (string line in PagesCollection.PageifyItems(PluginLoader.LogLoaded, 100))
            {
                ModernConsole.WriteLine($"$g   {line}");
            }
        }

        if (ModuleLoader.LogSkipped.Count > 1 || PluginLoader.LogSkipped.Count > 1)
        {
            ModernConsole.WriteLine($"\n⭕ $!gSkipped$!r extensions: ($bmodules$!r, $gplugins)");
            foreach (string line in PagesCollection.PageifyItems(ModuleLoader.LogSkipped, 100))
            {
                ModernConsole.WriteLine($"$!d$b   {line}");
            }
            foreach (string line in PagesCollection.PageifyItems(PluginLoader.LogSkipped, 100))
            {
                ModernConsole.WriteLine($"$!d$g   {line}");
            }
        }

        if (ModuleLoader.LogFailed.Count > 0 || PluginLoader.LogFailed.Count > 0)
        {
            Console.WriteLine("\n");
        }

        foreach (KeyValuePair<string, Exception> kvp in ModuleLoader.LogFailed)
        {
            ModernConsole.WriteLine($"Failed to load $bmodule$!r $!d$b'$!r$b{kvp.Key}$!d'$!r: $r{kvp.Value}");
        }
        foreach (KeyValuePair<string, Exception> kvp in PluginLoader.LogFailed)
        {
            ModernConsole.WriteLine($"Failed to load $gplugin$!r $!d$g'$!r$g{kvp.Key}$!d'$!r: $r{kvp.Value}");
        }

        Console.WriteLine("\n");
    }
}
