// Code is generated by the Amethyst.PacketGenerator (v1.0.5.0) tool.
// Do not edit this file manually.
#pragma warning disable CA1051

using Amethyst.Network.Engine.Packets;
using Amethyst.Network.Utilities;

namespace Amethyst.Network.Packets;

public sealed class PlayerSlotPacket : IPacket<PlayerSlot>
{
    public int PacketID => 5;

    public static PlayerSlot Deserialize(ReadOnlySpan<byte> data, int offset = 0)
    {
        FastPacketReader reader = new(data, offset);

        byte PlayerIndex = reader.ReadByte();
        short SlotIndex = reader.ReadInt16();
        short ItemStack = reader.ReadInt16();
        byte ItemPrefix = reader.ReadByte();
        short ItemID = reader.ReadInt16();

        return new PlayerSlot
        {
            PlayerIndex = PlayerIndex,
            SlotIndex = SlotIndex,
            ItemStack = ItemStack,
            ItemPrefix = ItemPrefix,
            ItemID = ItemID,
        };
    }

    public static byte[] Serialize(PlayerSlot packet)
    {
        FastPacketWriter writer = new(5, 128);

        writer.WriteByte(packet.PlayerIndex);
        writer.WriteInt16(packet.SlotIndex);
        writer.WriteInt16(packet.ItemStack);
        writer.WriteByte(packet.ItemPrefix);
        writer.WriteInt16(packet.ItemID);

        return writer.Build();
    }
}

public struct PlayerSlot
{
    public byte PlayerIndex;
    public short SlotIndex;
    public short ItemStack;
    public byte ItemPrefix;
    public short ItemID;
}
