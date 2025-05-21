using Amethyst.Network;
using Terraria;

namespace Amethyst.Systems.Commands.Arguments;

public sealed class ItemReference
{
    internal ItemReference(int type)
    {
        Index = type;

        _netItem = new NetItem(type, 1, 0);

        TItem = new Item();
        TItem.SetDefaults(type);
        TItem.stack = 1;
    }

    private NetItem _netItem;

    public int Index { get; }

    public NetItem NetItem => _netItem;

    public Item TItem { get; }
}
