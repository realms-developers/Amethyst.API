namespace Amethyst.Server.Entities.Players;

public static class PlayerUtils
{
    public static void BroadcastText(string text, byte r, byte g, byte b)
    {
        foreach (var plr in EntityTrackers.Players)
        {
            if (plr.Active)
            {
                plr.SendText(text, r, g, b);
            }
        }
    }

    public static void BroadcastPacketBytes(byte[] packetBytes, int ignore = -1)
    {
        foreach (var plr in EntityTrackers.Players)
        {
            if (plr.Active && plr.Index != ignore)
            {
                plr.SendPacketBytes(packetBytes);
            }
        }
    }
}
