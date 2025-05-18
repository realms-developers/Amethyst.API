using Amethyst.Gameplay.Players;
using Amethyst.Network;
using Amethyst.Network.Managing;
using Amethyst.Network.Packets;

namespace Amethyst.Security.Rules.Players;

public sealed class PlayerLifeRule : ISecurityRule
{
    public string Name => "coresec_playerLife";

    public void Load(NetworkInstance net)
    {
        net.SecureIncoming[16].Add(OnPlayerLife);
    }

    private bool OnPlayerLife(in IncomingPacket packet)
    {
        BinaryReader reader = packet.GetReader();

        reader.ReadByte();

        short current = reader.ReadInt16();
        short max = reader.ReadInt16();

        if (current > SecurityManager.Configuration.MaxAllowedLife * 1.2 ||
            max > SecurityManager.Configuration.MaxAllowedLife)
        {
            AmethystLog.Security.Debug(Name, $"security.invalidLife => {packet.Player.Name} [Allowed Max: {SecurityManager.Configuration.MaxAllowedLife}; Current: {current}: Max: {max}]");
            packet.Player.Kick("security.invalidLife");
            return true;
        }

        if (PlayerManager.IsSSCEnabled && packet.Player.Jail.IsJailed)
        {
            byte[] buffer = new PacketWriter().SetType(16)
                .PackByte((byte)packet.Player.Index)
                .PackInt16(packet.Player._lastLife)
                .PackInt16(packet.Player._lastMaxLife)
                .BuildPacket();

            PlayerUtilities.BroadcastPacket(buffer);
            return true;
        }

        packet.Player._lastLife = current;
        packet.Player._lastMaxLife = current;

        return false;
    }

    public void Unload(NetworkInstance net)
    {
        net.SecureIncoming[16].Remove(OnPlayerLife);
    }
}
