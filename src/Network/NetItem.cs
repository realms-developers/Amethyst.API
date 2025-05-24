namespace Amethyst.Network;

public struct NetItem(int id, short stack, byte prefix)
{
    public int ID { get; set; } = id;
    public short Stack { get; set; } = stack;
    public byte Prefix { get; set; } = prefix;

    public static bool operator ==(NetItem left, NetItem right)
    {
        return left.ID == right.ID && left.Stack == right.Stack && left.Prefix == right.Prefix;
    }

    public static bool operator !=(NetItem left, NetItem right) => !(left == right);

    public override readonly bool Equals(object? obj) => base.Equals(obj);

    public override readonly int GetHashCode() => base.GetHashCode();
}
