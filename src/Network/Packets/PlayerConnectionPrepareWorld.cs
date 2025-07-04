// Code is generated by the Amethyst.PacketGenerator (v1.0.5.0) tool.
// Do not edit this file manually.
#pragma warning disable CA1051

using Amethyst.Network.Engine.Packets;
using Amethyst.Network.Utilities;

namespace Amethyst.Network.Packets;

public sealed class PlayerConnectionPrepareWorldPacket : IPacket<PlayerConnectionPrepareWorld>
{
    public int PacketID => 129;

    public static PlayerConnectionPrepareWorld Deserialize(ReadOnlySpan<byte> data, int offset = 0)
    {
        return new PlayerConnectionPrepareWorld
        {
        };
    }

    public static byte[] Serialize(PlayerConnectionPrepareWorld packet)
    {
        FastPacketWriter writer = new(129, 4);
        return writer.Build();
    }
}

public struct PlayerConnectionPrepareWorld
{
}
