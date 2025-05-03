using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Amethyst.Items;

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
}
