using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Amethyst.Network.Structures;

namespace Amethyst.Network.Utilities;

public unsafe ref struct FastPacketReader
{
    private readonly ReadOnlySpan<byte> _span;
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
    public NetColor ReadNetColor()
    {
        int value = Unsafe.Read<int>(_ptr);
        _ptr += 4;
        return new NetColor(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NetText ReadNetText()
    {
        byte mode = ReadByte();
        string text = ReadString();

        if (mode != 0)
        {
            int substitutionCount = ReadByte();
            NetText[] substitutions = new NetText[substitutionCount];

            for (int i = 0; i < substitutionCount; i++)
            {
                substitutions[i] = ReadNetText();
            }

            return new NetText(mode, text, substitutions);
        }

        return new NetText(mode, text, null);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NetVector2 ReadNetVector2()
    {
        float x = Unsafe.Read<float>(_ptr);
        _ptr += 4;
        float y = Unsafe.Read<float>(_ptr);
        _ptr += 4;
        return new NetVector2(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NetDeathReason ReadNetDeathReason()
    {
        NetBitsByte bitsByte = *_ptr;
        _ptr++;

        int sourcePlayerIndex = -1;
        int sourceNPCIndex = -1;
        int sourceProjectileLocalIndex = -1;
        int sourceOtherIndex = -1;
        int sourceProjectileType = 0;
        int sourceItemType = 0;
        int sourceItemPrefix = 0;
        string? sourceCustomReason = null;

        if (bitsByte[0])
        {
            sourcePlayerIndex = Unsafe.Read<short>(_ptr);
            _ptr += 2;
        }
        if (bitsByte[1])
        {
            sourceNPCIndex = Unsafe.Read<short>(_ptr);
            _ptr += 2;
        }
        if (bitsByte[2])
        {
            sourceProjectileLocalIndex = Unsafe.Read<short>(_ptr);
            _ptr += 2;
        }
        if (bitsByte[3])
        {
            sourceOtherIndex = *_ptr;
            _ptr++;
        }
        if (bitsByte[4])
        {
            sourceProjectileType = Unsafe.Read<short>(_ptr);
            _ptr += 2;
        }
        if (bitsByte[5])
        {
            sourceItemType = Unsafe.Read<short>(_ptr);
            _ptr += 2;
        }
        if (bitsByte[6])
        {
            sourceItemPrefix = *_ptr;
            _ptr++;
        }
        if (bitsByte[7])
        {
            sourceCustomReason = ReadString();
        }
        return new NetDeathReason(sourcePlayerIndex, sourceNPCIndex, sourceProjectileLocalIndex, sourceOtherIndex, sourceProjectileType, sourceItemType, sourceItemPrefix, sourceCustomReason);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NetTrackerData ReadNetTrackerData()
    {
        short expectedOwner = ReadInt16();
        if (expectedOwner == -1)
        {
            return new NetTrackerData();
        }

        short expectedIdentity = ReadInt16();
        short expectedType = ReadInt16();

        return new NetTrackerData((short)expectedOwner, expectedIdentity, expectedType);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadBoolean()
    {
        bool value = *_ptr != 0;
        _ptr++;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool[] ReadBooleanArray(int count)
    {
        bool[] array = new bool[count];
        for (int i = 0; i < count; i++)
        {
            array[i] = *_ptr != 0;
            _ptr++;
        }

        return array;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte ReadByte()
    {
        byte value = *_ptr;
        _ptr++;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] ReadByteArray(int count)
    {
        byte[] array = new byte[count];
        for (int i = 0; i < count; i++)
        {
            array[i] = *_ptr;
            _ptr++;
        }

        return array;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sbyte ReadSByte()
    {
        sbyte value = (sbyte)*_ptr;
        _ptr++;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sbyte[] ReadSByteArray(int count)
    {
        sbyte[] array = new sbyte[count];
        for (int i = 0; i < count; i++)
        {
            array[i] = (sbyte)*_ptr;
            _ptr++;
        }

        return array;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short ReadInt16()
    {
        short value = Unsafe.Read<short>(_ptr);
        _ptr += 2;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short[] ReadInt16Array(int count)
    {
        short[] array = new short[count];
        for (int i = 0; i < count; i++)
        {
            array[i] = Unsafe.Read<short>(_ptr);
            _ptr += 2;
        }

        return array;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort ReadUInt16()
    {
        ushort value = Unsafe.Read<ushort>(_ptr);
        _ptr += 2;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort[] ReadUInt16Array(int count)
    {
        ushort[] array = new ushort[count];
        for (int i = 0; i < count; i++)
        {
            array[i] = Unsafe.Read<ushort>(_ptr);
            _ptr += 2;
        }

        return array;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadInt32()
    {
        int value = Unsafe.Read<int>(_ptr);
        _ptr += 4;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int[] ReadInt32Array(int count)
    {
        int[] array = new int[count];
        for (int i = 0; i < count; i++)
        {
            array[i] = Unsafe.Read<int>(_ptr);
            _ptr += 4;
        }

        return array;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadUInt32()
    {
        uint value = Unsafe.Read<uint>(_ptr);
        _ptr += 4;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint[] ReadUInt32Array(int count)
    {
        uint[] array = new uint[count];
        for (int i = 0; i < count; i++)
        {
            array[i] = Unsafe.Read<uint>(_ptr);
            _ptr += 4;
        }

        return array;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadInt64()
    {
        long value = Unsafe.Read<long>(_ptr);
        _ptr += 8;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long[] ReadInt64Array(int count)
    {
        long[] array = new long[count];
        for (int i = 0; i < count; i++)
        {
            array[i] = Unsafe.Read<long>(_ptr);
            _ptr += 8;
        }

        return array;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadUInt64()
    {
        ulong value = Unsafe.Read<ulong>(_ptr);
        _ptr += 8;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong[] ReadUInt64Array(int count)
    {
        ulong[] array = new ulong[count];
        for (int i = 0; i < count; i++)
        {
            array[i] = Unsafe.Read<ulong>(_ptr);
            _ptr += 8;
        }

        return array;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ReadSingle()
    {
        float value = Unsafe.Read<float>(_ptr);
        _ptr += 4;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float[] ReadSingleArray(int count)
    {
        float[] array = new float[count];
        for (int i = 0; i < count; i++)
        {
            array[i] = Unsafe.Read<float>(_ptr);
            _ptr += 4;
        }

        return array;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double ReadDouble()
    {
        double value = Unsafe.Read<double>(_ptr);
        _ptr += 8;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double[] ReadDoubleArray(int count)
    {
        double[] array = new double[count];
        for (int i = 0; i < count; i++)
        {
            array[i] = Unsafe.Read<double>(_ptr);
            _ptr += 8;
        }

        return array;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> ReadBytesSpan(int count)
    {
        if (count < 0 || count > _span.Length - (_ptr - (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(_span))))
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count exceeds buffer length.");
        }

        ReadOnlySpan<byte> span = new(_ptr, count);
        _ptr += count;
        return span;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadString()
    {
        int length = Read7BitEncodedInt();
        if (length <= 0)
        {
            return string.Empty;
        }

        string value = System.Text.Encoding.UTF8.GetString(_ptr, length);
        _ptr += length;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Read7BitEncodedInt()
    {
        int count = 0;
        int shift = 0;
        byte b;

        if (_ptr >= (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(_span)) + _span.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(_span), "Buffer overflow while reading 7-bit encoded integer.");
        }

        do
        {
            b = *_ptr;
            _ptr++;
            count |= (b & 0x7F) << shift;
            shift += 7;
        }
        while ((b & 0x80) != 0);

        return count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Skip(int count)
    {
        if (count < 0 || count > _span.Length - (_ptr - (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(_span))))
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count exceeds buffer length.");
        }

        _ptr += count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reset()
    {
        _ptr = (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(_span));
    }
}
