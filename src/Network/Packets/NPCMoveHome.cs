// Code is generated by the Amethyst.PacketGenerator (v1.0.5.0) tool.
// Do not edit this file manually.
#pragma warning disable CA1051

using Amethyst.Network.Engine.Packets;
using Amethyst.Network.Utilities;

namespace Amethyst.Network.Packets;

public sealed class NPCMoveHomePacket : IPacket<NPCMoveHome>
{
    public int PacketID => 60;

    public static NPCMoveHome Deserialize(ReadOnlySpan<byte> data, int offset = 0)
    {
        FastPacketReader reader = new(data, offset);

        short NPCIndex = reader.ReadInt16();
        short TileX = reader.ReadInt16();
        short TileY = reader.ReadInt16();
        byte Action = reader.ReadByte();

        return new NPCMoveHome
        {
            NPCIndex = NPCIndex,
            TileX = TileX,
            TileY = TileY,
            Action = Action,
        };
    }

    public static byte[] Serialize(NPCMoveHome packet)
    {
        FastPacketWriter writer = new(60, 128);

        writer.WriteInt16(packet.NPCIndex);
        writer.WriteInt16(packet.TileX);
        writer.WriteInt16(packet.TileY);
        writer.WriteByte(packet.Action);

        return writer.Build();
    }
}

public struct NPCMoveHome
{
    public short NPCIndex;
    public short TileX;
    public short TileY;
    public byte Action;
}
