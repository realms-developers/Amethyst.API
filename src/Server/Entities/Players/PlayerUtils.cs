using Amethyst.Kernel;
using Amethyst.Network.Structures;

namespace Amethyst.Server.Entities.Players;

public static class PlayerUtils
{
    public static void BroadcastText(string text, byte r, byte g, byte b, params object?[] args)
    {
        foreach (PlayerEntity plr in EntityTrackers.Players)
        {
            if (plr.Active)
            {
                string msg = string.Format(null,
                    Localization.Get(text, plr.User?.Messages.Language ?? AmethystSession.Profile.DefaultLanguage), args);

                plr.SendText(msg, r, g, b);
            }
        }
    }

    public static void BroadcastText(string text, NetColor color, params object?[] args) => BroadcastText(text, color.R, color.G, color.B, args);

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

    public static void BroadcastPacketBytes(byte[] packetBytes, int offset, int length, int ignore = -1)
    {
        foreach (PlayerEntity plr in EntityTrackers.Players)
        {
            if (plr.Active && plr.Index != ignore)
            {
                plr.SendPacketBytes(packetBytes, offset, length);
            }
        }
    }
}
