#pragma warning disable CA1051

using Terraria;

namespace Amethyst.Network.Structures;

public struct NetVector2
{
    public NetVector2(float x, float y)
    {
        X = x;
        Y = y;
    }

    public static implicit operator NetVector2(Microsoft.Xna.Framework.Vector2 vector)
        => new NetVector2(vector.X, vector.Y);

    public static implicit operator Microsoft.Xna.Framework.Vector2(NetVector2 vector)
        => new Microsoft.Xna.Framework.Vector2(vector.X, vector.Y);

    public static implicit operator NetVector2(System.Numerics.Vector2 vector)
        => new NetVector2(vector.X, vector.Y);

    public static implicit operator System.Numerics.Vector2(NetVector2 vector)
        => new System.Numerics.Vector2(vector.X, vector.Y);

    public bool IsValid()
    {
        return InWorld() && !float.IsNaN(X) && !float.IsNaN(Y) && !float.IsInfinity(X) && !float.IsInfinity(Y);
    }
    public bool InWorld()
    {
        return X >= 0 && Y >= 0 && X <= Main.maxTilesX * 16 && Y <= Main.maxTilesY * 16;
    }

    public override readonly string ToString()
    {
        return $"({X}, {Y})";
    }

    public float X;
    public float Y;
}
