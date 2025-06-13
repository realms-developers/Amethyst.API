using Amethyst.Hooks.Autoloading;
using Amethyst.Server.Entities.Players;
using Amethyst.Systems.Users.Players;

namespace Amethyst.Hooks.Args.Players;

[AutoloadHook]
public sealed class PlayerSetUserArgs(PlayerEntity player, PlayerUser? old, PlayerUser? newUser)
{
    public PlayerEntity Player { get; } = player;

    public PlayerUser? Old { get; } = old;
    public PlayerUser? New { get; set; } = newUser;
}
