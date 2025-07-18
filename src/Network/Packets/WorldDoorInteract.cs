// Code is generated by the Amethyst.PacketGenerator (v1.0.5.0) tool.
// Do not edit this file manually.
#pragma warning disable CA1051

using Amethyst.Network.Engine.Packets;
using Amethyst.Network.Utilities;

namespace Amethyst.Network.Packets;

public sealed class WorldDoorInteractPacket : IPacket<WorldDoorInteract>
{
    public int PacketID => 19;

    public static WorldDoorInteract Deserialize(ReadOnlySpan<byte> data, int offset = 0)
    {
        FastPacketReader reader = new(data, offset);

        byte Action = reader.ReadByte();
        short TileX = reader.ReadInt16();
        short TileY = reader.ReadInt16();
        byte PlayerAbove = reader.ReadByte();

        return new WorldDoorInteract
        {
            Action = Action,
            TileX = TileX,
            TileY = TileY,
            PlayerAbove = PlayerAbove
        };
    }

    public static byte[] Serialize(WorldDoorInteract packet)
    {
        FastPacketWriter writer = new(19, 128);

        writer.WriteByte(packet.Action);
        writer.WriteInt16(packet.TileX);
        writer.WriteInt16(packet.TileY);
        writer.WriteByte(packet.PlayerAbove);

        return writer.Build();
    }
}

public struct WorldDoorInteract
{
    public byte Action;
    public short TileX;
    public short TileY;
    public byte PlayerAbove;
}
