// Code is generated by the Amethyst.PacketGenerator (v1.0.5.0) tool.
// Do not edit this file manually.
#pragma warning disable CA1051

using Amethyst.Network.Engine.Packets;
using Amethyst.Network.Utilities;

namespace Amethyst.Network.Packets;

public sealed class WorldFrameSectionPacket : IPacket<WorldFrameSection>
{
    public int PacketID => 11;

    public static WorldFrameSection Deserialize(ReadOnlySpan<byte> data, int offset = 0)
    {
        FastPacketReader reader = new(data, offset);

        short StartX = reader.ReadInt16();
        short StartY = reader.ReadInt16();
        short EndX = reader.ReadInt16();
        short EndY = reader.ReadInt16();

        return new WorldFrameSection
        {
            StartX = StartX,
            StartY = StartY,
            EndX = EndX,
            EndY = EndY,
        };
    }

    public static byte[] Serialize(WorldFrameSection packet)
    {
        FastPacketWriter writer = new(11, 128);

        writer.WriteInt16(packet.StartX);
        writer.WriteInt16(packet.StartY);
        writer.WriteInt16(packet.EndX);
        writer.WriteInt16(packet.EndY);

        return writer.Build();
    }
}

public struct WorldFrameSection
{
    public short StartX;
    public short StartY;
    public short EndX;
    public short EndY;
}
