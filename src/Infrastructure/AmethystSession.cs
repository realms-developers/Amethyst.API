using Amethyst.Commands;
using Amethyst.Extensions;
using Amethyst.Infrastructure.Core.Profiles;
using Amethyst.Infrastructure.Kernel;
using Amethyst.Infrastructure.Server;
using Amethyst.Permissions;
using Amethyst.Players;
using Amethyst.Players.Auth;
using Amethyst.Security;

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

        ExtensionsOrganizer.Initialize();

        ExtensionsOrganizer.LoadModules();
        ExtensionsOrganizer.LoadPlugins();

        ServerLauncher.Start();
    }

}
