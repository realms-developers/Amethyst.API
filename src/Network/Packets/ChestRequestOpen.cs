// Code is generated by the Amethyst.PacketGenerator (v1.0.5.0) tool.
// Do not edit this file manually.
#pragma warning disable CA1051

using Amethyst.Network.Engine.Packets;
using Amethyst.Network.Utilities;

namespace Amethyst.Network.Packets;

public sealed class ChestRequestOpenPacket : IPacket<ChestRequestOpen>
{
    public int PacketID => 31;

    public static ChestRequestOpen Deserialize(ReadOnlySpan<byte> data, int offset = 0)
    {
        FastPacketReader reader = new(data, offset);

        short TileX = reader.ReadInt16();
        short TileY = reader.ReadInt16();

        return new ChestRequestOpen
        {
            TileX = TileX,
            TileY = TileY,
        };
    }

    public static byte[] Serialize(ChestRequestOpen packet)
    {
        FastPacketWriter writer = new(31, 128);

        writer.WriteInt16(packet.TileX);
        writer.WriteInt16(packet.TileY);

        return writer.Build();
    }
}

public struct ChestRequestOpen
{
    public short TileX;
    public short TileY;
}
