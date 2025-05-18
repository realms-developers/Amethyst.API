using System.Globalization;
using Amethyst.Network;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Amethyst.Gameplay.Players;

public static class PlayerUtilities
{
    public static void BroadcastPacket(byte[] data)
    {
        foreach (NetPlayer plr in PlayerManager.Tracker)
        {
            plr.Socket.SendPacket(data);
        }
    }

    public static void BroadcastPacket(byte[] data, Predicate<NetPlayer> predicate)
    {
        foreach (NetPlayer plr in PlayerManager.Tracker)
        {
            if (predicate(plr))
            {
                plr.Socket.SendPacket(data);
            }
        }
    }

    public static void BroadcastText(string text, Color color, Predicate<NetPlayer>? predicate = null)
    {
        foreach (NetPlayer plr in PlayerManager.Tracker)
        {
            if (predicate == null || predicate(plr))
            {
                plr.SendMessage(text, color);
            }
        }
    }

    public static void BroadcastLocalizedText(string text, object[] args, Color color, Predicate<NetPlayer>? predicate = null)
    {
        foreach (NetPlayer plr in PlayerManager.Tracker)
        {
            if (predicate == null || predicate(plr))
            {
                plr.SendMessage(string.Format(CultureInfo.InvariantCulture, Localization.Get(text, plr.Language), args), color);
            }
        }
    }
