// Code is generated by the Amethyst.PacketGenerator (v1.0.5.0) tool.
// Do not edit this file manually.
#pragma warning disable CA1051

using Amethyst.Network.Engine.Packets;
using Amethyst.Network.Utilities;

namespace Amethyst.Network.Packets;

public sealed class NPCSyncBuffsPacket : IPacket<NPCSyncBuffs>
{
    public int PacketID => 54;

    public static NPCSyncBuffs Deserialize(ReadOnlySpan<byte> data, int offset = 0)
    {
        FastPacketReader reader = new(data, offset);

        short NPCIndex = reader.ReadInt16();
        ushort[] BuffTypes = reader.ReadUInt16Array(20);
        short[] BuffTime = reader.ReadInt16Array(20);

        return new NPCSyncBuffs
        {
            NPCIndex = NPCIndex,
            BuffTypes = BuffTypes,
            BuffTime = BuffTime,
        };
    }

    public static byte[] Serialize(NPCSyncBuffs packet)
    {
        FastPacketWriter writer = new(54, 128);

        writer.WriteInt16(packet.NPCIndex);
        writer.WriteUInt16Array(packet.BuffTypes);
        writer.WriteInt16Array(packet.BuffTime);

        return writer.Build();
    }
}

public struct NPCSyncBuffs
{
    public short NPCIndex;
    public ushort[] BuffTypes;
    public short[] BuffTime;
}
