using System.Buffers;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Amethyst.Server.Entities;

namespace Amethyst.Server.Network.Core;

internal class NetworkClient : IDisposable
{
    internal int _index;
    internal Socket _socket;
    internal SocketAsyncEventArgs _args;
    internal byte[] _dataBuffer;
    internal int _received;
    internal int _consumed;
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
        while (!_tokenSrc.IsCancellationRequested)
        {
            try
            {
                byte[] packet = _handleQueue.Take(_tokenSrc.Token);

                bool handled = false;
                NetworkManager.InvokeHandlers[packet[2]]?.Invoke(EntityTrackers.Players[_index], packet.AsSpan(3), ref handled);
            }
            catch { }
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

    internal void Receive()
    {
        var args = _args;
        args.SetBuffer(_dataBuffer, _received, _dataBuffer.Length - _received);
        args.Completed += (_, _) => OnReceive();

        if (!_socket.ReceiveAsync(args))
            OnReceive();
    }

    private void OnReceive()
    {
        int bytes = _args.BytesTransferred;
        if (bytes <= 0)
        {
            Dispose();
            return;
        }

        _received += bytes;

        while (true)
        {
            if (!TryGetFullPacket(_dataBuffer.AsSpan(0, _received), out ReadOnlySpan<byte> packet))
                break;

            _handleQueue.Add(packet.ToArray());

            int packetLen = Unsafe.ReadUnaligned<ushort>(ref MemoryMarshal.GetReference(packet));
            _consumed += packetLen;

            Thread.Sleep(1);
        }

        if (_consumed > 0)
        {
            Buffer.BlockCopy(_dataBuffer, _consumed, _dataBuffer, 0, _received - _consumed);
            _received -= _consumed;
            _consumed = 0;
        }

        Receive();
    }

    internal static bool TryGetFullPacket(ReadOnlySpan<byte> buffer, out ReadOnlySpan<byte> packet)
    {
        packet = default;

        if (buffer.Length < 3)
            return false;

        ushort length = Unsafe.ReadUnaligned<ushort>(ref MemoryMarshal.GetReference(buffer));

        if (length < 3 || buffer.Length < length)
            return false;

        packet = buffer.Slice(0, length);
        return true;
    }

    public void Dispose()
    {
        try { _socket?.Shutdown(SocketShutdown.Both); } catch { }
        _socket?.Close();
        _args?.Dispose();

        _tokenSrc.Cancel();
        _tokenSrc.Dispose();
        _handleQueue.Dispose();
    }
}
