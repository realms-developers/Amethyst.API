using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Amethyst.Network.Utilities;

public sealed unsafe class StrippedMemoryStream(byte* bytePtr) : Stream
{
    internal byte* _ptr = bytePtr;
    internal int ptrOffset;

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length => 0;

    public override long Position
    {
        get => 0;
        set {}
    }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        Span<byte> span = buffer.AsSpan(offset, count);
        if (span.Length == 0)
        {
            return 0;
        }

        byte* spanPtr = (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));
        Buffer.MemoryCopy(
            source: _ptr + ptrOffset,
            destination: spanPtr,
            destinationSizeInBytes: count,
            sourceBytesToCopy: count);

        return count;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return 0;
    }

    public override void SetLength(long value)
    {
    }

    public override unsafe void Write(byte[] buffer, int offset, int count)
    {
        Span<byte> span = new(buffer, offset, count);
        if (span.Length == 0)
        {
            return;
        }

        byte* spanPtr = (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));

        Buffer.MemoryCopy(
            source: spanPtr,
            destination: _ptr + ptrOffset,
            destinationSizeInBytes: span.Length,
            sourceBytesToCopy: span.Length);

        ptrOffset += span.Length;
    }
}
