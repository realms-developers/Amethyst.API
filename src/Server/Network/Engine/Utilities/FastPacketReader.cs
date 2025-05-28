using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Amethyst.Server.Network.Engine.Utilities;

public unsafe ref struct FastPacketReader
{
    private ReadOnlySpan<byte> _span;
    private byte* _ptr;

    public FastPacketReader(byte[] buffer, int offset = 0)
    {
        _span = buffer;
        _ptr = (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(_span));
        _ptr += offset;
    }

    public FastPacketReader(ReadOnlySpan<byte> span, int offset = 0)
    {
        _span = span;
        _ptr = (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(_span));
        _ptr += offset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Read<T>() where T : unmanaged
    {
        int size = sizeof(T);
        T value = Unsafe.Read<T>(_ptr);
        _ptr += size;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte ReadByte()
    {
        byte value = *_ptr;
        _ptr++;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sbyte ReadSByte()
    {
        sbyte value = (sbyte)(*_ptr);
        _ptr++;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short ReadInt16()
    {
        short value = Unsafe.Read<short>(_ptr);
        _ptr += 2;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort ReadUInt16()
    {
        ushort value = Unsafe.Read<ushort>(_ptr);
        _ptr += 2;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadInt32()
    {
        int value = Unsafe.Read<int>(_ptr);
        _ptr += 4;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadUInt32()
    {
        uint value = Unsafe.Read<uint>(_ptr);
        _ptr += 4;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadInt64()
    {
        long value = Unsafe.Read<long>(_ptr);
        _ptr += 8;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadUInt64()
    {
        ulong value = Unsafe.Read<ulong>(_ptr);
        _ptr += 8;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> ReadBytes(int count)
    {
        if (count < 0 || count > _span.Length - (_ptr - (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(_span))))
            throw new ArgumentOutOfRangeException(nameof(count), "Count exceeds buffer length.");

        ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(_ptr, count);
        _ptr += count;
        return span;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadString()
    {
        int length = ReadInt32();
        if (length < 0 || length > _span.Length - (_ptr - (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(_span))))
            throw new ArgumentOutOfRangeException(nameof(length), "String length exceeds buffer length.");

        string value = System.Text.Encoding.UTF8.GetString(_ptr, length);
        _ptr += length;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Skip(int count)
    {
        if (count < 0 || count > _span.Length - (_ptr - (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(_span))))
            throw new ArgumentOutOfRangeException(nameof(count), "Count exceeds buffer length.");

        _ptr += count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reset()
    {
        _ptr = (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(_span));
    }
}
