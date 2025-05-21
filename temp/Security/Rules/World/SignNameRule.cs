using Amethyst.Network.Managing;
using Amethyst.Network.Packets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Amethyst.Security.Rules.World;

public sealed class SignNameRule : ISecurityRule
{
    public string Name => "coresec_wldSignName";

    public void Load(NetworkInstance net)
    {
        net.SecureIncoming[47].Add(OnSignName);
    }

    private bool OnSignName(in IncomingPacket packet)
    {
        BinaryReader reader = packet.GetReader();

        if (packet.Player.Jail.IsJailed)
        {
            return true;
        }

        if (packet.Player._securityThreshold.Fire(8))
        {
            packet.Player.Jail.SetTemp(TimeSpan.FromSeconds(5));
            return true;
        }

        byte interaction = reader.ReadByte();
        int x = reader.ReadInt16();
        int y = reader.ReadInt16();
        int style = reader.ReadInt16();
        int indexToRemove = reader.ReadInt16();

        if (style != packet.Player.Utils.HeldItem.type || packet.Player.Utils.InCenteredCube(x, y, 16))
        {
            packet.Player.Jail.SetTemp(TimeSpan.FromSeconds(3));
            return true;
        }

        Tile tile = Main.tile[x, y];

        Tile bottomTile = Main.tile[x, y + 1];
        Tile bottomTile2 = Main.tile[x + 1, y + 1];

        if (bottomTile.active() == false || bottomTile.type == TileID.MetalBars || bottomTile.type == TileID.Teleporter ||
            bottomTile2.active() == false || bottomTile2.type == TileID.MetalBars || bottomTile2.type == TileID.Teleporter)
        {
            if (!ValidationExtensions.IsInTerrariaWorld(new Point(x, y)))
            {
                return true;
            }
        }

        if (interaction != 0 && interaction != 4 && tile.type != TileID.Containers && tile.type != TileID.Containers2 && tile.type != TileID.Dressers)
        {
            packet.Player.Jail.SetTemp(TimeSpan.FromSeconds(5));
            return true;
        }

        return false;
    }

    public void Unload(NetworkInstance net)
    {
        net.SecureIncoming[47].Remove(OnSignName);
    }
}
