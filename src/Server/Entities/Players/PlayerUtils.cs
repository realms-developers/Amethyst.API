using Amethyst.Network.Structures;

namespace Amethyst.Server.Entities.Players;

public static class PlayerUtils
{
    public static void BroadcastText(string text, byte r, byte g, byte b, bool localized = false)
    {
        foreach (PlayerEntity plr in EntityTrackers.Players)
        {
            if (plr.Active)
            {
                string msg = localized && plr.User != null ? Localization.Get(text, plr.User.Messages.Language) : text;

                plr.SendText(msg, r, g, b);
            }
        }
    }

    public static void BroadcastText(string text, NetColor color, bool localized = false) => BroadcastText(text, color.R, color.G, color.B, localized);

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
