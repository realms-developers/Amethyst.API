using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Players;

namespace Amethyst.Systems.Users;

public static class UsersOrganizer
{
    public static IUsersService<PlayerUser, PlayerUserMetadata> PlayerUsers { get; } = new PlayersUsersService();
}
