using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Amethyst.Network.Utilities;
using Amethyst.Server.Entities.Players;
using Ionic.Zlib;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;
using Terraria.ID;

namespace Amethyst.Network.Handling;

public static partial class PacketSendingUtility
{
    public const int TileSize = sizeof(ushort) * 3 + sizeof(byte) * 4 + sizeof(short) * 2;

    private static BinaryWriter TempWriter = new BinaryWriter(new MemoryStream());

	private static short[] CompressChestList = new short[8000];

	private static short[] CompressSignList = new short[1000];

	private static short[] CompressEntities = new short[1000];

    // if method CompressTileBlock will be invoked twice in same time it will cause epic fails in data
    // because of FreeBufferData, CompressChestList, CompressSignList, CompressEntities
    // so we need to use semaphore to prevent this
    private static readonly SemaphoreSlim _sectionPacketSemaphore = new(1, 1);

    public static byte[] CompressTileBlock(int xStart, int yStart, short width, short height)
    {
        _sectionPacketSemaphore.Wait();

        try
        {
            using MemoryStream stream = new();
            using DeflateStream output = new DeflateStream(stream, CompressionMode.Compress, leaveOpen: true);
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(output))
                {
                    binaryWriter.Write(xStart);
                    binaryWriter.Write(yStart);
                    binaryWriter.Write(width);
                    binaryWriter.Write(height);
                    CompressTileBlockInner(binaryWriter, xStart, yStart, width, height);
                }
            }

