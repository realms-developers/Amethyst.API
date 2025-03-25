namespace Amethyst.Network;

public struct NetItem
{
    public NetItem(int id, short stack, byte prefix)
    {
        ID = id;
        Stack = stack;
        Prefix = prefix;
    }

    public int ID;
    public short Stack;
    public byte Prefix;

    public static bool operator ==(NetItem left, NetItem right)
    {
        if (left.ID == right.ID && left.Stack == right.Stack && left.Prefix == right.Prefix)
        {
            return true;
        }

        return false;
    }

    public static bool operator !=(NetItem left, NetItem right)
    {
        return !(left == right);
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}