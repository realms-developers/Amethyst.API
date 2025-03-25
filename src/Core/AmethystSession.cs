using Amethyst.Commands;
using Amethyst.Core.Profiles;
using Amethyst.Core.Server;
using Amethyst.Extensions.Modules;
using Amethyst.Extensions.Plugins;
using Amethyst.Permissions;
using Amethyst.Localization;
using Amethyst.Players;

namespace Amethyst.Core;

public static class AmethystSession
{
    // private static AmethystSession? _session = null;

    // // dsaf dont touch this 
    // // this was initialized in AmethystKernel
    // public static AmethystSession Session
    // {
    //     get => _session ?? throw new InvalidOperationException("Cannot get access to a uninitialized AmethystSession.");
    //     private set => _session = value;
    // }

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

        LocalizationManager.Initialize();
        CommandsManager.Initialize();
        PlayerManager.Initialize();

        ModuleLoader.LoadModules();
        PluginLoader.LoadPlugins();

        ServerLauncher.Start();
    }

    private static void InitializeConfig()
    {
        Profile.Config.Get<StorageConfiguration>().Load();
        Profile.Config.Get<StorageConfiguration>().Modify(SetupStorageConfiguration, true);

        Profile.Config.Get<ExtensionsConfiguration>().Load();
        Profile.Config.Get<ExtensionsConfiguration>().Modify(SetupExtensionsConfiguration, true);
    }

    private static StorageConfiguration SetupStorageConfiguration(StorageConfiguration configuration)
    {
        // MongoDB
        configuration.MongoConnection ??= "mongodb://localhost:27017/";

        configuration.MongoDatabaseName ??= Profile.Name;

        // MySQL
        configuration.MySQLServer ??= "localhost";
        configuration.MySQLDatabase ??= Profile.Name;
        configuration.MySQLUid ??= "root";
        configuration.MySQLPwd ??= string.Empty;

        return configuration;
    }

    private static ExtensionsConfiguration SetupExtensionsConfiguration(ExtensionsConfiguration configuration)
    {
        configuration.AllowedModules ??= new () { "Amethyst.Environment.dll" };

        configuration.AllowedPlugins ??= new () { string.Empty };

        return configuration;
    }
}
