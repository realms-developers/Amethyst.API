using Microsoft.Xna.Framework;

namespace Amethyst.Network;

public sealed class PacketWriter : IDisposable
{
    internal MemoryStream stream;
    internal BinaryWriter writer;

    public PacketWriter()
    {
        stream = new MemoryStream();
        writer = new BinaryWriter(stream);
        writer.BaseStream.Position = 3L;
    }

    public PacketWriter SetType(short type)
    {
        long position = writer.BaseStream.Position;
        writer.BaseStream.Position = 2L;
        writer.Write(type);
        writer.BaseStream.Position = position;
        return this;
    }

    public PacketWriter PackBoolean(bool flag)
    {
        writer.Write(flag);
        return this;
    }

    public PacketWriter PackSByte(sbyte num)
    {
        writer.Write(num);
        return this;
    }

    public PacketWriter PackByte(byte num)
    {
        writer.Write(num);
        return this;
    }

    public PacketWriter PackInt16(short num)
    {
        writer.Write(num);
        return this;
    }

    public PacketWriter PackUInt16(ushort num)
    {
        writer.Write(num);
        return this;
    }

    public PacketWriter PackInt32(int num)
    {
        writer.Write(num);
        return this;
    }

    public PacketWriter PackUInt32(uint num)
    {
        writer.Write(num);
        return this;
    }

    public PacketWriter PackInt64(long num)
    {
        writer.Write(num);
        return this;
    }

    public PacketWriter PackUInt64(ulong num)
    {
        writer.Write(num);
        return this;
    }

    public PacketWriter PackSingle(float num)
    {
        writer.Write(num);
        return this;
    }

    public PacketWriter PackString(string str)
    {
        writer.Write(str);
        return this;
    }

    public PacketWriter PackColor(byte r, byte g, byte b)
    {
        writer.Write(r);
        writer.Write(g);
        writer.Write(b);
        return this;
    }

    public PacketWriter PackColor(Color color)
    {
        writer.Write(color.R);
        writer.Write(color.G);
        writer.Write(color.B);
        return this;
    }

    public PacketWriter PackVector2(float x, float y)
    {
        writer.Write(x);
        writer.Write(y);
        return this;
    }

    public PacketWriter PackVector2(Vector2 vector2)
    {
        writer.Write(vector2.X);
        writer.Write(vector2.Y);
        return this;
    }

    public PacketWriter PackPoint(int x, int y)
    {
        writer.Write(x);
        writer.Write(y);
        return this;
    }

    public PacketWriter PackPoint(Point point)
    {
        writer.Write(point.X);
        writer.Write(point.Y);
        return this;
    }

    public byte[] BuildPacket()
    {
        long position = writer.BaseStream.Position;
        writer.BaseStream.Position = 0L;
        writer.Write((short)position);
        writer.BaseStream.Position = position;
        return stream.ToArray();
    }

    public void Dispose()
    {
        stream.Dispose();
        writer.Dispose();
        GC.SuppressFinalize(this);
    }
}
