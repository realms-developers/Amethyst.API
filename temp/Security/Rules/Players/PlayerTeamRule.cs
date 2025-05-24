using Amethyst.Network.Managing;
using Amethyst.Network.Packets;

namespace Amethyst.Security.Rules.Players;

public sealed class PlayerTeamRule : ISecurityRule
{
    public string Name => "coresec_playerTeam";

    public void Load(NetworkInstance net)
    {
        net.SecureIncoming[30].Add(OnPlayerSetTeam);
    }

    private bool OnPlayerSetTeam(in IncomingPacket packet)
    {
        if (packet.Player.Jail.IsJailed || SecurityManager.Configuration.DisableSwitchingTeam || packet.Player._securityThreshold.Fire(11))
        {
            packet.Player.Utils.SetTeam(0, true);
            return true;
        }

        return false;
    }

    public void Unload(NetworkInstance net)
    {
        net.SecureIncoming[30].Remove(OnPlayerSetTeam);
    }
}
