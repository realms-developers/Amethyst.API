// Code is generated by the Amethyst.PacketGenerator (v1.0.5.0) tool.
// Do not edit this file manually.
#pragma warning disable CA1051

using Amethyst.Network.Engine.Packets;
using Amethyst.Network.Utilities;

namespace Amethyst.Network.Packets;

public sealed class NPCDryadStardewAnimationPacket : IPacket<NPCDryadStardewAnimation>
{
    public int PacketID => 144;

    public static NPCDryadStardewAnimation Deserialize(ReadOnlySpan<byte> data, int offset = 0)
    {
        return new();
    }

    public static byte[] Serialize(NPCDryadStardewAnimation packet)
    {
        FastPacketWriter writer = new(144, 128);
        return writer.Build();
    }
}

public struct NPCDryadStardewAnimation
{
}
