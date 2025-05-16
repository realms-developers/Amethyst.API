using Amethyst.Core;
using Amethyst.Network;
using Amethyst.Network.Managing;
using Amethyst.Network.Packets;
using Amethyst.Players;

namespace Amethyst.Security.Rules.Players;

public sealed class PlayerManaRule : ISecurityRule
{
    public string Name => "coresec_playerMana";

    public void Load(NetworkInstance net)
    {
        net.SecureIncoming[42].Add(OnPlayerMana);
    }

    private bool OnPlayerMana(in IncomingPacket packet)
    {
        BinaryReader reader = packet.GetReader();

        reader.ReadByte();

        short current = reader.ReadInt16();
        short max = reader.ReadInt16();

        if (current > SecurityManager.Configuration.MaxAllowedMana * 2 ||
            max > SecurityManager.Configuration.MaxAllowedMana)
        {
            AmethystLog.Security.Debug(Name, $"security.invalidMana => {packet.Player.Name} [Allowed Max: {SecurityManager.Configuration.MaxAllowedMana}; Current: {current}: Max: {max}]");
            packet.Player.Kick("security.invalidMana");
            return true;
        }

        if (PlayerManager.IsSSCEnabled && packet.Player.Jail.IsJailed)
        {
            byte[] buffer = new PacketWriter().SetType(16)
                .PackByte((byte)packet.Player.Index)
                .PackInt16(packet.Player._lastMana)
                .PackInt16(packet.Player._lastMaxMana)
                .BuildPacket();

            PlayerUtilities.BroadcastPacket(buffer);
            return true;
        }

        packet.Player._lastMana = current;
        packet.Player._lastMaxMana = current;

        return false;
    }

    public void Unload(NetworkInstance net)
    {
        net.SecureIncoming[42].Remove(OnPlayerMana);
    }
}
