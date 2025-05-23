using Amethyst.Hooks.Autoloading;
using Amethyst.Systems.Users.Players;

namespace Amethyst.Server.Entities.Players.Hooks;

[AutoloadHook]
public sealed class PlayerSetUserArgs
{
    public PlayerSetUserArgs(PlayerEntity player, PlayerUser? old, PlayerUser? newUser)
    {
        Player = player;
        Old = old;
        New = newUser;
    }

    public PlayerEntity Player { get; }

    public PlayerUser? Old { get; }
    public PlayerUser? New { get; set; }
}
