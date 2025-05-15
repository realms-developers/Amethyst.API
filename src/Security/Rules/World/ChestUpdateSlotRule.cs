using Amethyst.Network;
using Amethyst.Network.Managing;
using Amethyst.Network.Packets;
using Amethyst.Players;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Amethyst.Security.Rules.World;

public sealed class ChestUpdateSlotRule : ISecurityRule
{
    public string Name => "coresec_wldChestUpdateSlot";

    public void Load(NetworkInstance net)
    {
        net.SecureIncoming[32].Add(OnChestUpdateSlot);
    }

    private bool OnChestUpdateSlot(in IncomingPacket packet)
    {
        BinaryReader reader = packet.GetReader();

        if (packet.Player.Jail.IsJailed)
        {
            return true;
        }

        int index = reader.ReadInt16();
        int slot = reader.ReadByte();
        int stack = reader.ReadInt16();
        int prefix = reader.ReadByte();
        int type = reader.ReadInt16();

        if (index < 0 || index >= 8000)
        {
            return true;
        }

        Chest? chest = Main.chest[index];

        if (chest == null || chest.item == null || slot >= chest.item.Length ||
            packet.Player.TPlayer.chest != index || !packet.Player.Utils.InCenteredCube(chest.x, chest.y, 32))
        {
            return true;
        }

        Item item = new Item();
        item.netDefaults(type);

        if (stack > item.maxStack || prefix >= PrefixID.Count)
        {
            chest.item[slot] ??= new Item();
            NetMessage.SendData(32, packet.Player.Index, -1, NetworkText.Empty, index, slot);
            return true;
        }

        if (SecurityManager.ItemBans.Contains(type))
        {
            chest.item[slot] ??= new Item();
            NetMessage.SendData(32, packet.Player.Index, -1, NetworkText.Empty, index, slot);

            packet.Player.ReplyError("security.itemBanned", type);
            return true;
        }

        return false;
    }

    public void Unload(NetworkInstance net)
    {
        net.SecureIncoming[32].Remove(OnChestUpdateSlot);
    }
}
