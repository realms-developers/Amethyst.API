using Amethyst.Network;
using Amethyst.Network.Managing;
using Amethyst.Network.Packets;
using Terraria;

namespace Amethyst.Security.Rules.Players;

public sealed class PlayerPvPRule : ISecurityRule
{
    public string Name => "coresec_playerPvP";

    public void Load(NetworkInstance net)
    {
        net.SecureIncoming[30].Add(OnPlayerSetPvP);
    }

    private bool OnPlayerSetPvP(in IncomingPacket packet)
    {
        if (packet.Player.Jail.IsJailed || SecurityManager.Configuration.DisableSwitchingPvP)
        {
            packet.Player.Utils.SetPvP(packet.Player.Utils.InPvP, true);
            return true;
        }

        return false;
    }

    public void Unload(NetworkInstance net)
    {
        net.SecureIncoming[30].Remove(OnPlayerSetPvP);
    }
}
