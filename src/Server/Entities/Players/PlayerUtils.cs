using Microsoft.Xna.Framework;

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
}
