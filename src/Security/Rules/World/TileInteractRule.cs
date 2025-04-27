using Amethyst.Network;
using Amethyst.Network.Managing;
using Amethyst.Network.Packets;
using Terraria;

namespace Amethyst.Security.Rules.World;

public sealed class TileInteractRule : ISecurityRule
{
    public string Name => "coresec_wldTileInteract";

    public void Load(NetworkInstance net)
    {
        net.SecureIncoming[17].Add(OnTileInteract);
    }

    private bool OnTileInteract(in IncomingPacket packet)
    {
        var reader = packet.GetReader();

        TileInteractType interactType = reader.Read<TileInteractType>();
		int x = reader.ReadInt16();
		int y = reader.ReadInt16();
		short type = reader.ReadInt16();
		int style = reader.ReadByte();

        if (!WorldGen.InWorld(x, y, 16))
        {
            return true;
        }

        if (interactType > TileInteractType.SlopeAndPound)
        {
            return true;
        }

        switch (interactType)
        {
            case TileInteractType.KillTile:
            case TileInteractType.KillTileV3:
            case TileInteractType.KillTileNoItem:

                if (!packet.Player.Utils.InCenteredCube(x, y, SecurityManager.Configuration.KillTileRange!.Value))
                {
                    return true;
                }

                return false;

            case TileInteractType.PlaceTile:



                return false;
        }
    }

    public void Unload(NetworkInstance net)
    {
        net.SecureIncoming[17].Remove(OnTileInteract);
    }
}
