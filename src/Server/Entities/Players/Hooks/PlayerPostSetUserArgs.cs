using Amethyst.Hooks.Autoloading;
using Amethyst.Systems.Users.Players;

namespace Amethyst.Server.Entities.Players.Hooks;

[AutoloadHook]
public sealed class PlayerPostSetUserArgs
{
    public PlayerPostSetUserArgs(PlayerEntity player, PlayerUser? newUser)
    {
        Player = player;
        User = newUser;
    }

    public PlayerEntity Player { get; }
    public PlayerUser? User { get; set; }
}
