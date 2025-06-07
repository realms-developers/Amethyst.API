#pragma warning disable CA1051

using Terraria;

namespace Amethyst.Network.Structures;

public struct NetItem(int id, short stack, byte prefix)
{
    public int ID = id;
    public short Stack = stack;
    public byte Prefix = prefix;

    public static implicit operator Item(NetItem netItem)
    {
        var item = new Item();
        item.SetDefaults(netItem.ID);
        item.stack = netItem.Stack;
        item.Prefix(netItem.Prefix);
        return item;
    }

    public static implicit operator NetItem(Item item)
    {
        return new NetItem(item.type, (short)item.stack, item.prefix);
    }

    public static bool operator ==(NetItem left, NetItem right)
    {
        return left.ID == right.ID && left.Stack == right.Stack && left.Prefix == right.Prefix;
    }

    public static bool operator !=(NetItem left, NetItem right) => !(left == right);

    public override readonly bool Equals(object? obj) => base.Equals(obj);

    public override readonly int GetHashCode() => base.GetHashCode();
}
