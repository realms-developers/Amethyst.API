using Amethyst.Network.Managing;
using Amethyst.Network.Packets;

namespace Amethyst.Security.Rules.Players;

public sealed class PlayerHealEffect : ISecurityRule
{
    public string Name => "coresec_playerHealEffect";

    public void Load(NetworkInstance net)
    {
        net.SecureIncoming[35].Add(OnPlayerHealEffect);
    }

    private bool OnPlayerHealEffect(in IncomingPacket packet)
    {
        BinaryReader reader = packet.GetReader();

        reader.ReadByte();

        short count = reader.ReadInt16();

        if (SecurityManager.Configuration.DisableHealCombatText || packet.Player._securityThreshold.Fire(9))
        {
            return true;
        }

        return false;
    }

    public void Unload(NetworkInstance net)
    {
        net.SecureIncoming[35].Remove(OnPlayerHealEffect);
    }
}
