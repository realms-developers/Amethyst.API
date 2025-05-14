using Amethyst.Network.Managing;
using Amethyst.Network.Packets;

namespace Amethyst.Security.Rules.NPCs;

public sealed class NPCUpdateRule : ISecurityRule
{
    public string Name => "coresec_npcUpdate";

    public void Load(NetworkInstance net)
    {
        net.SecureIncoming[21].Add(OnUpdateNPC);
    }

    private bool OnUpdateNPC(in IncomingPacket packet)
    {
        return packet.Player.Jail.IsJailed;
    }

    public void Unload(NetworkInstance net)
    {
        net.SecureIncoming[21].Remove(OnUpdateNPC);
    }
}

