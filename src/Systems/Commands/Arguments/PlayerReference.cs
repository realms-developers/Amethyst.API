using Amethyst.Gameplay.Players;
using Terraria;

namespace Amethyst.Systems.Commands.Arguments;

public sealed class PlayerReference
{
    internal PlayerReference(int index)
    {
        Index = index;
    }

    public int Index { get; }

    public NetPlayer Player => PlayerManager.Tracker[Index];
    public Player TPlayer => Main.player[Index];
}
