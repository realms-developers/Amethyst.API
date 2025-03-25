using System.Globalization;
using Amethyst.Network;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Amethyst.Players;

public static class PlayerUtilities
{
    public static void BroadcastPacket(byte[] data)
    {
        foreach (var plr in PlayerManager.Tracker) 
            plr.Socket.SendPacket(data);
    }

    public static void BroadcastPacket(byte[] data, Predicate<NetPlayer> predicate)
    {
        foreach (var plr in PlayerManager.Tracker) 
            if (predicate(plr))
                plr.Socket.SendPacket(data);
    }

    public static void BroadcastText(string text, Color color, Predicate<NetPlayer>? predicate = null)
    {
        foreach (var plr in PlayerManager.Tracker) 
            if (predicate == null || predicate(plr))
                plr.SendMessage(text, color);
    }

    public static void BroadcastLocalizedText(string text, object[] args, Color color, Predicate<NetPlayer>? predicate = null)
    {
        foreach (var plr in PlayerManager.Tracker) 
            if (predicate == null || predicate(plr))
            {
                plr.SendMessage(string.Format(CultureInfo.InvariantCulture, plr.Language.LocalizeDirect(text), args), color);
            }
    }

    public static Item TerrarifySlot(NetPlayer player, NetItem netItem, int slot)
    {
        var item = new Item();
        item.SetDefaults(netItem.ID);
        item.stack = netItem.Stack;
        item.prefix = netItem.Prefix;

        if (slot >= PlayerItemSlotID.Loadout3_Dye_0)
            player.TPlayer.Loadouts[2].Dye[slot - PlayerItemSlotID.Loadout3_Dye_0] = item;
        else if (slot >= PlayerItemSlotID.Loadout3_Armor_0)
            player.TPlayer.Loadouts[2].Armor[slot - PlayerItemSlotID.Loadout3_Armor_0] = item;
        else if (slot >= PlayerItemSlotID.Loadout2_Dye_0)
            player.TPlayer.Loadouts[1].Dye[slot - PlayerItemSlotID.Loadout2_Dye_0] = item;
        else if (slot >= PlayerItemSlotID.Loadout2_Armor_0)
            player.TPlayer.Loadouts[1].Armor[slot - PlayerItemSlotID.Loadout2_Armor_0] = item;
        else if (slot >= PlayerItemSlotID.Loadout1_Dye_0)
            player.TPlayer.Loadouts[0].Dye[slot - PlayerItemSlotID.Loadout1_Dye_0] = item;
        else if (slot >= PlayerItemSlotID.Loadout1_Armor_0)
            player.TPlayer.Loadouts[0].Armor[slot - PlayerItemSlotID.Loadout1_Armor_0] = item;
        else if (slot >= PlayerItemSlotID.Bank4_0)
            player.TPlayer.bank4.item[slot - PlayerItemSlotID.Bank4_0] = item;
        else if (slot >= PlayerItemSlotID.Bank3_0)
            player.TPlayer.bank3.item[slot - PlayerItemSlotID.Bank3_0] = item;
        else if (slot >= PlayerItemSlotID.TrashItem)
            player.TPlayer.trashItem = item;
        else if (slot >= PlayerItemSlotID.Bank2_0)
            player.TPlayer.bank2.item[slot - PlayerItemSlotID.Bank2_0] = item;
        else if (slot >= PlayerItemSlotID.Bank1_0)
            player.TPlayer.bank.item[slot - PlayerItemSlotID.Bank1_0] = item;
        else if (slot >= PlayerItemSlotID.MiscDye0)
            player.TPlayer.miscDyes[slot - PlayerItemSlotID.MiscDye0] = item;
        else if (slot >= PlayerItemSlotID.Misc0)
            player.TPlayer.miscEquips[slot - PlayerItemSlotID.Misc0] = item;
        else if (slot >= PlayerItemSlotID.Dye0)
            player.TPlayer.dye[slot - PlayerItemSlotID.Dye0] = item;
        else if (slot >= PlayerItemSlotID.Armor0)
            player.TPlayer.armor[slot - PlayerItemSlotID.Armor0] = item;
        else
            player.TPlayer.inventory[slot - PlayerItemSlotID.Inventory0] = item;

        return item;
    }
}