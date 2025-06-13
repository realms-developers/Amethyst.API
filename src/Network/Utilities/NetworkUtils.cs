using Terraria;

namespace Amethyst.Network.Utilities;

public static class NetworkUtils
{
    public static int WorldWidth => Main.maxTilesX;
    public static int WorldHeight => Main.maxTilesY;

    public static bool IsInWorldX(this int x)
    {
        return x >= 0 && x < WorldWidth;
    }

    public static bool IsInWorldX(this short x)
    {
        return x >= 0 && x < WorldWidth;
    }

    public static bool IsInWorldY(this int y)
    {
        return y >= 0 && y < WorldHeight;
    }

    public static bool IsInWorldY(this short y)
    {
        return y >= 0 && y < WorldHeight;
    }

    public static bool IsInWorld(this (int, int) point)
    {
        return point.Item1 >= 0 && point.Item1 < WorldWidth && point.Item2 >= 0 && point.Item2 < WorldHeight;
    }

    public static bool IsInWorld(this (short, short) point)
    {
        return point.Item1 >= 0 && point.Item1 < WorldWidth && point.Item2 >= 0 && point.Item2 < WorldHeight;
    }
}
