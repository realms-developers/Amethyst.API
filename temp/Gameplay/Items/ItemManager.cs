using Amethyst.Gameplay.Players;
using Amethyst.Network;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Amethyst.Gameplay.Items;

public static class ItemManager
{
    public static ItemTracker Tracker { get; } = new ItemTracker();

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

    public static void LocalCreateItem(NetPlayer player, short itemIndex, int type, int stack, byte prefix)
    {
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

        player.Socket.SendPacket(itemBasePacket);

        byte[] itemOwnPacket = new PacketWriter().SetType(22)
                                .PackInt16(itemIndex)
                                .PackByte((byte)player.Index)
                                .BuildPacket();

        player.Socket.SendPacket(itemOwnPacket);
    }

    public static void LocalDecorativeItem(NetPlayer player, short itemIndex, int type, int stack, byte prefix)
    {
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

        player.Socket.SendPacket(itemBasePacket);

        byte[] itemOwnPacket = new PacketWriter().SetType(22)
                                .PackInt16(itemIndex)
                                .PackByte(254)
                                .BuildPacket();

        player.Socket.SendPacket(itemOwnPacket);
    }
}
