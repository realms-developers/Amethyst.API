using Amethyst.Hooks.Autoloading;
using Amethyst.Systems.Users.Players;

namespace Amethyst.Server.Entities.Players.Hooks;

[AutoloadHook]
public sealed class PlayerTrackerRemoveArgs
{
    public PlayerTrackerRemoveArgs(PlayerEntity player)
    {
        Player = player;
    }

    public PlayerEntity Player { get; }
}
