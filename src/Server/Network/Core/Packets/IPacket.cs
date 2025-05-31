namespace Amethyst.Server.Network.Core.Packets;

public interface IPacket<T>
{
    int PacketID { get; }

    T Deserialize(ReadOnlySpan<byte> data, int offset = 0);
    byte[] Serialize(T packet);
}
