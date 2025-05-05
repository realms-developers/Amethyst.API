using Microsoft.Xna.Framework;
using Terraria;

namespace Amethyst.Security;

public static class ValidationExtensions
{
    public static bool IsBadVector2(this Vector2 vector2)
    {
        return  float.IsNaN(vector2.X) || float.IsInfinity(vector2.X) ||
                float.IsNaN(vector2.Y) || float.IsInfinity(vector2.Y);
    }
    public static bool IsInTerrariaWorld(this Vector2 vector2, int offset = 8)
    {
        float min = offset * 16;

        float maxX = (Main.maxTilesX - offset) * 16;
        float maxY = (Main.maxTilesY - offset) * 16;

        return  vector2.X >= min || vector2.X <= maxX ||
                vector2.Y >= min || vector2.Y <= maxY;
    }
}
