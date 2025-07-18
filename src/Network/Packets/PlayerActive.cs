// Code is generated by the Amethyst.PacketGenerator (v1.0.5.0) tool.
// Do not edit this file manually.
#pragma warning disable CA1051

using Amethyst.Network.Engine.Packets;
using Amethyst.Network.Utilities;

namespace Amethyst.Network.Packets;

public sealed class PlayerActivePacket : IPacket<PlayerActive>
{
    public int PacketID => 14;

    public static PlayerActive Deserialize(ReadOnlySpan<byte> data, int offset = 0)
    {
        FastPacketReader reader = new(data, offset);

        byte PlayerIndex = reader.ReadByte();
        bool State = reader.ReadBoolean();

        return new PlayerActive
        {
            PlayerIndex = PlayerIndex,
            State = State,
        };
    }

    public static byte[] Serialize(PlayerActive packet)
    {
        FastPacketWriter writer = new(14, 16);

        writer.WriteByte(packet.PlayerIndex);
        writer.WriteBoolean(packet.State);

        return writer.Build();
    }
}

public struct PlayerActive
{
    public byte PlayerIndex;
    public bool State;
}
