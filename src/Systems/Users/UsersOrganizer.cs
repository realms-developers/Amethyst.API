using Amethyst.Systems.Users.Artificial;
using Amethyst.Systems.Users.Artificial.Extensions;
using Amethyst.Systems.Users.Artificial.Messages;
using Amethyst.Systems.Users.Artificial.Permissions;
using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Permissions;
using Amethyst.Systems.Users.Players;
using Amethyst.Systems.Users.Players.Extensions;
using Amethyst.Systems.Users.Players.Messages;
using Amethyst.Systems.Users.Players.Permissions;
using Amethyst.Systems.Users.Players.Suspension;
using Amethyst.Systems.Users.Common.Permissions;
using Amethyst.Systems.Users.Players.Commands;
using Amethyst.Systems.Users.Artificial.Commands;

namespace Amethyst.Systems.Users;

public static class UsersOrganizer
{
    static UsersOrganizer()
    {
        ConsoleUser = ArtificialUsers.CreateUser(new ArtificialUserMetadata("root"));

        IPermissionProvider rootProvider = new RootPermissionProvider(ConsoleUser);
        ConsoleUser.Permissions.AddChild(rootProvider);

        ConsoleUser.Commands.Repositories.Clear();
        ConsoleUser.Commands.Repositories.AddRange(["shared", "root", "debug"]);
    }

    public static PlayersUsersService PlayerUsers { get; } = new PlayersUsersService(
        new PlayerMessageBuilder(),
        new PlayerPermissionBuilder(),
        new PlayerExtensionBuilder(),
        new PlayerSuspensionBuilder(),
        new PlayerCommandBuilder());

    public static ArtificialUsersService ArtificialUsers { get; } = new ArtificialUsersService(
        new ArtificialMessageBuilder(),
        new ArtificialPermissionBuilder(),
        new ArtificialExtensionBuilder(),
        new ArtificialCommandBuilder());

    public static IAmethystUser ConsoleUser { get; }
}
