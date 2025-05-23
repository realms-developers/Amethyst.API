using Amethyst.Hooks.Autoloading;
using Amethyst.Systems.Users.Players;

namespace Amethyst.Server.Entities.Players.Hooks;

[AutoloadHook]
public sealed class PlayerTrackerInsertArgs
{
    public PlayerTrackerInsertArgs(PlayerEntity player)
    {
        Player = player;
    }

    public PlayerEntity Player { get; }
}
