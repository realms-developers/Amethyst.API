using Amethyst.Server.Network.Structures;
using Amethyst.Server.Entities.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Amethyst.Server.Network.Packets;

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
        itemIndex = itemIndex == 400 ? FindFreeIndex() : itemIndex;

        if (itemIndex == -1)
        {
            return;
        }

        byte[] itemBasePacket = new ItemUpdatePacket().Serialize(new ItemUpdate()
        {
            ItemIndex = itemIndex,
            Position = player.TPlayer.position,
            Velocity = Vector2.Zero,
            ItemStack = (short)stack,
            ItemPrefix = prefix,
            ItemType = (short)type
        });

        byte[] itemOwnPacket = new ItemOwnerPacket().Serialize(new ItemOwner()
        {
            ItemIndex = itemIndex,
            PlayerIndex = (byte)player.Index
        });

        player.SendPacketBytes(itemBasePacket);
        player.SendPacketBytes(itemOwnPacket);
    }

    public static void CreateLocalDecorativeItem(PlayerEntity player, short itemIndex, int type, int stack, byte prefix)
    {
        itemIndex = itemIndex == 400 ? FindFreeIndex() : itemIndex;

        if (itemIndex == -1)
        {
            return;
        }

        byte[] itemBasePacket = new ItemUpdatePacket().Serialize(new ItemUpdate()
        {
            ItemIndex = itemIndex,
            Position = player.TPlayer.position,
            Velocity = Vector2.Zero,
            ItemStack = (short)stack,
            ItemPrefix = prefix,
            ItemType = (short)type,
            OwnIgnore = (byte)player.Index
        });

        byte[] itemOwnPacket = new ItemOwnerPacket().Serialize(new ItemOwner()
        {
            ItemIndex = itemIndex,
            PlayerIndex = 254
        });

        player.SendPacketBytes(itemBasePacket);
        player.SendPacketBytes(itemOwnPacket);
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
