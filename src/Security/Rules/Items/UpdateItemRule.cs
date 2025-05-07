using Amethyst.Core;
using Amethyst.Items;
using Amethyst.Network;
using Amethyst.Network.Managing;
using Amethyst.Network.Packets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Amethyst.Security.Rules.World;

public sealed class UpdateItemRule : ISecurityRule
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
        var reader = packet.GetReader();

        short index = reader.ReadInt16();
	    Vector2 position = NetExtensions.ReadVector2(reader);
		Vector2 velocity = NetExtensions.ReadVector2(reader);
		short stack = reader.ReadInt16();
		byte prefix = reader.ReadByte();
		byte ownIgnore = reader.ReadByte();
		short type = reader.ReadInt16();

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
            return true;
        }

        if (type < 0 || type >= ItemID.Count ||
            prefix >= PrefixID.Count ||
            stack <= 0 || stack > 9999)
        {
            AmethystLog.Security.Debug(Name, $"security.itemUpdate => {packet.Player.Name} invalid item data [Type: {type}; Prefix: {prefix}, Stack: {stack}]");
            return true;
        }

        if (index != 400 && Main.item[index].type != 0)
        {
            AmethystLog.Security.Debug(Name, $"security.itemUpdate => {packet.Player.Name} tried to replace existing item");
            return true;
        }

        Item item = new Item();
        item.SetDefaults(type);
        if (stack > item.maxStack)
        {
            AmethystLog.Security.Debug(Name, $"security.itemUpdate => {packet.Player.Name} invalid item stack [Requested: {stack}, Max: {item.maxStack}]");
            return true;
        }

        if (packet.Player._securityThreshold.Fire(6)) // drop threshold
        {
            return true;
        }

        // code for returning item.
        // ItemManager.LocalCreateItem(packet.Player, index, type, stack, prefix);

        // TODO: drop threshold settings, returning item by threshold

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
