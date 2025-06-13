using System.Drawing;

namespace Amethyst.Server.Entities.Players;

public static class PlayerUtils
{
    public static void BroadcastText(string text, byte r, byte g, byte b)
    {
        foreach (PlayerEntity plr in EntityTrackers.Players)
        {
            if (plr.Active)
            {
                plr.SendText(text, r, g, b);
            }
        }
    }

    public static void BroadcastText(string text, Color color) => BroadcastText(text, color.R, color.G, color.B);

    public static void BroadcastPacketBytes(byte[] packetBytes, int ignore = -1)
    {
        foreach (PlayerEntity plr in EntityTrackers.Players)
        {
            if (plr.Active && plr.Index != ignore)
            {
                plr.SendPacketBytes(packetBytes);
            }
        }
    }
}
