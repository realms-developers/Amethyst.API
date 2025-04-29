using Amethyst.Network;
using Amethyst.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;

namespace Amethyst.World;

public static class WorldUtils
{
    public static Tile[,] CopyTiles(int x, int y, int width, int height)
    {
        Tile[,] array = new Tile[width, height];

        for (int i = x; i < x + width; i++)
        for (int j = y; j < y + height; j++)
        {
            array[i, j] = new Tile();
            array[i, j].CopyFrom(Main.tile[i, j]);
        }

        return array;
    }

    public static void PasteTiles(int x, int y, Tile[,] array)
    {
        for (int i = x; i < x + array.GetLength(0); i++)
        for (int j = y; j < y + array.GetLength(1); j++)
        {
            Main.tile[i, j].CopyFrom(array[i - x, j - y]);
        }
    }

    public static void ClearDropped()
    {
        for (int i = 0; i < Main.item.Length; i++)
        {
            Main.item[i] = new();

            NetMessage.SendData(21, -1, -1, NetworkText.Empty, i);
            NetMessage.SendData(22, -1, -1, NetworkText.Empty, i, 255);
        }
    }

    public static void SendCombatText(float x, float y, string text, Color color, NetPlayer? target = null)
    {
        using PacketWriter writer = new();

        byte[] packetBytes = writer
            .SetType((short)PacketTypes.CreateCombatTextExtended)
            .PackSingle(x)
            .PackSingle(y - 32)
            .PackColor(color)
            .PackByte(0)
            .PackString(text)
            .BuildPacket();

        if (target != null)
        {
            target.Socket.SendPacket(packetBytes);
        }
        else
        {
            PlayerUtilities.BroadcastPacket(packetBytes);
        }
    }

    public static void SendCombatText(float x, float y, string text, NetColor color, NetPlayer? target = null) =>
        SendCombatText(x, y, text, color.ToXNA(), target);
}
