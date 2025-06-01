#pragma warning disable CA1051
// Terraria.BitsByte

namespace Amethyst.Server.Network.Structures;

public struct NetBitsByte
{
    public byte ByteValue;

    public bool this[int key]
    {
        get => (ByteValue & (1 << key)) != 0;
        set
        {
            if (value) ByteValue |= (byte)(1 << key);
            else ByteValue &= (byte)(~(1 << key));
        }
    }

    public static implicit operator byte(NetBitsByte bitsBytes)
        => bitsBytes.ByteValue;

    public static implicit operator NetBitsByte(byte byteValue)
        => new NetBitsByte() { ByteValue = byteValue };
}
