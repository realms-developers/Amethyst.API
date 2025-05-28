using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Amethyst.Server.Network.Engine.Utilities;

// DONT TOUCH THIS CLASS UNLESS YOU KNOW WHAT YOU ARE DOING
// This class is designed for high-perfomance, NOT for flexibility and perfect code.

public unsafe ref struct FastPacketWriter : IDisposable
{
    private byte[] _buffer;
    private GCHandle _handle;
    private byte* _ptr;

    public FastPacketWriter()
    {
        throw new InvalidOperationException("Use the constructor with packet type or capacity.");
    }

    /// <summary>
    /// This is an alternative to BinaryWriter, which does not allow resizing the buffer after creation.
    /// It is designed for high-performance packet writing operations.
    /// </summary>
    /// <param name="capacity">Buffer capacity</param>
    public FastPacketWriter(byte packetType, int capacity = 1024)
    {
        _buffer = new byte[capacity];
        _handle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
        _ptr = (byte*)_handle.AddrOfPinnedObject() + 2;
        WriteByte(packetType);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<T>(T value) where T : unmanaged
    {
        int size = sizeof(T);

        Unsafe.Write(_ptr, value);
        _ptr += size;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteByte(byte value)
    {
        *(_ptr) = value;
        _ptr++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSByte(sbyte value)
    {
        *(_ptr) = (byte)value;
        _ptr++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt16(short value)
    {
        Unsafe.Write(_ptr, value);
        _ptr += 2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt16(ushort value)
    {
        Unsafe.Write(_ptr, value);
        _ptr += 2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt32(int value)
    {
        Unsafe.Write(_ptr, value);
        _ptr += 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt32(uint value)
    {
        Unsafe.Write(_ptr, value);
        _ptr += 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt64(long value)
    {
        Unsafe.Write(_ptr, value);
        _ptr += 8;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt64(ulong value)
    {
        Unsafe.Write(_ptr, value);
        _ptr += 8;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSingle(float value)
    {
        Unsafe.Write(_ptr, value);
        _ptr += 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteDouble(double value)
    {
        Unsafe.Write(_ptr, value);
        _ptr += 8;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteString(string value)
    {
        if (value == null)
        {
            WriteInt32(0);
            return;
        }

        int length = value.Length;
        WriteInt32(length);

        for (int i = 0; i < length; i++)
        {
            WriteByte((byte)value[i]);
        }
    }

    public byte[] BuildPacket()
    {
        int length = (int)(_ptr - (byte*)_handle.AddrOfPinnedObject());
        if (length > _buffer.Length)
            throw new InvalidOperationException("Written span exceeds buffer length.");

        _ptr = (byte*)_handle.AddrOfPinnedObject();
        WriteUInt16((ushort)length);

        Array.Resize(ref _buffer, length);

        return _buffer;
    }

    public void Dispose()
    {
        if (_handle.IsAllocated)
            _handle.Free();
    }
}
