using Amethyst.Hooks.Autoloading;
using Amethyst.Server.Entities.Players;

namespace Amethyst.Hooks.Args.Players;

[AutoloadHook]
public sealed class PlayerTrackerRemoveArgs(PlayerEntity player)
{
    public PlayerEntity Player { get; } = player;
}
