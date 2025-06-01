#pragma warning disable CA1051

using Amethyst.Server.Network.Utilities;
using Terraria;

namespace Amethyst.Server.Network.Structures;

public struct NetTile
{
    public NetTile(FastPacketReader reader)
    {
        var bitsBytes = (BitsByte)reader.ReadByte();
        var bitsBytes2 = (BitsByte)reader.ReadByte();
        var bitsBytes3 = (BitsByte)reader.ReadByte();

        Active = bitsBytes[0];

        if (bitsBytes[4])
        {
            Wire = true;
        }

        if (bitsBytes[5])
        {
            HalfBrick = true;
        }

        if (bitsBytes[6])
        {
            Actuator = true;
        }

        if (bitsBytes[7])
        {
            Inactive = true;
        }

        Wire2 = bitsBytes2[0];
        Wire3 = bitsBytes2[1];

        if (bitsBytes2[2])
        {
            TileColor = reader.ReadByte();
        }

        if (bitsBytes2[3])
        {
            WallColor = reader.ReadByte();
        }

        if (Active)
        {
            TileID = reader.ReadUInt16();
            if (Main.tileFrameImportant[TileID])
            {
                FrameX = reader.ReadInt16();
                FrameY = reader.ReadInt16();
            }

            if (bitsBytes2[4])
            {
                Slope++;
            }

            if (bitsBytes2[5])
            {
                Slope += 2;
            }

            if (bitsBytes2[6])
            {
                Slope += 4;
            }
        }

        if (bitsBytes[3])
        {
            Liquid = reader.ReadByte();
            LiquidID = reader.ReadByte();
        }

        Wire4 = bitsBytes2[7];

        Fullbright = bitsBytes3[0];
        FullbrightWall = bitsBytes3[1];
        Invisible = bitsBytes3[2];
        InvisibleWall = bitsBytes3[3];

        if (bitsBytes[2])
        {
            Wall = reader.ReadUInt16();
        }
    }

    public void Serialize(FastPacketWriter packet)
    {
        BitsByte bitsByte1 = new();

        bitsByte1[0] = Active;
        bitsByte1[2] = Wall > 0;
        bitsByte1[3] = Liquid > 0;
        bitsByte1[4] = Wire;
        bitsByte1[5] = HalfBrick;
        bitsByte1[6] = Actuator;
        bitsByte1[7] = Inactive;

        BitsByte bitsByte2 = new();

        bitsByte2[0] = Wire2;
        bitsByte2[1] = Wire3;
        bitsByte2[2] = Active && TileColor > 0;
        bitsByte2[3] = Wall > 0 && WallColor > 0;
        bitsByte2 = (byte)((byte)bitsByte2 + (byte)(Slope << 4));
        bitsByte2[7] = Wire4;

        BitsByte bitsByte3 = new();

        bitsByte3[0] = Fullbright;
        bitsByte3[1] = FullbrightWall;
        bitsByte3[2] = Invisible;
        bitsByte3[3] = InvisibleWall;

        packet.WriteByte((byte)bitsByte1);
        packet.WriteByte((byte)bitsByte2);
        packet.WriteByte((byte)bitsByte3);

        if (bitsByte2[2])
        {
            packet.WriteByte((byte)TileColor);
        }

        if (bitsByte2[3])
        {
            packet.WriteByte((byte)WallColor);
        }

        if (Active)
        {
            packet.WriteUInt16(TileID);
            if (Main.tileFrameImportant[TileID])
            {
                packet.WriteInt16(FrameX);
                packet.WriteInt16(FrameY);
            }
        }

        if (Wall > 0)
        {
            packet.WriteUInt16(Wall);
        }

        if (Liquid > 0)
        {
            packet.WriteByte(Liquid);
            packet.WriteByte(LiquidID);
        }
    }

    public bool Active;
    public ushort TileID;
    public short FrameX;
    public short FrameY;
    public ushort Wall;
    public byte Liquid;
    public byte LiquidID;
    public bool Wire;
    public bool Wire2;
    public bool Wire3;
    public bool Wire4;
    public bool Inactive;
    public bool HalfBrick;
    public bool Actuator;
    public byte TileColor;
    public byte WallColor;
    public byte Slope;
    public bool Fullbright;
    public bool FullbrightWall;
    public bool Invisible;
    public bool InvisibleWall;
}
