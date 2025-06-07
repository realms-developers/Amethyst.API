using Amethyst.Hooks.Autoloading;
using Amethyst.Server.Entities.Players;

namespace Amethyst.Hooks.Args.Players;

[AutoloadHook]
public sealed class PlayerFullyJoinedArgs
{
    public PlayerFullyJoinedArgs(PlayerEntity player)
    {
        Player = player;
    }

    public PlayerEntity Player { get; }
}
