using Amethyst.Network;
using Amethyst.Players;
using Microsoft.Xna.Framework;

namespace Amethyst.World;

public static class WorldUtils
{
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
