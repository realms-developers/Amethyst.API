using System.Globalization;
using Microsoft.Xna.Framework;

namespace Amethyst.Server.Network.Structures;

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

    public readonly Color ToXNA() => new(R, G, B);

    public readonly string ToHex()
        => R.ToString("X2", CultureInfo.InvariantCulture) + G.ToString("X2", CultureInfo.InvariantCulture) + B.ToString("X2", CultureInfo.InvariantCulture);

    public readonly int ToPackedValue()
        => (R << 16) | (G << 8) | B;

    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
}
