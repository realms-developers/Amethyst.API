using Amethyst.Network.Managing;
using Amethyst.Network.Packets;
using Amethyst.Players;
using Terraria;
using Terraria.ID;

namespace Amethyst.Security.Rules.Players;

public sealed class PlayerSlotRule : ISecurityRule
{
    public string Name => "coresec_playerSlot";

    public void Load(NetworkInstance net)
    {
        net.SecureIncoming[4].Add(OnPlayerSlot);
    }

    private bool OnPlayerSlot(in IncomingPacket packet)
    {
        var reader = packet.GetReader();

        reader.ReadByte();

        int slotId = reader.ReadInt16();
		int stack = reader.ReadInt16();
		int prefix = reader.ReadByte();
		int type = reader.ReadInt16();

        if (slotId < 0 || slotId >= 350)
        {
            return true;
        }

        if (type < -1 || type >= ItemID.Count)
        {
            return true;
        }

        if (SecurityManager.Configuration.PreventStackCheat)
        {
            Item item = new Item();
            item.SetDefaults(type);

            if (stack > item.maxStack)
            {
                if (SecurityManager.Configuration.NotifyModerators == true)
                {
                    foreach (NetPlayer player in PlayerManager.Tracker.Capable)
                    {
                        if (player.HasPermission(SecurityManager.ModeratorPermission))
                        {
                            player.ReplyError("security.moderatorNotify.stackCheat", player.Name, item.Name, stack, item.maxStack);
                        }
                    }
                }

                packet.Player.Kick("security.stackCheat");

                return true;
            }
        }

        return false;
    }

    public void Unload(NetworkInstance net)
    {
        net.SecureIncoming[4].Remove(OnPlayerSlot);
    }
}
