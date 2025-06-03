using System.Buffers;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Amethyst.Network.Utilities;
using Amethyst.Server.Entities;

namespace Amethyst.Network.Engine;

internal sealed class NetworkClient : IDisposable
{
    internal int _index;
    internal Socket _socket;
    internal SocketAsyncEventArgs _args;
    internal byte[] _dataBuffer;
    internal int _received;
    internal BlockingCollection<byte[]> _handleQueue = new BlockingCollection<byte[]>(new ConcurrentQueue<byte[]>());
    internal CancellationTokenSource _tokenSrc = new CancellationTokenSource();

    internal NetworkClient(int index, Socket socket, byte[] buffer)
    {
        _index = index;
        _socket = socket;
        _dataBuffer = buffer;
        _args = new SocketAsyncEventArgs();

        Task.Run(HandleQueue);
    }

    private void HandleQueue()
    {
        while (_disposed == false && !_tokenSrc.IsCancellationRequested)
        {
            try
            {
                byte[] packet = _handleQueue.Take(_tokenSrc.Token);

                NetworkManager.HandlePacket(this, packet);
                ArrayPool<byte>.Shared.Return(packet);
            }
            catch
            {
            }
        }
    }

    internal void Send(byte[] data)
    {
        var args = new SocketAsyncEventArgs();
        args.SetBuffer(data, 0, data.Length);
        args.Completed += (_, e) =>
        {
            e.Dispose();
        };

        if (!_socket.SendAsync(args))
        {
            args.Dispose();
        }
    }

    private SocketAsyncEventArgs? _prevArgs;
    internal void Receive()
    {
        _prevArgs?.Dispose();
        _prevArgs = null;

        var args = new SocketAsyncEventArgs();
        args.SetBuffer(_dataBuffer, _received, _dataBuffer.Length - _received);
        args.Completed += OnReceiveCompleted;

        if (!_socket.ReceiveAsync(args))
        {
            Task.Delay(5);
            OnReceiveCompleted(null, args);
        }
    }

    private void OnReceiveCompleted(object? _, SocketAsyncEventArgs e)
    {
        if (OnReceive(e))
        {
            if (_disposed)
                return;

            _prevArgs = e;
            Receive();
        }
    }

    private unsafe bool OnReceive(SocketAsyncEventArgs args)
    {
        int bytes = args.BytesTransferred;
        if (bytes <= 0)
        {
            Dispose();
            return false;
        }

        _received += bytes;

        if (_received < 2)
        {
            // not enough data to read the length
            return true;
        }

        var span = _dataBuffer.AsSpan(0, 2);
        ushort length = Unsafe.Read<ushort>((byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span)));

        if (length < 3 || length > 1000)
        {
            AmethystLog.Network.Error(nameof(NetworkClient), $"Received invalid packet length: {length}");
            Dispose();
            return false;
        }

        if (_received < length)
        {
            // not enough data to read the full packet
            return true;
        }

        // We have a full packet
        var data = ArrayPool<byte>.Shared.Rent(length);
        Buffer.BlockCopy(_dataBuffer, 0, data, 0, length);
        _handleQueue.Add(data);

        if (_received > length)
        {
            // Shift the remaining data to the start of the buffer
            ShiftBuffer(length);
        }
        else
        {
            // Reset the buffer if we received exactly the length of the packet
            _received = 0;
        }

        return true;
    }

    private void ShiftBuffer(int length)
    {
        if (_received - length > 0)
        {
            Buffer.BlockCopy(_dataBuffer, length, _dataBuffer, 0, _received - length);
        }
        _received -= length;
        Array.Clear(_dataBuffer, _received, _dataBuffer.Length - _received);
    }

    private bool _disposed;
    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;

        try { _socket?.Shutdown(SocketShutdown.Both); } catch { }

        _socket?.Close();
        _args?.Dispose();

        _tokenSrc.Cancel();
        _tokenSrc.Dispose();
        _handleQueue.Dispose();
    }
}
