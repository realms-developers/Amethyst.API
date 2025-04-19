using Amethyst.Network.Managing;
using Amethyst.Network.Packets;
using Microsoft.Xna.Framework;

namespace Amethyst.Security.Rules;

public class BasicRule : ISecurityRule
{
    public string Name => "basic";

    public void Load(NetworkInstance net)
    {
        net.SecureIncoming[17].Add(OnSeventeenPacket);
    }

    private bool OnSeventeenPacket(in IncomingPacket packet)
    {
        packet.Player.SendMessage("sex, amogus=" + packet.Player.HasPermission("amogus permission"), Color.White);

        return packet.Player.HasPermission("amogus permission");
    }

    public void Unload(NetworkInstance net)
    {
        net.SecureIncoming[17].Remove(OnSeventeenPacket);
    }
}
