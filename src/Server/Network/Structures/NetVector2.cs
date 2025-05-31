namespace Amethyst.Server.Network.Structures;

#pragma warning disable CA1051
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

    public float X;
    public float Y;
}
