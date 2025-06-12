using Amethyst.Hooks.Autoloading;
using Amethyst.Server.Entities.Players;
using Amethyst.Systems.Users.Players;

namespace Amethyst.Hooks.Args.Players;

[AutoloadHook]
public sealed class PlayerPostSetUserArgs(PlayerEntity player, PlayerUser? newUser)
{
    public PlayerEntity Player { get; } = player;
    public PlayerUser? User { get; set; } = newUser;
}
