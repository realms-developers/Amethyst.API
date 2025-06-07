using Amethyst.Hooks.Autoloading;
using Amethyst.Server.Entities.Players;

namespace Amethyst.Hooks.Args.Players;

[AutoloadHook]
public sealed class PlayerTrackerRemoveArgs
{
    public PlayerTrackerRemoveArgs(PlayerEntity player)
    {
        Player = player;
    }

    public PlayerEntity Player { get; }
}
