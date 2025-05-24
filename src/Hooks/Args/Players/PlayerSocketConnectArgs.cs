using Amethyst.Hooks.Autoloading;

namespace Amethyst.Hooks.Args.Players;

[AutoloadHook]
public sealed class PlayerSocketConnectArgs
{
    public PlayerSocketConnectArgs(int index)
    {
        Index = index;
    }

    public int Index { get; }
}
