using Amethyst.Network.Managing;
using Amethyst.Network.Packets;
using Terraria;
using Terraria.ID;

namespace Amethyst.Security.Rules.Players;

public sealed class PlayerZoneRule : ISecurityRule
{
    public string Name => "coresec_playerZone";

    public void Load(NetworkInstance net)
    {
        net.SecureIncoming[35].Add(OnPlayerZone);
    }

    private bool OnPlayerZone(in IncomingPacket packet)
    {
        BinaryReader reader = packet.GetReader();

        reader.ReadByte();
        reader.ReadByte();

        BitsByte zone = reader.ReadByte();

        if (zone[0] || zone[1] || zone[2] || zone[3])
        {
            bool hasSolar = false, hasVortex = false, hasNebula = false, hasStardust = false;

			foreach (var npc in Main.npc)
            {
                switch (npc.netID)
                {
                    case NPCID.LunarTowerSolar:
                        hasSolar = true;
                        break;

                    case NPCID.LunarTowerStardust:
                        hasStardust = true;
                        break;

                    case NPCID.LunarTowerVortex:
                        hasVortex = true;
                        break;

                    case NPCID.LunarTowerNebula:
                        hasNebula = true;
                        break;
                }
            }

            if ((zone[0] && !hasSolar) || (zone[1] && !hasVortex) || (zone[2] && !hasNebula) || (zone[3] && !hasStardust))
            {
                return true;
            }
        }

        return false;
    }

    public void Unload(NetworkInstance net)
    {
        net.SecureIncoming[35].Remove(OnPlayerZone);
    }
}
