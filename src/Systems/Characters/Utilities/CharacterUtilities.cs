using Amethyst.Network;
using Amethyst.Server.Entities.Players;
using Amethyst.Systems.Characters.Base;
using Terraria;
using Terraria.ID;

namespace Amethyst.Systems.Characters.Utilities;

public static class CharacterUtilities
{
    public static Item TerrarifySlot(PlayerEntity player, NetItem netItem, int slot)
    {
        var item = new Item();
        item.SetDefaults(netItem.ID);
        item.stack = netItem.Stack;
        item.prefix = netItem.Prefix;

        if (slot >= PlayerItemSlotID.Loadout3_Dye_0)
        {
            player.TPlayer.Loadouts[2].Dye[slot - PlayerItemSlotID.Loadout3_Dye_0] = item;
        }
        else if (slot >= PlayerItemSlotID.Loadout3_Armor_0)
        {
            player.TPlayer.Loadouts[2].Armor[slot - PlayerItemSlotID.Loadout3_Armor_0] = item;
        }
        else if (slot >= PlayerItemSlotID.Loadout2_Dye_0)
        {
            player.TPlayer.Loadouts[1].Dye[slot - PlayerItemSlotID.Loadout2_Dye_0] = item;
        }
        else if (slot >= PlayerItemSlotID.Loadout2_Armor_0)
        {
            player.TPlayer.Loadouts[1].Armor[slot - PlayerItemSlotID.Loadout2_Armor_0] = item;
        }
        else if (slot >= PlayerItemSlotID.Loadout1_Dye_0)
        {
            player.TPlayer.Loadouts[0].Dye[slot - PlayerItemSlotID.Loadout1_Dye_0] = item;
        }
        else if (slot >= PlayerItemSlotID.Loadout1_Armor_0)
        {
            player.TPlayer.Loadouts[0].Armor[slot - PlayerItemSlotID.Loadout1_Armor_0] = item;
        }
        else if (slot >= PlayerItemSlotID.Bank4_0)
        {
            player.TPlayer.bank4.item[slot - PlayerItemSlotID.Bank4_0] = item;
        }
        else if (slot >= PlayerItemSlotID.Bank3_0)
        {
            player.TPlayer.bank3.item[slot - PlayerItemSlotID.Bank3_0] = item;
        }
        else if (slot >= PlayerItemSlotID.TrashItem)
        {
            player.TPlayer.trashItem = item;
        }
        else if (slot >= PlayerItemSlotID.Bank2_0)
        {
            player.TPlayer.bank2.item[slot - PlayerItemSlotID.Bank2_0] = item;
        }
        else if (slot >= PlayerItemSlotID.Bank1_0)
        {
            player.TPlayer.bank.item[slot - PlayerItemSlotID.Bank1_0] = item;
        }
        else if (slot >= PlayerItemSlotID.MiscDye0)
        {
            player.TPlayer.miscDyes[slot - PlayerItemSlotID.MiscDye0] = item;
        }
        else if (slot >= PlayerItemSlotID.Misc0)
        {
            player.TPlayer.miscEquips[slot - PlayerItemSlotID.Misc0] = item;
        }
        else if (slot >= PlayerItemSlotID.Dye0)
        {
            player.TPlayer.dye[slot - PlayerItemSlotID.Dye0] = item;
        }
        else if (slot >= PlayerItemSlotID.Armor0)
        {
            player.TPlayer.armor[slot - PlayerItemSlotID.Armor0] = item;
        }
        else
        {
            player.TPlayer.inventory[slot - PlayerItemSlotID.Inventory0] = item;
        }

        return item;
    }

    public static void CopyCharacter(ICharacterModel from, ref ICharacterModel to)
    {
        for (int i = 0; i < from.Slots.Length; i++)
        {
            to.Slots[i] = new NetItem(from.Slots[i].ID, from.Slots[i].Stack, from.Slots[i].Prefix);
        }

        to.MaxLife = from.MaxLife;
        to.MaxMana = from.MaxMana;
        to.Info1 = from.Info1;
        to.Info2 = from.Info2;
        to.Info3 = from.Info3;
        to.SkinVariant = from.SkinVariant;
        to.Hair = from.Hair;
        to.HairDye = from.HairDye;
        to.HideAccessories = new bool[from.HideAccessories.Length];

        for (int i = 0; i < from.HideAccessories.Length; i++)
        {
            to.HideAccessories[i] = from.HideAccessories[i];
        }

        to.HideMisc = from.HideMisc;
        to.Colors = new NetColor[from.Colors.Length];

        for (int i = 0; i < from.Colors.Length; i++)
        {
            to.Colors[i] = new NetColor(from.Colors[i].R, from.Colors[i].G, from.Colors[i].B);
        }

        to.QuestsCompleted = from.QuestsCompleted;
    }
}
