using System.Net;
using System.Net.Sockets;
using Amethyst.Server.Entities;
using Amethyst.Server.Entities.Players;
using Amethyst.Network.Utilities;
using Amethyst.Network.Packets;
using Amethyst.Network.Structures;

namespace Amethyst.Network.Core;

internal sealed class AmethystTcpServer : IDisposable
{
    private readonly Socket _listener;

    internal AmethystTcpServer(IPAddress ip, int port)
    {
        _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _listener.Bind(new IPEndPoint(ip, port));
        _listener.Listen(NetworkManager.SocketBacklog);
    }

    internal void Start()
    {
        AcceptNext();
    }

    private void AcceptNext()
    {
        Task.Delay(NetworkManager.SocketAcceptDelay);

        var args = new SocketAsyncEventArgs();
        args.Completed += OnAcceptCompleted;
        if (!_listener.AcceptAsync(args))
            OnAcceptCompleted(null, args);
    }

    private void OnAcceptCompleted(object? sender, SocketAsyncEventArgs e)
    {
        var client = e.AcceptSocket;
        e.Dispose();
        if (client != null) HandleClient(client);

        AcceptNext();
    }

    private void HandleClient(Socket socket)
    {
        try
        {
            AmethystLog.Network.Info(nameof(AmethystTcpServer), $"Handling incoming connection from {socket.RemoteEndPoint}.");

            if (NetworkManager.IsLocked)
            {
                socket.Send(PlayerDisconnectPacket.Serialize(new PlayerDisconnect()
                {
                    Reason = new NetText(0, "Server is locked", null)
                }));
                socket.Dispose();
                return;
            }

            for (int i = 0; i < NetworkManager.MaxPlayers; i++)
            {
                if (EntityTrackers.Players[i] == null)
                {
                    var handler = new NetworkClient(i, socket, new byte[32000]);
                    EntityTrackers.Players.Manager!.Insert(i, new PlayerEntity(i, handler));
                    handler.Receive();
                    return;
                }
            }

            socket.Send(PlayerDisconnectPacket.Serialize(new PlayerDisconnect()
            {
                Reason = new NetText(0, "Server is full", null)
            }));
            socket.Dispose();
        }
        catch
        {
            socket.Dispose();
            AmethystLog.Network.Debug(nameof(AmethystTcpServer), "Failed to handle client connection");
        }
    }

    public void Dispose()
    {
        _listener.Dispose();
    }
}
