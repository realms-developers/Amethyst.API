using Amethyst.Network;
using Amethyst.Players;
using Terraria;

namespace Amethyst.Commands.Arguments;

public sealed class ItemReference
{
    internal ItemReference(int type)
    {
        Index = type;

        _netItem = new NetItem(type, 1, 0);

        _tItem = new Item();
        _tItem.SetDefaults(type);
        _tItem.stack = 1;
    }

    private NetItem _netItem;
    private Item _tItem;

    public int Index { get; }

    public NetItem NetItem => _netItem;
    public Item TItem => _tItem;
}