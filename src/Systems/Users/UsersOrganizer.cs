using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Players;

namespace Amethyst.Systems.Users;

public static class UsersOrganizer
{
    public static PlayersUsersService PlayerUsers { get; } = new PlayersUsersService();
}
