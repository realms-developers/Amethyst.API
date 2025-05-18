using System.Net;
using System.Net.Sockets;
using Amethyst.Infrastructure;
using Terraria;
using Terraria.Net;
using Terraria.Net.Sockets;

namespace Amethyst.Network;

public class RemadeTcpSocket : ISocket, IDisposable
{
    public RemadeTcpSocket()
    {
        _connection = new TcpClient
        {
            NoDelay = true
        };
    }

    public RemadeTcpSocket(TcpClient tcpClient)
    {
        if (tcpClient.Client.RemoteEndPoint == null)
        {
            return;
        }

        _connection = tcpClient;
        _connection.NoDelay = true;
        _connected = true;
        var ipendPoint = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
        _remoteAddress = new TcpAddress(ipendPoint.Address, ipendPoint.Port);
    }

    void ISocket.Close()
    {
        _remoteAddress = null;
        _connection?.Close();
        _connected = false;
    }

    bool ISocket.IsConnected() => _connected && _connection != null && _connection.Client != null && _connection.Connected;

    void ISocket.Connect(RemoteAddress address)
    {
        var tcpAddress = (TcpAddress)address;
        _connection?.Connect(tcpAddress.Address, tcpAddress.Port);
        _remoteAddress = address;
    }

    private void ReadCallback(IAsyncResult result)
    {
        if (result.AsyncState is not Tuple<SocketReceiveCallback, object>)
        {
            return;
        }

        var tuple = (Tuple<SocketReceiveCallback, object>?)result.AsyncState;
        if (tuple == null)
        {
            return;
        }

        try
        {
            if (_connection == null)
            {
                return;
            }

            tuple?.Item1.Invoke(tuple.Item2, _connection.GetStream().EndRead(result));
        }
        catch (InvalidOperationException)
        {
            _connected = false;
            ((ISocket)this).Close();
        }
    }

    private void SendCallback(IAsyncResult result)
    {
        object[]? array = (object[]?)result.AsyncState;
        if (array == null)
        {
            return;
        }

        LegacyNetBufferPool.ReturnBuffer((byte[])array[1]);
        var tuple = (Tuple<SocketSendCallback, object>)array[0];
        try
        {
            _connection?.GetStream().EndWrite(result);
            tuple.Item1.Invoke(tuple.Item2);
        }
        catch (ObjectDisposedException)
        {
            _connected = false;
            ((ISocket)this).Close();
        }
        catch (Exception)
        {
            _connected = false;
            ((ISocket)this).Close();
        }
    }

    void ISocket.SendQueuedPackets()
    {
    }

    void ISocket.AsyncSend(byte[] data, int offset, int size, SocketSendCallback callback, object state)
    {
        try
        {
            byte[] array = LegacyNetBufferPool.RequestBuffer(data, offset, size);
            _connection?.GetStream().BeginWrite(array, 0, size, new AsyncCallback(SendCallback), new object[]
            {
                new Tuple<SocketSendCallback, object>(callback, state),
                array
            });
        }
        catch (ObjectDisposedException)
        {
            _connected = false;
            ((ISocket)this).Close();
        }
        catch (SocketException)
        {
            _connected = false;
            ((ISocket)this).Close();
        }
        catch (IOException)
        {
            _connected = false;
            ((ISocket)this).Close();
        }
    }

    void ISocket.AsyncReceive(byte[] data, int offset, int size, SocketReceiveCallback callback, object state)
    {
        try
        {
            _connection?.GetStream().BeginRead(data, offset, size, new AsyncCallback(ReadCallback),
                new Tuple<SocketReceiveCallback, object>(callback, state));
        }
        catch (ObjectDisposedException)
        {
            _connected = false;
            ((ISocket)this).Close();
        }
    }

    bool ISocket.IsDataAvailable()
    {
        try
        {
            return _connection?.GetStream().DataAvailable ?? false;
        }
        catch (ObjectDisposedException)
        {
            _connected = false;
            ((ISocket)this).Close();
        }

        return false;
    }

    RemoteAddress ISocket.GetRemoteAddress()
    {
        return _remoteAddress!;
    }

    bool ISocket.StartListening(SocketConnectionAccepted callback)
    {
        IPAddress? any = IPAddress.Any;
        if (Netplay.ServerIP != null && !IPAddress.TryParse(Netplay.ServerIPText, out any))
        {
            any = IPAddress.Any;
        }

        _listenerCallback = callback;
        _listener ??= new TcpListener(any, AmethystSession.Profile.Port);
        try
        {
            _listener.Start();
        }
        catch (Exception)
        {
            return false;
        }

        ThreadPool.QueueUserWorkItem(new WaitCallback(ListenLoop));
        return true;
    }

    void ISocket.StopListening()
    {
    }

    private void ListenLoop(object? unused)
    {
        for (; ; )
        {
            try
            {
                if (_listener == null)
                {
                    return;
                }

                TcpClient tcpClient = _listener.AcceptTcpClient();
                if (Netplay.FindNextOpenClientSlot() == -1)
                {
                    continue;
                }

                ISocket socket = new RemadeTcpSocket(tcpClient);
                _listenerCallback?.Invoke(socket);

                Thread.Sleep(100);
            }
            catch (Exception)
            {
            }
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    private readonly TcpClient? _connection;
    private TcpListener? _listener;
    private SocketConnectionAccepted? _listenerCallback;
    private RemoteAddress? _remoteAddress;
    private bool _connected;
}
