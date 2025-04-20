using Amethyst.Network.Managing;
using Amethyst.Network.Packets;
using Terraria;

namespace Amethyst.Security.Rules.Players;

public sealed class PlayerSpawnRule : ISecurityRule
{
    public string Name => "coresec_playerSpawn";

    public void Load(NetworkInstance net)
    {
        net.SecureIncoming[12].Add(OnPlayerSpawn);
    }

    private bool OnPlayerSpawn(in IncomingPacket packet)
    {
        var reader = packet.GetReader();

		reader.ReadByte();

		short spawnX = reader.ReadInt16();
		short spawnY = reader.ReadInt16();
	    int respawnTimer = reader.ReadInt32();

        if (!WorldGen.InWorld(spawnX, spawnY, 16) || respawnTimer > 60)
        {
            return true;
        }

        return false;
    }

    public void Unload(NetworkInstance net)
    {
        net.SecureIncoming[12].Remove(OnPlayerSpawn);
    }
}
