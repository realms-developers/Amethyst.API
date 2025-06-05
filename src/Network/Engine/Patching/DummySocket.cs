using System.Net;
using Terraria.Net;
using Terraria.Net.Sockets;

namespace Amethyst.Network.Engine.Patching;

public sealed class DummySocket : ISocket
{
    internal DummySocket(NetworkClient refClient)
    {
        _refClient = refClient;
    }
    private readonly NetworkClient _refClient;

    public void AsyncReceive(byte[] data, int offset, int size, SocketReceiveCallback callback, object state = null!)
    {
    }

    public void AsyncSend(byte[] data, int offset, int size, SocketSendCallback callback, object state = null!)
    {
        byte[] bytes = new byte[size];
        data.AsSpan(offset, size).CopyTo(bytes.AsSpan());
        _refClient.Send(bytes);
    }

    public void Close()
    {
    }

    public void Connect(RemoteAddress address)
    {
    }

    public RemoteAddress GetRemoteAddress()
    {
        return new TcpAddress(IPAddress.Parse("0.0.0.0"), 32767);
    }

    public bool IsConnected()
    {
        return _refClient._socket.Connected;
    }

    public bool IsDataAvailable()
    {
        return false; // no way!
    }

    public void SendQueuedPackets()
    {
        // epic send queued packets code
    }

    public bool StartListening(SocketConnectionAccepted callback)
    {
        return false;
    }

    public void StopListening()
    {
    }
}
