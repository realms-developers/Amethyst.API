namespace Amethyst.Server.Network.Engine.Packets;

public interface IPacket<T>
{
    int PacketID { get; }

    T Deserialize(ReadOnlySpan<byte> data, int offset = 0);
    ReadOnlySpan<byte> Serialize(T packet);
}
