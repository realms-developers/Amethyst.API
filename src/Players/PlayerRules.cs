namespace Amethyst.Players;

public sealed class PlayerRules
{
    internal PlayerRules(NetPlayer plr) => Player = plr;

    public NetPlayer Player { get; }

    public bool CanInteractItem { get; }
}
