namespace Amethyst.Network.Engine.Packets;

public interface IPacket<T>
{
    int PacketID { get; }
}
