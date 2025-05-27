using System.Buffers;
using System.Net;
using System.Net.Sockets;
using Amethyst.Server.Entities;
using Amethyst.Server.Entities.Players;
using Amethyst.Server.Network.Engine.Utilities;

namespace Amethyst.Server.Network.Engine;

internal sealed class AmethystTcpServer : IDisposable
{
    private readonly Socket _listener;
    private readonly ArrayPool<byte> _pool = ArrayPool<byte>.Shared;

    internal AmethystTcpServer(IPAddress ip, int port)
    {
        _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _listener.Bind(new IPEndPoint(ip, port));
        _listener.Listen(128);
    }

    internal void Start()
    {
        AcceptNext();
    }

    private void AcceptNext()
    {
        var args = new SocketAsyncEventArgs();
        args.Completed += OnAcceptCompleted;
        if (!_listener.AcceptAsync(args))
            OnAcceptCompleted(null, args);
    }

    private void OnAcceptCompleted(object? sender, SocketAsyncEventArgs e)
    {
        var client = e.AcceptSocket;
        e.Dispose();
        AcceptNext();
        if (client != null) HandleClient(client);
    }

    private void HandleClient(Socket socket)
    {
        for (int i = 0; i < 255; i++)
        {
            if (EntityTrackers.Players[i] == null)
            {
                var handler = new NetworkClient(i, socket, new byte[32000]);
                EntityTrackers.Players.Manager!.Insert(i, new PlayerEntity(i, handler));
                handler.Receive();
                return;
            }
        }

        var disconnectPacket = new FastPacketWriter(2, 64);
        disconnectPacket.WriteByte(0); // Disconnect packet type
        disconnectPacket.WriteString("Server is full.");
        try
        {
            socket.Send(disconnectPacket.BuildPacket());
        }
        catch (SocketException)
        {
            // Handle send failure, possibly due to client disconnecting.
        }
        finally
        {
            socket.Dispose();
        }
    }

    public void Dispose()
    {
        _listener.Dispose();
    }
}
