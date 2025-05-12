using Amethyst.Core;
using Amethyst.Items;
using Amethyst.Network;
using Amethyst.Network.Managing;
using Amethyst.Network.Packets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Amethyst.Security.Rules.Items;

public sealed class ItemUpdateRule : ISecurityRule
{
    public string Name => "coresec_itemUpdate";

    public void Load(NetworkInstance net)
    {
        net.SecureIncoming[21].Add(OnUpdateItem);
        net.SecureIncoming[90].Add(OnUpdateItem);
        net.SecureIncoming[145].Add(OnUpdateItem);
        net.SecureIncoming[148].Add(OnUpdateItem);
    }

    private bool OnUpdateItem(in IncomingPacket packet)
    {
        BinaryReader reader = packet.GetReader();

        short index = reader.ReadInt16();
        Vector2 position = NetExtensions.ReadVector2(reader);
        Vector2 velocity = NetExtensions.ReadVector2(reader);
        short stack = reader.ReadInt16();
        byte prefix = reader.ReadByte();
        byte _ = reader.ReadByte();
        short type = reader.ReadInt16();

        if (position.IsBadVector2() || !position.IsInTerrariaWorld() || velocity.IsBadVector2())
        {
            AmethystLog.Security.Debug(Name, $"security.badVec2 (item) => {packet.Player.Name} item {index} [Bad: {position.IsBadVector2()}; Bad Velocity: {velocity.IsBadVector2()} InWorld: {position.IsInTerrariaWorld()}; X: {position.X / 16}; Y: {position.Y / 16}]");
            packet.Player.Kick("security.badVec2");
            return true;
        }

        AmethystLog.Security.Debug(Name, $"security.itemUpdate => {packet.Player.Name} item drop [Index: {index}, Type: {type}]");

        if (index < 0 || index > Main.item.Length)
        {
            packet.Player.Kick("security.invalidItemIndex");
            AmethystLog.Security.Debug(Name, $"security.itemUpdate => {packet.Player.Name} invalid item index: {index}");
            return true;
        }

        if (type == 0) // pickup
        {
            if (index == 400)
            {
                AmethystLog.Security.Debug(Name, $"security.itemUpdate => {packet.Player.Name} stupid pickup; item index is 400");
                return true;
            }

            AmethystLog.Security.Debug(Name, $"security.itemUpdate => {packet.Player.Name} pickup {Main.item[index].type} => {type}");
            return false;
        }

        if (type < 0 || type >= ItemID.Count ||
            prefix >= PrefixID.Count ||
            stack <= 0 || stack > 9999)
        {
            AmethystLog.Security.Debug(Name, $"security.itemUpdate => {packet.Player.Name} invalid item data [Type: {type}; Prefix: {prefix}, Stack: {stack}]");
            return true;
        }

        if (index != 400 && Main.item[index].type > 0 && Main.item[index].stack > 0)
        {
            AmethystLog.Security.Debug(Name, $"security.itemUpdate => {packet.Player.Name} tried to replace existing item");
            return true;
        }

        if (SecurityManager.ItemBans.Contains(type))
        {
            Main.item[index] = new Item();
            NetMessage.SendData(21, packet.Sender, -1, NetworkText.Empty, index);

            packet.Player.ReplyError("security.itemBanned", type);
            return true;
        }

        Item item = new();
        item.SetDefaults(type);
        if (stack > item.maxStack)
        {
            AmethystLog.Security.Debug(Name, $"security.itemUpdate => {packet.Player.Name} invalid item stack [Requested: {stack}, Max: {item.maxStack}]");
            return true;
        }

        if (!SecurityManager.Configuration.DisableItemDropThreshold && packet.Player._securityThreshold.Fire(6)) // drop threshold
        {
            if (SecurityManager.Configuration.ReturnDroppedItemInThreshold == true)
            {
                ItemManager.LocalCreateItem(packet.Player, index, type, stack, prefix);
            }
            return true;
        }

        return false;
    }

    public void Unload(NetworkInstance net)
    {
        net.SecureIncoming[21].Remove(OnUpdateItem);
        net.SecureIncoming[90].Remove(OnUpdateItem);
        net.SecureIncoming[145].Remove(OnUpdateItem);
        net.SecureIncoming[148].Remove(OnUpdateItem);
    }
}
