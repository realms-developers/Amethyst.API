using Amethyst.Network;
using Amethyst.Server.Entities.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Amethyst.Server.Entities.Items;

public static class ItemUtils
{
    public static void CreateItem(float x, float y, int reservedFor, int type, int stack, byte prefix)
    {
        int itemIndex = Item.NewItem(new EntitySource_DebugCommand(), (int)x, (int)y, 16, 16, type, stack, true, prefix, true);
        Main.item[itemIndex].playerIndexTheItemIsReservedFor = reservedFor;
        NetMessage.SendData(21, reservedFor == 255 ? -1 : reservedFor, -1, NetworkText.Empty, itemIndex, 1);

        if (reservedFor != 255)
        {
            NetMessage.SendData(22, reservedFor, -1, NetworkText.Empty, itemIndex);
        }
    }

    public static void CreateLocalItem(PlayerEntity player, short itemIndex, int type, int stack, byte prefix)
    {
        var network = player.GetNetwork();

        itemIndex = itemIndex == 400 ? FindFreeIndex() : itemIndex;

        if (itemIndex == -1)
        {
            return;
        }

        byte[] itemBasePacket = new PacketWriter().SetType(21)
                                .PackInt16(itemIndex)
                                .PackVector2(player.TPlayer.position)
                                .PackVector2(Vector2.Zero)
                                .PackInt16((short)stack)
                                .PackByte(prefix)
                                .PackByte(0)
                                .PackInt16((short)type)
                                .BuildPacket();

        network.SendPacketBytes(itemBasePacket);

        byte[] itemOwnPacket = new PacketWriter().SetType(22)
                                .PackInt16(itemIndex)
                                .PackByte((byte)player.Index)
                                .BuildPacket();

        network.SendPacketBytes(itemOwnPacket);
    }

    public static void CreateLocalDecorativeItem(PlayerEntity player, short itemIndex, int type, int stack, byte prefix)
    {
        var network = player.GetNetwork();

        itemIndex = itemIndex == 400 ? FindFreeIndex() : itemIndex;

        if (itemIndex == -1)
        {
            return;
        }

        byte[] itemBasePacket = new PacketWriter().SetType(21)
                                .PackInt16(itemIndex)
                                .PackVector2(player.TPlayer.position)
                                .PackVector2(Vector2.Zero)
                                .PackInt16((short)stack)
                                .PackByte(prefix)
                                .PackByte((byte)player.Index)
                                .PackInt16((short)type)
                                .BuildPacket();

        network.SendPacketBytes(itemBasePacket);

        byte[] itemOwnPacket = new PacketWriter().SetType(22)
                                .PackInt16(itemIndex)
                                .PackByte(254)
                                .BuildPacket();

        network.SendPacketBytes(itemOwnPacket);
    }

    public static short FindFreeIndex()
    {
        for (short i = 0; i < Main.item.Length; i++)
        {
            if (Main.item[i]?.active != true)
            {
                return i;
            }
        }

        return -1;
    }
}
