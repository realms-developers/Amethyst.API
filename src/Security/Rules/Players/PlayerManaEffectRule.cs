using Amethyst.Network.Managing;
using Amethyst.Network.Packets;

namespace Amethyst.Security.Rules.Players;

public sealed class PlayerManaEffectRule : ISecurityRule
{
    public string Name => "coresec_playerManaEffect";

    public void Load(NetworkInstance net)
    {
        net.SecureIncoming[43].Add(OnPlayerManaEffect);
    }

    private bool OnPlayerManaEffect(in IncomingPacket packet)
    {
        BinaryReader reader = packet.GetReader();

        reader.ReadByte();

        short count = reader.ReadInt16();

        if (SecurityManager.Configuration.DisableManaHealCombatText || packet.Player._securityThreshold.Fire(10))
        {
            return true;
        }

        return false;
    }

    public void Unload(NetworkInstance net)
    {
        net.SecureIncoming[43].Remove(OnPlayerManaEffect);
    }
}
