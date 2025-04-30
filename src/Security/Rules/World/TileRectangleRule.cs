using Amethyst.Network;
using Amethyst.Network.Managing;
using Amethyst.Network.Packets;

namespace Amethyst.Security.Rules.World;

public sealed class TileRectangleRule : ISecurityRule
{
    public string Name => "coresec_wldTileRect";

    public void Load(NetworkInstance net)
    {
        net.SecureIncoming[20].Add(OnTileRectangle);
    }

    private bool OnTileRectangle(in IncomingPacket packet)
    {
        // temporarily disabled
        return !packet.Player.HasPermission("security.tilerectangle");

        // var reader = packet.GetReader();

		// int x = reader.ReadInt16();
		// int y = reader.ReadInt16();
		// ushort width = reader.ReadByte();
		// ushort height = reader.ReadByte();
		// byte flags = reader.ReadByte();

        // if (width > 4 || height > 4)
        // {
        //     return true;
        // }

        // NetTile[,] tiles = new NetTile[width, height];

        // for (int i = 0; i < width; i++)
        // for (int j = 0; j < height; j++)
        //     tiles[i, j] = new NetTile(reader);

        //return false;
    }

    public void Unload(NetworkInstance net)
    {
        net.SecureIncoming[20].Remove(OnTileRectangle);
    }
}
