using Terraria;
using Terraria.Net.Sockets;

namespace Amethyst.Network.Implementation;

internal sealed class BasicNetworkClient : INetworkClient
{
    internal BasicNetworkClient(int index, ISocket socket)
    {
        PlayerIndex = index;

        _socket = socket;
    }

    private readonly ISocket _socket;
    private bool _isFrozen;

    public int PlayerIndex { get; }

    public bool IsConnected => _socket.IsConnected();

    public bool IsFrozen => _isFrozen;

    public void SetFreeze(bool state) => _isFrozen = state;

    public void Disconnect(string reason)
    {
        PacketWriter packet = new PacketWriter()
            .SetType(2)
            .PackByte(0)
            .PackString(reason);

        SendPacket(packet.BuildPacket());
    }

    public void SendPacket(byte[] packet)
    {
        SendPacket(packet, 0, packet.Length);
    }

    public void SendPacket(byte[] packet, int start, int length)
    {
        if (_socket.IsConnected() && _isFrozen == false)
        {
            _socket.AsyncSend(packet, start, length, Netplay.Clients[PlayerIndex].ServerWriteCallBack);
        }
    }
}
