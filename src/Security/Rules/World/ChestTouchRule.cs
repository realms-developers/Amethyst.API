using Amethyst.Network.Managing;
using Amethyst.Network.Packets;

namespace Amethyst.Security.Rules.World;

public sealed class ChestTouchRule : ISecurityRule
{
    public string Name => "coresec_wldChestTouch";

    public void Load(NetworkInstance net)
    {
        net.SecureIncoming[31].Add(OnChestTouch);
    }

    private bool OnChestTouch(in IncomingPacket packet)
    {
        BinaryReader reader = packet.GetReader();

        if (packet.Player.Jail.IsJailed)
        {
            return true;
        }

        int x = reader.ReadInt16();
        int y = reader.ReadInt16();

        if (!packet.Player.Utils.InCenteredCube(x, y, 32))
        {
            return true;
        }

        return false;
    }

    public void Unload(NetworkInstance net)
    {
        net.SecureIncoming[31].Remove(OnChestTouch);
    }
}
