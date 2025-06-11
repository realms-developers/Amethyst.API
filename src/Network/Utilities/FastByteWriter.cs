using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Amethyst.Network.Structures;

namespace Amethyst.Network.Utilities;

// DONT TOUCH THIS CLASS UNLESS YOU KNOW WHAT YOU ARE DOING
// This class is designed for high-perfomance, NOT for flexibility and perfect code.

public unsafe ref struct FastByteWriter : IDisposable
{
    private byte[] _buffer;
    private GCHandle _handle;
    private byte* _ptr;

    public FastByteWriter()
    {
        throw new InvalidOperationException("Use the constructor with packet type or capacity.");
    }

    /// <summary>
    /// This is an alternative to BinaryWriter, which does not allow resizing the buffer after creation.
    /// It is designed for high-performance packet writing operations.
    /// </summary>
    /// <param name="capacity">Buffer capacity</param>
    public FastByteWriter(int capacity = 1024)
    {
        _buffer = new byte[capacity];
        _handle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
        _ptr = (byte*)_handle.AddrOfPinnedObject();
    }
    public FastByteWriter(byte[] array)
    {
        _buffer = array;
        _handle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
        _ptr = (byte*)_handle.AddrOfPinnedObject();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<T>(T value) where T : unmanaged
    {
        int size = sizeof(T);

        Unsafe.Write(_ptr, value);
        _ptr += size;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteNetColor(NetColor value)
    {
        Unsafe.Write(_ptr, value.ToPackedValue());
        _ptr += 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteNetText(NetText value)
    {
        WriteByte(value.Mode);
        WriteString(value.Text);

        if (value.Mode != 0)
        {
            value.Substitutions ??= Array.Empty<NetText>();

            WriteByte((byte)value.Substitutions.Length);
            foreach (NetText substitution in value.Substitutions)
            {
                WriteNetText(substitution);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteNetVector2(NetVector2 value)
    {
        WriteSingle(value.X);
        WriteSingle(value.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteNetDeathReason(NetDeathReason value)
    {
        NetBitsByte bitsByte = new();
        bitsByte[0] = value.SourcePlayerIndex != -1;
        bitsByte[1] = value.SourceNPCIndex != -1;
        bitsByte[2] = value.SourceProjectileLocalIndex != -1;
        bitsByte[3] = value.SourceOtherIndex != -1;
        bitsByte[4] = value.SourceProjectileType != 0;
        bitsByte[5] = value.SourceItemType != 0;
        bitsByte[6] = value.SourceItemPrefix != 0;
        bitsByte[7] = value.SourceCustomReason != null;
        *_ptr = bitsByte.ByteValue;
        _ptr++;
        if (bitsByte[0])
        {
            Unsafe.Write(_ptr, (short)value.SourcePlayerIndex);
            _ptr += 2;
        }
        if (bitsByte[1])
        {
            Unsafe.Write(_ptr, (short)value.SourceNPCIndex);
            _ptr += 2;
        }
        if (bitsByte[2])
        {
            Unsafe.Write(_ptr, (short)value.SourceProjectileLocalIndex);
            _ptr += 2;
        }
        if (bitsByte[3])
        {
            *(_ptr) = (byte)value.SourceOtherIndex;
            _ptr++;
        }
        if (bitsByte[4])
        {
            Unsafe.Write(_ptr, (short)value.SourceProjectileType);
            _ptr += 2;
        }
        if (bitsByte[5])
        {
            Unsafe.Write(_ptr, (short)value.SourceItemType);
            _ptr += 2;
        }
        if (bitsByte[6])
        {
            *(_ptr) = (byte)value.SourceItemPrefix;
            _ptr++;
        }
        if (bitsByte[7])
        {
            WriteString(value.SourceCustomReason!);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteNetTrackerData(NetTrackerData value)
    {
        WriteInt16(value.ExpectedOwner);

        if (value.ExpectedOwner >= 0)
        {
            WriteInt16(value.ExpectedIdentity);
            WriteInt16(value.ExpectedType);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteByte(byte value)
    {
        *(_ptr) = value;
        _ptr++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteByteArray(byte[] values)
    {
        if (values == null || values.Length == 0)
        {
            return;
        }

        int length = values.Length;
        for (int i = 0; i < length; i++)
        {
            *(_ptr) = values[i];
            _ptr++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSByte(sbyte value)
    {
        *(_ptr) = (byte)value;
        _ptr++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSByteArray(sbyte[] values)
    {
        if (values == null || values.Length == 0)
        {
            return;
        }

        int length = values.Length;
        for (int i = 0; i < length; i++)
        {
            *(_ptr) = (byte)values[i];
            _ptr++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBoolean(bool value)
    {
        *(_ptr) = value ? (byte)1 : (byte)0;
        _ptr++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBooleanArray(bool[] values)
    {
        if (values == null || values.Length == 0)
        {
            return;
        }

        int length = values.Length;
        for (int i = 0; i < length; i++)
        {
            *(_ptr) = values[i] ? (byte)1 : (byte)0;
            _ptr++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt16(short value)
    {
        Unsafe.Write(_ptr, value);
        _ptr += 2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt16Array(short[] values)
    {
        if (values == null || values.Length == 0)
        {
            return;
        }

        int length = values.Length;
        for (int i = 0; i < length; i++)
        {
            Unsafe.Write(_ptr, values[i]);
            _ptr += 2;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt16(ushort value)
    {
        Unsafe.Write(_ptr, value);
        _ptr += 2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt16Array(ushort[] values)
    {
        if (values == null || values.Length == 0)
        {
            return;
        }

        int length = values.Length;
        for (int i = 0; i < length; i++)
        {
            Unsafe.Write(_ptr, values[i]);
            _ptr += 2;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt32(int value)
    {
        Unsafe.Write(_ptr, value);
        _ptr += 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt32Array(int[] values)
    {
        if (values == null || values.Length == 0)
        {
            return;
        }

        int length = values.Length;
        for (int i = 0; i < length; i++)
        {
            Unsafe.Write(_ptr, values[i]);
            _ptr += 4;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt32(uint value)
    {
        Unsafe.Write(_ptr, value);
        _ptr += 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt32Array(uint[] values)
    {
        if (values == null || values.Length == 0)
        {
            return;
        }

        int length = values.Length;
        for (int i = 0; i < length; i++)
        {
            Unsafe.Write(_ptr, values[i]);
            _ptr += 4;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt64(long value)
    {
        Unsafe.Write(_ptr, value);
        _ptr += 8;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt64Array(long[] values)
    {
        if (values == null || values.Length == 0)
        {
            return;
        }

        int length = values.Length;
        for (int i = 0; i < length; i++)
        {
            Unsafe.Write(_ptr, values[i]);
            _ptr += 8;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt64(ulong value)
    {
        Unsafe.Write(_ptr, value);
        _ptr += 8;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt64Array(ulong[] values)
    {
        if (values == null || values.Length == 0)
        {
            return;
        }

        int length = values.Length;
        for (int i = 0; i < length; i++)
        {
            Unsafe.Write(_ptr, values[i]);
            _ptr += 8;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSingle(float value)
    {
        Unsafe.Write(_ptr, value);
        _ptr += 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSingleArray(float[] values)
    {
        if (values == null || values.Length == 0)
        {
            return;
        }

        int length = values.Length;
        for (int i = 0; i < length; i++)
        {
            Unsafe.Write(_ptr, values[i]);
            _ptr += 4;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteDouble(double value)
    {
        Unsafe.Write(_ptr, value);
        _ptr += 8;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteDoubleArray(double[] values)
    {
        if (values == null || values.Length == 0)
        {
            return;
        }

        int length = values.Length;
        for (int i = 0; i < length; i++)
        {
            Unsafe.Write(_ptr, values[i]);
            _ptr += 8;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteString(string value)
    {
        if (value == null)
        {
            WriteByte(0);
            return;
        }

        Span<byte> bytes = System.Text.Encoding.UTF8.GetBytes(value).AsSpan();
        Write7BitEncodedInt(bytes.Length);
        WriteByteSpan(bytes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write7BitEncodedInt(int value)
    {
        uint v = (uint)value;
        while (v >= 0x80)
        {
            WriteByte((byte)(v | 0x80));
            v >>= 7;
        }
        WriteByte((byte)v);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteByteSpan(Span<byte> values)
    {
        Buffer.MemoryCopy(
            source: Unsafe.AsPointer(ref MemoryMarshal.GetReference(values)),
            destination: _ptr,
            destinationSizeInBytes: values.Length,
            sourceBytesToCopy: values.Length);

        _ptr += values.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBytePointer(void* values, int length)
    {
        Buffer.MemoryCopy(
            source: values,
            destination: _ptr,
            destinationSizeInBytes: length,
            sourceBytesToCopy: length);

        _ptr += length;
    }

    public void EnsureCapacity(int additionalCapacity)
    {
        if (_ptr + additionalCapacity > (byte*)_handle.AddrOfPinnedObject() + _buffer.Length)
        {
            int newSize = _buffer.Length + additionalCapacity;
            Array.Resize(ref _buffer, newSize);
            _handle.Free();
            _handle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
            _ptr = (byte*)_handle.AddrOfPinnedObject() + (_ptr - (byte*)_handle.AddrOfPinnedObject());
        }
    }

    public byte[] Build()
    {
        int length = (int)(_ptr - (byte*)_handle.AddrOfPinnedObject());
        if (length > _buffer.Length)
        {
            throw new InvalidOperationException("Written span exceeds buffer length.");
        }

        _ptr = (byte*)_handle.AddrOfPinnedObject();
        WriteUInt16((ushort)length);

        Array.Resize(ref _buffer, length);
        return _buffer;
    }

    public byte[] BuildNoResize()
    {
        int length = (int)(_ptr - (byte*)_handle.AddrOfPinnedObject());
        if (length > _buffer.Length)
        {
            throw new InvalidOperationException("Written span exceeds buffer length.");
        }

        _ptr = (byte*)_handle.AddrOfPinnedObject();
        WriteUInt16((ushort)length);

        return _buffer;
    }

    public void Dispose()
    {
        if (_handle.IsAllocated)
        {
            _handle.Free();
        }
    }
}