            var data = stream.ToArray().AsSpan();
            FastPacketWriter writer = new FastPacketWriter(10, data.Length + 3);
            writer.WriteByteSpan(data);
            return writer.BuildNoResize();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error compressing tile block: {ex.Message}");
            return Array.Empty<byte>();
        }
        finally
        {
            _sectionPacketSemaphore.Release();
        }
    }

	private static unsafe void CompressTileBlockInner(BinaryWriter writer, int startX, int startY, int width, int height)
	{
        short chestCount = 0;
        short signCount = 0;
        short teCount = 0;
        short num4 = 0;
        int num5 = 0;
        int num6 = 0;
        byte b = 0;

        Span<byte> buffer = stackalloc byte[16];
        byte* bufferPtr = (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(buffer));

        Span<short> chestSpan = CompressChestList.AsSpan();
        Span<short> signSpan = CompressSignList.AsSpan();
        Span<short> teSpan = CompressEntities.AsSpan();

        TileData* tile = null;
        for (int i = startY; i < startY + height; i++)
        {
            for (int j = startX; j < startX + width; j++)
            {
                TileData* tile2 = Main.tile[j, i].ptr;
                // if (tile2->isTheSameAs(tile) && TileID.Sets.AllowsSaveCompressionBatching[tile2->type])
                // {
                //     num4++;
                //     continue;
                // }
                if (tile != null)
                {
                    // if (num4 > 255)
                    // {
                    //     b |= 0x80;
                    //     Unsafe.WriteUnaligned(ref buffer[num5], num4);
                    //     num5 += 2;
                    // }
                    // else if (num4 > 0)
                    // {
                    //     b |= 0x40;
                    //     buffer[num5] = (byte)((num4 & 0xFF00) >> 8);
                    //     num5++;
                    // }
                    buffer[num6] = b;
                    writer.Write(buffer.Slice(num6, num5 - num6));
                    //num4 = 0;
                }
                num5 = 4;
                byte b3 = 0, b2 = 0, b4 = 0;
                b = 0;
                if (tile2->active())
                {
                    b |= 2;
                    buffer[num5] = (byte)tile2->type;
                    num5++;
                    if (tile2->type > 255)
                    {
                        buffer[num5] = (byte)(tile2->type >> 8);
                        num5++;
                        b |= 0x20;
                    }

                    // Chests
                    short chestIdx = -1;
                    switch (tile2->type)
                    {
                        case var t when TileID.Sets.BasicChest[t] && tile2->frameX % 36 == 0 && tile2->frameY % 36 == 0:
                            chestIdx = (short)Chest.FindChest(j, i);
                            break;
                        case 88 when tile2->frameX % 54 == 0 && tile2->frameY % 36 == 0:
                            chestIdx = (short)Chest.FindChest(j, i);
                            break;
                    }
                    if (chestIdx != -1)
                        chestSpan[chestCount++] = chestIdx;

                    // Signs
                    short signIdx = -1;
                    switch (tile2->type)
                    {
                        case 85 when tile2->frameX % 36 == 0 && tile2->frameY % 36 == 0:
                        case 55 when tile2->frameX % 36 == 0 && tile2->frameY % 36 == 0:
                        case 425 when tile2->frameX % 36 == 0 && tile2->frameY % 36 == 0:
                        case 573 when tile2->frameX % 36 == 0 && tile2->frameY % 36 == 0:
                            signIdx = (short)Sign.ReadSign(j, i);
                            break;
                    }
                    if (signIdx != -1)
                        signSpan[signCount++] = signIdx;

                    // Tile Entities
                    int teIdx = -1;
                    switch (tile2->type)
                    {
                        case 378 when tile2->frameX % 36 == 0 && tile2->frameY == 0:
                            teIdx = TETrainingDummy.Find(j, i);
                            break;
                        case 395 when tile2->frameX % 36 == 0 && tile2->frameY == 0:
                            teIdx = TEItemFrame.Find(j, i);
                            break;
                        case 520 when tile2->frameX % 18 == 0 && tile2->frameY == 0:
                            teIdx = TEFoodPlatter.Find(j, i);
                            break;
                        case 471 when tile2->frameX % 54 == 0 && tile2->frameY == 0:
                            teIdx = TEWeaponsRack.Find(j, i);
                            break;
                        case 470 when tile2->frameX % 36 == 0 && tile2->frameY == 0:
                            teIdx = TEDisplayDoll.Find(j, i);
                            break;
                        case 475 when tile2->frameX % 54 == 0 && tile2->frameY == 0:
                            teIdx = TEHatRack.Find(j, i);
                            break;
                        case 597 when tile2->frameX % 54 == 0 && tile2->frameY % 72 == 0:
                            teIdx = TETeleportationPylon.Find(j, i);
                            break;
                    }
                    if (teIdx != -1)
                        teSpan[teCount++] = (short)teIdx;

                    if (Main.tileFrameImportant[tile2->type])
                    {
                        // Faster: write directly without masking/shifting each time
                        Unsafe.Write((byte*)(bufferPtr + num5), tile2->frameX);
                        Unsafe.Write((byte*)(bufferPtr + num5 + 2), tile2->frameY);
                        num5 += 4;
                    }
                    var col = tile2->color();
                    if (col != 0)
                    {
                        b3 |= 8;
                        buffer[num5] = col;
                        num5++;
                    }
                }
                if (tile2->wall != 0)
                {
                    b |= 4;
                    buffer[num5] = (byte)tile2->wall;
                    num5++;
                    var wallcol = tile2->wallColor();
                    if (wallcol != 0)
                    {
                        b3 |= 0x10;
                        buffer[num5] = wallcol;
                        num5++;
                    }
                }
                if (tile2->liquid != 0)
                {
                    if (!tile2->shimmer())
                    {
                        b = (tile2->lava() ? ((byte)(b | 0x10)) : ((!tile2->honey()) ? ((byte)(b | 8)) : ((byte)(b | 0x18))));
                    }
                    else
                    {
                        b3 |= 0x80;
                        b |= 8;
                    }
                    buffer[num5] = tile2->liquid;
                    num5++;
                }
                if (tile2->wire())
                {
                    b4 |= 2;
                }
                if (tile2->wire2())
                {
                    b4 |= 4;
                }
                if (tile2->wire3())
                {
                    b4 |= 8;
                }
                b4 |= (byte)(tile2->halfBrick() ? 16 : ((tile2->slope() != 0) ? (tile2->slope() + 1 << 4) : 0));
                if (tile2->actuator())
                {
                    b3 |= 2;
                }
                if (tile2->inActive())
                {
                    b3 |= 4;
                }
                if (tile2->wire4())
                {
                    b3 |= 0x20;
                }
                if (tile2->wall > 255)
                {
                    buffer[num5] = (byte)(tile2->wall >> 8);
                    num5++;
                    b3 |= 0x40;
                }
                if (tile2->invisibleBlock())
                {
                    b2 |= 2;
                }
                if (tile2->invisibleWall())
                {
                    b2 |= 4;
                }
                if (tile2->fullbrightBlock())
                {
                    b2 |= 8;
                }
                if (tile2->fullbrightWall())
                {
                    b2 |= 0x10;
                }
                num6 = 3;
                if (b2 != 0)
                {
                    b3 |= 1;
                    buffer[num6] = b2;
                    num6--;
                }
                if (b3 != 0)
                {
                    b4 |= 1;
                    buffer[num6] = b3;
                    num6--;
                }
                if (b4 != 0)
                {
                    b |= 1;
                    buffer[num6] = b4;
                    num6--;
                }
                tile = tile2;
            }
        }
        if (num4 > 0)
        {
            buffer[num5++] = (byte)num4;
            if (num4 > 255)
            {
                b |= 0x80;
                buffer[num5++] = (byte)(num4 >> 8);
            }
            else
            {
                b |= 0x40;
            }
        }
        buffer[num6] = b;
        writer.Write(buffer.Slice(num6, num5 - num6));
        writer.Write(chestCount);
        for (int k = 0; k < chestCount; k++)
        {
            Chest chest = Main.chest[CompressChestList[k]];
            writer.Write(CompressChestList[k]);
            writer.Write((short)chest.x);
            writer.Write((short)chest.y);
            writer.Write(chest.name);
        }
        writer.Write(signCount);
        for (int l = 0; l < signCount; l++)
        {
            Sign sign = Main.sign[CompressSignList[l]];
            writer.Write(CompressSignList[l]);
            writer.Write((short)sign.x);
            writer.Write((short)sign.y);
            writer.Write(sign.text);
        }
        writer.Write(teCount);
        TempWriter.BaseStream.Position = 0;
        for (int m = 0; m < teCount; m++)
        {
            TileEntity.Write(TempWriter, TileEntity.ByID[CompressEntities[m]]);
        }
	}
}
