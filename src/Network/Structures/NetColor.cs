#pragma warning disable CA1051

using System.Globalization;
using Microsoft.Xna.Framework;

namespace Amethyst.Network.Structures;

public struct NetColor
{
    public NetColor(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
    }

    public NetColor(string hex)
    {
        R = Convert.ToByte(hex.Substring(0, 2), 16);
        G = Convert.ToByte(hex.Substring(2, 2), 16);
        B = Convert.ToByte(hex.Substring(4, 2), 16);
    }

    public NetColor(int packedValue)
    {
        R = (byte)((packedValue >> 16) & 0xFF);
        G = (byte)((packedValue >> 8) & 0xFF);
        B = (byte)(packedValue & 0xFF);
    }

    public static implicit operator NetColor(Color color)
        => new(color.R, color.G, color.B);

    public static implicit operator Color(NetColor color)
        => new(color.R, color.G, color.B);

    public static implicit operator NetColor(string hex)
        => new(hex);

    public static implicit operator string(NetColor color)
        => color.ToHex();

    public static implicit operator NetColor(int packedValue)
        => new(packedValue);

    public static implicit operator int(NetColor color)
        => color.ToPackedValue();

    public readonly string ToHex()
        => R.ToString("X2", CultureInfo.InvariantCulture) + G.ToString("X2", CultureInfo.InvariantCulture) + B.ToString("X2", CultureInfo.InvariantCulture);

    public readonly int ToPackedValue()
        => (R << 16) | (G << 8) | B;

    public byte R;
    public byte G;
    public byte B;
}
