using Amethyst.Hooks.Autoloading;

namespace Amethyst.Hooks.Args.Players;

[AutoloadHook]
public sealed class PlayerSocketDisconnectArgs
{
    public PlayerSocketDisconnectArgs(int index)
    {
        Index = index;
    }

    public int Index { get; }
}
