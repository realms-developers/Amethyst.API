using Amethyst.Server.Entities;
using Amethyst.Server.Entities.Players;
using Terraria.Localization;
using Terraria;
using Amethyst.Network.Utilities;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Amethyst.Network.Handling.Packets.Handshake;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;

namespace Amethyst.Network.Handling.NetMessagePatch;

internal sealed class NetworkPatcher : NetMessage
{
    internal static unsafe void Initialize()
    {
        On.Terraria.NetMessage.SendData += SendDataPatched;
    }

    private static void SendDataPatched(On.Terraria.NetMessage.orig_SendData orig, int msgType, int remoteClient, int ignoreClient, NetworkText text, int number, float number2, float number3, float number4, int number5, int number6, int number7)
    {
        Task.Run(() =>
        {
            try
            {
                SendDataTask(msgType, remoteClient, ignoreClient, text, number, number2, number3, number4, number5, number6, number7);
            }
            catch (Exception ex)
            {
                AmethystLog.Network.Error(nameof(NetworkPatcher), $"Failed to send data packet: {ex}");
            }
        });
    }
    public static unsafe void SendDataTask(int msgType, int remoteClient, int ignoreClient, NetworkText text, int number, float number2, float number3, float number4, int number5, int number6, int number7)
    {
        if (msgType == 7)
        {
            return;
        }

        if (msgType == 21 && (Main.item[number].shimmerTime > 0f || Main.item[number].shimmered))
        {
            msgType = 145;
        }
        int num = 256;
        if (text == null)
        {
            text = NetworkText.Empty;
        }
        if (Main.netMode == 2 && remoteClient >= 0)
        {
            num = remoteClient;
        }

        var writer = new FastPacketWriter((byte)msgType, 512);
        switch (msgType)
        {
            case 1:
                writer.WriteString("Terraria" + 279);
                break;
            case 2:
                writer.WriteNetText(text);
                break;
            case 3:
                writer.Write((byte)remoteClient);
                writer.Write(value: false);
                break;
            case 4:
                {
                    Player player4 = Main.player[number];
                    writer.Write((byte)number);
                    writer.Write((byte)player4.skinVariant);
                    writer.Write((byte)player4.hair);
                    writer.WriteString(player4.name);
                    writer.Write(player4.hairDye);
                    ushort hideaccs = 0;
                    for (int i = 0; i < player4.hideVisibleAccessory.Length; i++)
                    {
                        if (player4.hideVisibleAccessory[i])
                        {
                            hideaccs |= (ushort)(1 << i);
                        }
                    }

                    writer.Write(num);
                    writer.Write(player4.hideMisc);
                    writer.WriteNetColor(player4.hairColor);
                    writer.WriteNetColor(player4.skinColor);
                    writer.WriteNetColor(player4.eyeColor);
                    writer.WriteNetColor(player4.shirtColor);
                    writer.WriteNetColor(player4.underShirtColor);
                    writer.WriteNetColor(player4.pantsColor);
                    writer.WriteNetColor(player4.shoeColor);
                    BitsByte bitsByte16 = (byte)0;
                    if (player4.difficulty == 1)
                    {
                        bitsByte16[0] = true;
                    }
                    else if (player4.difficulty == 2)
                    {
                        bitsByte16[1] = true;
                    }
                    else if (player4.difficulty == 3)
                    {
                        bitsByte16[3] = true;
                    }
                    bitsByte16[2] = player4.extraAccessory;
                    writer.Write(bitsByte16);
                    BitsByte bitsByte17 = (byte)0;
                    bitsByte17[0] = player4.UsingBiomeTorches;
                    bitsByte17[1] = player4.happyFunTorchTime;
                    bitsByte17[2] = player4.unlockedBiomeTorches;
                    bitsByte17[3] = player4.unlockedSuperCart;
                    bitsByte17[4] = player4.enabledSuperCart;
                    writer.Write(bitsByte17);
                    BitsByte bitsByte18 = (byte)0;
                    bitsByte18[0] = player4.usedAegisCrystal;
                    bitsByte18[1] = player4.usedAegisFruit;
                    bitsByte18[2] = player4.usedArcaneCrystal;
                    bitsByte18[3] = player4.usedGalaxyPearl;
                    bitsByte18[4] = player4.usedGummyWorm;
                    bitsByte18[5] = player4.usedAmbrosia;
                    bitsByte18[6] = player4.ateArtisanBread;
                    writer.Write(bitsByte18);
                    break;
                }
            case 20:
                {
                    int num13 = number;
                    int num14 = (int)number2;
                    int num15 = (int)number3;
                    if (num15 < 0)
                    {
                        num15 = 0;
                    }
                    int num16 = (int)number4;
                    if (num16 < 0)
                    {
                        num16 = 0;
                    }
                    if (num13 < num15)
                    {
                        num13 = num15;
                    }
                    if (num13 >= Main.maxTilesX + num15)
                    {
                        num13 = Main.maxTilesX - num15 - 1;
                    }
                    if (num14 < num16)
                    {
                        num14 = num16;
                    }
                    if (num14 >= Main.maxTilesY + num16)
                    {
                        num14 = Main.maxTilesY - num16 - 1;
                    }
                    writer.Write((short)num13);
                    writer.Write((short)num14);
                    writer.Write((byte)num15);
                    writer.Write((byte)num16);
                    writer.Write((byte)number5);
                    for (int num17 = num13; num17 < num13 + num15; num17++)
                    {
                        for (int num18 = num14; num18 < num14 + num16; num18++)
                        {
                            BitsByte bitsByte19 = (byte)0;
                            BitsByte bitsByte20 = (byte)0;
                            BitsByte bitsByte21 = (byte)0;
                            byte b3 = 0;
                            byte b4 = 0;
                            Tile tile2 = Main.tile[num17, num18];
                            bitsByte19[0] = tile2.active();
                            bitsByte19[2] = tile2.wall > 0;
                            bitsByte19[3] = tile2.liquid > 0 && Main.netMode == 2;
                            bitsByte19[4] = tile2.wire();
                            bitsByte19[5] = tile2.halfBrick();
                            bitsByte19[6] = tile2.actuator();
                            bitsByte19[7] = tile2.inActive();
                            bitsByte20[0] = tile2.wire2();
                            bitsByte20[1] = tile2.wire3();
                            if (tile2.active() && tile2.color() > 0)
                            {
                                bitsByte20[2] = true;
                                b3 = tile2.color();
                            }
                            if (tile2.wall > 0 && tile2.wallColor() > 0)
                            {
                                bitsByte20[3] = true;
                                b4 = tile2.wallColor();
                            }
                            bitsByte20 = (byte)((byte)bitsByte20 + (byte)(tile2.slope() << 4));
                            bitsByte20[7] = tile2.wire4();
                            bitsByte21[0] = tile2.fullbrightBlock();
                            bitsByte21[1] = tile2.fullbrightWall();
                            bitsByte21[2] = tile2.invisibleBlock();
                            bitsByte21[3] = tile2.invisibleWall();
                            writer.Write(bitsByte19);
                            writer.Write(bitsByte20);
                            writer.Write(bitsByte21);
                            if (b3 > 0)
                            {
                                writer.Write(b3);
                            }
                            if (b4 > 0)
                            {
                                writer.Write(b4);
                            }
                            if (tile2.active())
                            {
                                writer.Write(tile2.type);
                                if (Main.tileFrameImportant[tile2.type])
                                {
                                    writer.Write(tile2.frameX);
                                    writer.Write(tile2.frameY);
                                }
                            }
                            if (tile2.wall > 0)
                            {
                                writer.Write(tile2.wall);
                            }
                            if (tile2.liquid > 0 && Main.netMode == 2)
                            {
                                writer.Write(tile2.liquid);
                                writer.Write(tile2.liquidType());
                            }
                        }
                    }
                    break;
                }
            case 21:
            case 90:
            case 145:
            case 148:
                {
                    Item item3 = Main.item[number];
                    writer.Write((short)number);
                    writer.WriteNetVector2(item3.position);
                    writer.WriteNetVector2(item3.velocity);
                    writer.Write((short)item3.stack);
                    writer.Write(item3.prefix);
                    writer.Write((byte)number2);
                    short value2 = 0;
                    if (item3.active && item3.stack > 0)
                    {
                        value2 = (short)item3.netID;
                    }
                    writer.Write(value2);
                    if (msgType == 145)
                    {
                        writer.Write(item3.shimmered);
                        writer.Write(item3.shimmerTime);
                    }
                    if (msgType == 148)
                    {
                        writer.Write((byte)MathHelper.Clamp(item3.timeLeftInWhichTheItemCannotBeTakenByEnemies, 0f, 255f));
                    }
                    break;
                }
            case 22:
                writer.Write((short)number);
                writer.Write((byte)Main.item[number].playerIndexTheItemIsReservedFor);
                break;
            case 23:
                {
                    NPC nPC2 = Main.npc[number];
                    writer.Write((short)number);
                    writer.WriteNetVector2(nPC2.position);
                    writer.WriteNetVector2(nPC2.velocity);
                    writer.Write((ushort)nPC2.target);
                    int num4 = nPC2.life;
                    if (!nPC2.active)
                    {
                        num4 = 0;
                    }
                    if (!nPC2.active || nPC2.life <= 0)
                    {
                        nPC2.netSkip = 0;
                    }
                    short value3 = (short)nPC2.netID;
                    bool[] array = new bool[4];
                    BitsByte bitsByte3 = (byte)0;
                    bitsByte3[0] = nPC2.direction > 0;
                    bitsByte3[1] = nPC2.directionY > 0;
                    bitsByte3[2] = (array[0] = nPC2.ai[0] != 0f);
                    bitsByte3[3] = (array[1] = nPC2.ai[1] != 0f);
                    bitsByte3[4] = (array[2] = nPC2.ai[2] != 0f);
                    bitsByte3[5] = (array[3] = nPC2.ai[3] != 0f);
                    bitsByte3[6] = nPC2.spriteDirection > 0;
                    bitsByte3[7] = num4 == nPC2.lifeMax;
                    writer.Write(bitsByte3);
                    BitsByte bitsByte4 = (byte)0;
                    bitsByte4[0] = nPC2.statsAreScaledForThisManyPlayers > 1;
                    bitsByte4[1] = nPC2.SpawnedFromStatue;
                    bitsByte4[2] = nPC2.strengthMultiplier != 1f;
                    writer.Write(bitsByte4);
                    for (int m = 0; m < NPC.maxAI; m++)
                    {
                        if (array[m])
                        {
                            writer.Write(nPC2.ai[m]);
                        }
                    }
                    writer.Write(value3);
                    if (bitsByte4[0])
                    {
                        writer.Write((byte)nPC2.statsAreScaledForThisManyPlayers);
                    }
                    if (bitsByte4[2])
                    {
                        writer.Write(nPC2.strengthMultiplier);
                    }
                    if (!bitsByte3[7])
                    {
                        byte b2 = 1;
                        if (nPC2.lifeMax > 32767)
                        {
                            b2 = 4;
                        }
                        else if (nPC2.lifeMax > 127)
                        {
                            b2 = 2;
                        }
                        writer.Write(b2);
                        switch (b2)
                        {
                            case 2:
                                writer.Write((short)num4);
                                break;
                            case 4:
                                writer.Write(num4);
                                break;
                            default:
                                writer.Write((sbyte)num4);
                                break;
                        }
                    }
                    if (nPC2.type >= 0 && nPC2.type < NPCID.Count && Main.npcCatchable[nPC2.type])
                    {
                        writer.Write((byte)nPC2.releaseOwner);
                    }
                    break;
                }
            case 24:
                writer.Write((short)number);
                writer.Write((byte)number2);
                break;
            case 107:
                writer.Write((byte)number2);
                writer.Write((byte)number3);
                writer.Write((byte)number4);
                writer.WriteNetText(text);
                writer.Write((short)number5);
                break;
            case 27:
                {
                    Projectile projectile = Main.projectile[number];
                    writer.Write((short)projectile.identity);
                    writer.WriteNetVector2(projectile.position);
                    writer.WriteNetVector2(projectile.velocity);
                    writer.Write((byte)projectile.owner);
                    writer.Write((short)projectile.type);
                    BitsByte bitsByte23 = (byte)0;
                    BitsByte bitsByte24 = (byte)0;
                    bitsByte23[0] = projectile.ai[0] != 0f;
                    bitsByte23[1] = projectile.ai[1] != 0f;
                    bitsByte24[0] = projectile.ai[2] != 0f;
                    if (projectile.bannerIdToRespondTo != 0)
                    {
                        bitsByte23[3] = true;
                    }
                    if (projectile.damage != 0)
                    {
                        bitsByte23[4] = true;
                    }
                    if (projectile.knockBack != 0f)
                    {
                        bitsByte23[5] = true;
                    }
                    if (projectile.type > 0 && projectile.type < ProjectileID.Count && ProjectileID.Sets.NeedsUUID[projectile.type])
                    {
                        bitsByte23[7] = true;
                    }
                    if (projectile.originalDamage != 0)
                    {
                        bitsByte23[6] = true;
                    }
                    if ((byte)bitsByte24 != 0)
                    {
                        bitsByte23[2] = true;
                    }
                    writer.Write(bitsByte23);
                    if (bitsByte23[2])
                    {
                        writer.Write(bitsByte24);
                    }
                    if (bitsByte23[0])
                    {
                        writer.Write(projectile.ai[0]);
                    }
                    if (bitsByte23[1])
                    {
                        writer.Write(projectile.ai[1]);
                    }
                    if (bitsByte23[3])
                    {
                        writer.Write((ushort)projectile.bannerIdToRespondTo);
                    }
                    if (bitsByte23[4])
                    {
                        writer.Write((short)projectile.damage);
                    }
                    if (bitsByte23[5])
                    {
                        writer.Write(projectile.knockBack);
                    }
                    if (bitsByte23[6])
                    {
                        writer.Write((short)projectile.originalDamage);
                    }
                    if (bitsByte23[7])
                    {
                        writer.Write((short)projectile.projUUID);
                    }
                    if (bitsByte24[0])
                    {
                        writer.Write(projectile.ai[2]);
                    }
                    break;
                }
            case 28:
                writer.Write((short)number);
                writer.Write((short)number2);
                writer.Write(number3);
                writer.Write((byte)(number4 + 1f));
                writer.Write((byte)number5);
                break;
            case 29:
                writer.Write((short)number);
                writer.Write((byte)number2);
                break;
            case 30:
                writer.Write((byte)number);
                writer.Write(Main.player[number].hostile);
                break;
            case 31:
                writer.Write((short)number);
                writer.Write((short)number2);
                break;
            case 32:
                {
                    Item item7 = Main.chest[number].item[(byte)number2];
                    writer.Write((short)number);
                    writer.Write((byte)number2);
                    short value4 = (short)item7.netID;
                    if (item7.Name == null)
                    {
                        value4 = 0;
                    }
                    writer.Write((short)item7.stack);
                    writer.Write(item7.prefix);
                    writer.Write(value4);
                    break;
                }
            case 33:
                {
                    int num5 = 0;
                    int num6 = 0;
                    int num7 = 0;
                    string? text2 = null;
                    if (number > -1)
                    {
                        num5 = Main.chest[number].x;
                        num6 = Main.chest[number].y;
                    }
                    if (number2 == 1f)
                    {
                        string text3 = text.ToString();
                        num7 = (byte)text3.Length;
                        if (num7 == 0 || num7 > 20)
                        {
                            num7 = 255;
                        }
                        else
                        {
                            text2 = text3;
                        }
                    }
                    writer.Write((short)number);
                    writer.Write((short)num5);
                    writer.Write((short)num6);
                    writer.Write((byte)num7);
                    if (text2 != null)
                    {
                        writer.WriteString(text2);
                    }
                    break;
                }
            case 34:
                writer.Write((byte)number);
                writer.Write((short)number2);
                writer.Write((short)number3);
                writer.Write((short)number4);
                if (Main.netMode == 2)
                {
                    Netplay.GetSectionX((int)number2);
                    Netplay.GetSectionY((int)number3);
                    writer.Write((short)number5);
                }
                else
                {
                    writer.Write((short)0);
                }
                break;
            case 35:
                writer.Write((byte)number);
                writer.Write((short)number2);
                break;
            case 36:
                {
                    Player player3 = Main.player[number];
                    writer.Write((byte)number);
                    writer.Write(player3.zone1);
                    writer.Write(player3.zone2);
                    writer.Write(player3.zone3);
                    writer.Write(player3.zone4);
                    writer.Write(player3.zone5);
                    break;
                }
            case 38:
                writer.WriteString(Netplay.ServerPassword);
                break;
            case 39:
                writer.Write((short)number);
                break;
            case 40:
                writer.Write((byte)number);
                writer.Write((short)Main.player[number].talkNPC);
                break;
            case 41:
                writer.Write((byte)number);
                writer.Write(Main.player[number].itemRotation);
                writer.Write((short)Main.player[number].itemAnimation);
                break;
            case 42:
                writer.Write((byte)number);
                writer.Write((short)Main.player[number].statMana);
                writer.Write((short)Main.player[number].statManaMax);
                break;
            case 43:
                writer.Write((byte)number);
                writer.Write((short)number2);
                break;
            case 45:
                writer.Write((byte)number);
                writer.Write((byte)Main.player[number].team);
                break;
            case 46:
                writer.Write((short)number);
                writer.Write((short)number2);
                break;
            case 47:
                writer.Write((short)number);
                writer.Write((short)Main.sign[number].x);
                writer.Write((short)Main.sign[number].y);
                writer.WriteString(Main.sign[number].text);
                writer.Write((byte)number2);
                writer.Write((byte)number3);
                break;
            case 48:
                {
                    Tile tile = Main.tile[number, (int)number2];
                    writer.Write((short)number);
                    writer.Write((short)number2);
                    writer.Write(tile.liquid);
                    writer.Write(tile.liquidType());
                    break;
                }
            case 50:
                {
                    writer.Write((byte)number);
                    for (int l = 0; l < Player.maxBuffs; l++)
                    {
                        writer.Write((ushort)Main.player[number].buffType[l]);
                    }
                    break;
                }
            case 51:
                writer.Write((byte)number);
                writer.Write((byte)number2);
                break;
            case 52:
                writer.Write((byte)number2);
                writer.Write((short)number3);
                writer.Write((short)number4);
                break;
            case 53:
                writer.Write((short)number);
                writer.Write((ushort)number2);
                writer.Write((short)number3);
                break;
            case 54:
                {
                    writer.Write((short)number);
                    for (int k = 0; k < NPC.maxBuffs; k++)
                    {
                        writer.Write((ushort)Main.npc[number].buffType[k]);
                        writer.Write((short)Main.npc[number].buffTime[k]);
                    }
                    break;
                }
            case 55:
                writer.Write((byte)number);
                writer.Write((ushort)number2);
                writer.Write((int)number3);
                break;
            case 56:
                writer.Write((short)number);
                if (Main.netMode == 2)
                {
                    string givenName = Main.npc[number].GivenName;
                    writer.WriteString(givenName);
                    writer.Write(Main.npc[number].townNpcVariationIndex);
                }
                break;
            case 57:
                writer.Write(WorldGen.tGood);
                writer.Write(WorldGen.tEvil);
                writer.Write(WorldGen.tBlood);
                break;
            case 58:
                writer.Write((byte)number);
                writer.Write(number2);
                break;
            case 59:
                writer.Write((short)number);
                writer.Write((short)number2);
                break;
            case 60:
                writer.Write((short)number);
                writer.Write((short)number2);
                writer.Write((short)number3);
                writer.Write((byte)number4);
                break;
            case 61:
                writer.Write((short)number);
                writer.Write((short)number2);
                break;
            case 62:
                writer.Write((byte)number);
                writer.Write((byte)number2);
                break;
            case 63:
            case 64:
                writer.Write((short)number);
                writer.Write((short)number2);
                writer.Write((byte)number3);
                writer.Write((byte)number4);
                break;
            case 65:
                {
                    BitsByte bitsByte29 = (byte)0;
                    bitsByte29[0] = (number & 1) == 1;
                    bitsByte29[1] = (number & 2) == 2;
                    bitsByte29[2] = number6 == 1;
                    bitsByte29[3] = number7 != 0;
                    writer.Write(bitsByte29);
                    writer.Write((short)number2);
                    writer.Write(number3);
                    writer.Write(number4);
                    writer.Write((byte)number5);
                    if (bitsByte29[3])
                    {
                        writer.Write(number7);
                    }
                    break;
                }
            case 66:
                writer.Write((byte)number);
                writer.Write((short)number2);
                break;
            case 68:
                writer.WriteString(Main.clientUUID);
                break;
            case 69:
                Netplay.GetSectionX((int)number2);
                Netplay.GetSectionY((int)number3);
                writer.Write((short)number);
                writer.Write((short)number2);
                writer.Write((short)number3);
                writer.WriteString(Main.chest[(short)number].name);
                break;
            case 70:
                writer.Write((short)number);
                writer.Write((byte)number2);
                break;
            case 71:
                writer.Write(number);
                writer.Write((int)number2);
                writer.Write((short)number3);
                writer.Write((byte)number4);
                break;
            case 72:
                {
                    for (int num20 = 0; num20 < 40; num20++)
                    {
                        writer.Write((short)Main.travelShop[num20]);
                    }
                    break;
                }
            case 73:
                writer.Write((byte)number);
                break;
            case 74:
                {
                    writer.Write((byte)Main.anglerQuest);
                    bool value7 = Main.anglerWhoFinishedToday.Contains(text.ToString());
                    writer.Write(value7);
                    break;
                }
            case 76:
                writer.Write((byte)number);
                writer.Write(Main.player[number].anglerQuestsFinished);
                writer.Write(Main.player[number].golferScoreAccumulated);
                break;
            case 77:
                writer.Write((short)number);
                writer.Write((ushort)number2);
                writer.Write((short)number3);
                writer.Write((short)number4);
                break;
            case 78:
                writer.Write(number);
                writer.Write((int)number2);
                writer.Write((sbyte)number3);
                writer.Write((sbyte)number4);
                break;
            case 79:
                writer.Write((short)number);
                writer.Write((short)number2);
                writer.Write((short)number3);
                writer.Write((short)number4);
                writer.Write((byte)number5);
                writer.Write((sbyte)number6);
                writer.Write(number7 == 1);
                break;
            case 80:
                writer.Write((byte)number);
                writer.Write((short)number2);
                break;
            case 81:
                {
                    writer.Write(number2);
                    writer.Write(number3);
                    Color c2 = default(Color);
                    c2.PackedValue = (uint)number;
                    writer.WriteNetColor(c2);
                    writer.Write((int)number4);
                    break;
                }
            case 119:
                {
                    writer.Write(number2);
                    writer.Write(number3);
                    Color c = default(Color);
                    c.PackedValue = (uint)number;
                    writer.WriteNetColor(c);
                    writer.WriteNetText(text);
                    break;
                }
            case 83:
                {
                    int num19 = number;
                    if (num19 < 0 && num19 >= 290)
                    {
                        num19 = 1;
                    }
                    int value6 = NPC.killCount[num19];
                    writer.Write((short)num19);
                    writer.Write(value6);
                    break;
                }
            case 84:
                {
                    byte b5 = (byte)number;
                    float stealth = Main.player[b5].stealth;
                    writer.Write(b5);
                    writer.Write(stealth);
                    break;
                }
            case 85:
                {
                    short value5 = (short)number;
                    writer.Write(value5);
                    break;
                }
            case 86:
                {
                    writer.Write(number);
                    bool flag3 = TileEntity.ByID.ContainsKey(number);
                    writer.Write(flag3);
                    if (flag3)
                    {
                        var stream86 = writer.StreamOpen();
                        BinaryWriter bwriter86 = new BinaryWriter(stream86);
                        TileEntity.Write(bwriter86, TileEntity.ByID[number], networkSend: true);
                        writer.StreamClose(stream86);
                    }
                    break;
                }
            case 87:
                writer.Write((short)number);
                writer.Write((short)number2);
                writer.Write((byte)number3);
                break;
            case 88:
                {
                    BitsByte bitsByte = (byte)number2;
                    BitsByte bitsByte2 = (byte)number3;
                    writer.Write((short)number);
                    writer.Write(bitsByte);
                    Item item5 = Main.item[number];
                    if (bitsByte[0])
                    {
                        writer.Write(item5.color.PackedValue);
                    }
                    if (bitsByte[1])
                    {
                        writer.Write((ushort)item5.damage);
                    }
                    if (bitsByte[2])
                    {
                        writer.Write(item5.knockBack);
                    }
                    if (bitsByte[3])
                    {
                        writer.Write((ushort)item5.useAnimation);
                    }
                    if (bitsByte[4])
                    {
                        writer.Write((ushort)item5.useTime);
                    }
                    if (bitsByte[5])
                    {
                        writer.Write((short)item5.shoot);
                    }
                    if (bitsByte[6])
                    {
                        writer.Write(item5.shootSpeed);
                    }
                    if (bitsByte[7])
                    {
                        writer.Write(bitsByte2);
                        if (bitsByte2[0])
                        {
                            writer.Write((ushort)item5.width);
                        }
                        if (bitsByte2[1])
                        {
                            writer.Write((ushort)item5.height);
                        }
                        if (bitsByte2[2])
                        {
                            writer.Write(item5.scale);
                        }
                        if (bitsByte2[3])
                        {
                            writer.Write((short)item5.ammo);
                        }
                        if (bitsByte2[4])
                        {
                            writer.Write((short)item5.useAmmo);
                        }
                        if (bitsByte2[5])
                        {
                            writer.Write(item5.notAmmo);
                        }
                    }
                    break;
                }
            case 89:
                {
                    writer.Write((short)number);
                    writer.Write((short)number2);
                    Item item4 = Main.player[(int)number4].inventory[(int)number3];
                    writer.Write((short)item4.netID);
                    writer.Write(item4.prefix);
                    writer.Write((short)number5);
                    break;
                }
            case 91:
                writer.Write(number);
                writer.Write((byte)number2);
                if (number2 != 255f)
                {
                    writer.Write((ushort)number3);
                    writer.Write((ushort)number4);
                    writer.Write((byte)number5);
                    if (number5 < 0)
                    {
                        writer.Write((short)number6);
                    }
                }
                break;
            case 92:
                writer.Write((short)number);
                writer.Write((int)number2);
                writer.Write(number3);
                writer.Write(number4);
                break;
            case 95:
                writer.Write((ushort)number);
                writer.Write((byte)number2);
                break;
            case 96:
                {
                    writer.Write((byte)number);
                    Player player2 = Main.player[number];
                    writer.Write((short)number4);
                    writer.Write(number2);
                    writer.Write(number3);
                    writer.WriteNetVector2(player2.velocity);
                    break;
                }
            case 97:
                writer.Write((short)number);
                break;
            case 98:
                writer.Write((short)number);
                break;
            case 99:
                writer.Write((byte)number);
                writer.WriteNetVector2(Main.player[number].MinionRestTargetPoint);
                break;
            case 115:
                writer.Write((byte)number);
                writer.Write((short)Main.player[number].MinionAttackTargetNPC);
                break;
            case 100:
                {
                    writer.Write((ushort)number);
                    NPC nPC = Main.npc[number];
                    writer.Write((short)number4);
                    writer.Write(number2);
                    writer.Write(number3);
                    writer.WriteNetVector2(nPC.velocity);
                    break;
                }
            case 101:
                writer.Write((ushort)NPC.ShieldStrengthTowerSolar);
                writer.Write((ushort)NPC.ShieldStrengthTowerVortex);
                writer.Write((ushort)NPC.ShieldStrengthTowerNebula);
                writer.Write((ushort)NPC.ShieldStrengthTowerStardust);
                break;
            case 102:
                writer.Write((byte)number);
                writer.Write((ushort)number2);
                writer.Write(number3);
                writer.Write(number4);
                break;
            case 103:
                writer.Write(NPC.MaxMoonLordCountdown);
                writer.Write(NPC.MoonLordCountdown);
                break;
            case 104:
                writer.Write((byte)number);
                writer.Write((short)number2);
                writer.Write(((short)number3 < 0) ? 0f : number3);
                writer.Write((byte)number4);
                writer.Write(number5);
                writer.Write((byte)number6);
                break;
            case 105:
                writer.Write((short)number);
                writer.Write((short)number2);
                writer.Write(number3 == 1f);
                break;
            case 106:
                writer.Write(new HalfVector2(number, number2).PackedValue);
                break;
            case 108:
                writer.Write((short)number);
                writer.Write(number2);
                writer.Write((short)number3);
                writer.Write((short)number4);
                writer.Write((short)number5);
                writer.Write((short)number6);
                writer.Write((byte)number7);
                break;
            case 109:
                writer.Write((short)number);
                writer.Write((short)number2);
                writer.Write((short)number3);
                writer.Write((short)number4);
                writer.Write((byte)number5);
                break;
            case 110:
                writer.Write((short)number);
                writer.Write((short)number2);
                writer.Write((byte)number3);
                break;
            case 112:
                writer.Write((byte)number);
                writer.Write((int)number2);
                writer.Write((int)number3);
                writer.Write((byte)number4);
                writer.Write((short)number5);
                break;
            case 113:
                writer.Write((short)number);
                writer.Write((short)number2);
                break;
            case 116:
                writer.Write(number);
                break;
            case 117:
                writer.Write((byte)number);
                writer.WriteNetDeathReason(_currentPlayerDeathReason);
                writer.Write((short)number2);
                writer.Write((byte)(number3 + 1f));
                writer.Write((byte)number4);
                writer.Write((sbyte)number5);
                break;
            case 118:
                writer.Write((byte)number);
                writer.WriteNetDeathReason(_currentPlayerDeathReason);
                writer.Write((short)number2);
                writer.Write((byte)(number3 + 1f));
                writer.Write((byte)number4);
                break;
            case 120:
                writer.Write((byte)number);
                writer.Write((byte)number2);
                break;
            case 121:
                {
                    int num3 = (int)number3;
                    bool flag2 = number4 == 1f;
                    if (flag2)
                    {
                        num3 += 8;
                    }
                    writer.Write((byte)number);
                    writer.Write((int)number2);
                    writer.Write((byte)num3);
                    if (TileEntity.ByID[(int)number2] is TEDisplayDoll tEDisplayDoll)
                    {
                        var stream121 = writer.StreamOpen();
                        BinaryWriter bwriter121 = new BinaryWriter(stream121);
                        tEDisplayDoll.WriteItem((int)number3, bwriter121, flag2);
                        writer.StreamClose(stream121);
                        break;
                    }
                    writer.Write(0);
                    writer.Write((byte)0);
                    break;
                }
            case 122:
                writer.Write(number);
                writer.Write((byte)number2);
                break;
            case 123:
                {
                    writer.Write((short)number);
                    writer.Write((short)number2);
                    Item item2 = Main.player[(int)number4].inventory[(int)number3];
                    writer.Write((short)item2.netID);
                    writer.Write(item2.prefix);
                    writer.Write((short)number5);
                    break;
                }
            case 124:
                {
                    int num2 = (int)number3;
                    bool flag = number4 == 1f;
                    if (flag)
                    {
                        num2 += 2;
                    }
                    writer.Write((byte)number);
                    writer.Write((int)number2);
                    writer.Write((byte)num2);
                    if (TileEntity.ByID[(int)number2] is TEHatRack tEHatRack)
                    {
                        var stream124 = writer.StreamOpen();
                        BinaryWriter bwriter124 = new BinaryWriter(stream124);
                        tEHatRack.WriteItem((int)number3, bwriter124, flag);
                        writer.StreamClose(stream124);
                        break;
                    }
                    writer.Write(0);
                    writer.Write((byte)0);
                    break;
                }
            case 125:
                writer.Write((byte)number);
                writer.Write((short)number2);
                writer.Write((short)number3);
                writer.Write((byte)number4);
                break;
            case 126:
                var stream = writer.StreamOpen();
                BinaryWriter bwriter = new BinaryWriter(stream);
                _currentRevengeMarker.WriteSelfTo(bwriter);
                writer.StreamClose(stream);
                break;
            case 127:
                writer.Write(number);
                break;
            case 128:
                writer.Write((byte)number);
                writer.Write((ushort)number5);
                writer.Write((ushort)number6);
                writer.Write((ushort)number2);
                writer.Write((ushort)number3);
                break;
            case 130:
                writer.Write((ushort)number);
                writer.Write((ushort)number2);
                writer.Write((short)number3);
                break;
            case 131:
                {
                    writer.Write((ushort)number);
                    writer.Write((byte)number2);
                    byte b = (byte)number2;
                    if (b == 1)
                    {
                        writer.Write((int)number3);
                        writer.Write((short)number4);
                    }
                    break;
                }
            case 132:
                var stream132 = writer.StreamOpen();
                BinaryWriter bwriter132 = new BinaryWriter(stream132);
                _currentNetSoundInfo.WriteSelfTo(bwriter132);
                writer.StreamClose(stream132);
                break;
            case 133:
                {
                    writer.Write((short)number);
                    writer.Write((short)number2);
                    Item item = Main.player[(int)number4].inventory[(int)number3];
                    writer.Write((short)item.netID);
                    writer.Write(item.prefix);
                    writer.Write((short)number5);
                    break;
                }
            case 134:
                {
                    writer.Write((byte)number);
                    Player player = Main.player[number];
                    writer.Write(player.ladyBugLuckTimeLeft);
                    writer.Write(player.torchLuck);
                    writer.Write(player.luckPotion);
                    writer.Write(player.HasGardenGnomeNearby);
                    writer.Write(player.equipmentBasedLuckBonus);
                    writer.Write(player.coinLuck);
                    break;
                }
            case 135:
                writer.Write((byte)number);
                break;
            case 136:
                {
                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            writer.Write((ushort)NPC.cavernMonsterType[i, j]);
                        }
                    }
                    break;
                }
            case 137:
                writer.Write((short)number);
                writer.Write((ushort)number2);
                break;
            case 139:
                {
                    writer.Write((byte)number);
                    bool value = number2 == 1f;
                    writer.Write(value);
                    break;
                }
            case 140:
                writer.Write((byte)number);
                writer.Write((int)number2);
                break;
            case 141:
                writer.Write((byte)number);
                writer.Write((byte)number2);
                writer.Write(number3);
                writer.Write(number4);
                writer.Write(number5);
                writer.Write(number6);
                break;
            case 142:
                {
                    writer.Write((byte)number);
                    Player obj = Main.player[number];
                    var stream142 = writer.StreamOpen();
                    BinaryWriter bwriter142 = new BinaryWriter(stream142);
                    obj.piggyBankProjTracker.Write(bwriter142);
                    obj.voidLensChest.Write(bwriter142);
                    writer.StreamClose(stream142);
                    break;
                }
            case 146:
                writer.Write((byte)number);
                switch (number)
                {
                    case 0:
                        writer.WriteNetVector2(new Vector2((int)number2, (int)number3));
                        break;
                    case 1:
                        writer.WriteNetVector2(new Vector2((int)number2, (int)number3));
                        writer.Write((int)number4);
                        break;
                    case 2:
                        writer.Write((int)number2);
                        break;
                }
                break;
            case 147:
                writer.Write((byte)number);
                writer.Write((byte)number2);
                ushort hideaccs2 = 0;
                for (int i = 0; i < Main.player[number].hideVisibleAccessory.Length; i++)
                {
                    if (Main.player[number].hideVisibleAccessory[i])
                    {
                        hideaccs2 |= (ushort)(1 << i);
                    }
                }
                writer.Write(hideaccs2);
                break;
        }

        byte[] packet = writer.Build();

        switch (msgType)
        {
            case 20:

                int x = number;
                int y = (int)number2;
                int w = (int)number3;
                if (w < 0)
                {
                    w = 0;
                }
                int h = (int)number4;
                if (h < 0)
                {
                    h = 0;
                }
                if (x < w)
                {
                    x = w;
                }
                if (x >= Main.maxTilesX + w)
                {
                    x = Main.maxTilesX - w - 1;
                }
                if (y < h)
                {
                    y = h;
                }
                if (y >= Main.maxTilesY + h)
                {
                    y = Main.maxTilesY - h - 1;
                }

                SendPacket(packet, remoteClient, ignoreClient, (plr) =>
                    plr.Sections.IsSent(x / 200, y / 150) &&
                    plr.Sections.IsSent((int)(x + w) / 200, (int)(y + h) / 150));
                break;

            default:
                SendPacket(packet, remoteClient, ignoreClient);
                break;
        }
    }

    private static void SendPacket(byte[] data, int remote, int ignore, Predicate<PlayerEntity>? filter = null)
    {
        if (remote != -1)
        {
            EntityTrackers.Players[remote]?.SendPacketBytes(data);
        }
        else
        {
            foreach (var plr in EntityTrackers.Players)
            {
                if (plr == null || plr.Phase != ConnectionPhase.Connected || (filter != null && !filter(plr)))
                    continue;

                if (plr.Index == ignore)
                    continue;

                plr.SendPacketBytes(data);
            }
        }
    }
}
