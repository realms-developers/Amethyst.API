using Amethyst.Gameplay.Players;
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
        BinaryReader reader = packet.GetReader();

        TileInteractType interactType = reader.Read<TileInteractType>();
        int x = reader.ReadInt16();
        int y = reader.ReadInt16();
        short type = reader.ReadInt16();
        int _ = reader.ReadByte();

        if (!WorldGen.InWorld(x, y, 16))
        {
            return true;
        }

        if (interactType > TileInteractType.SlopeAndPound)
        {
            return true;
        }

        NetPlayer player = packet.Player;
        Tile tile = Main.tile[x, y];

        if (packet.Player.Jail.IsJailed)
        {
            player.Utils.SendRectangle(x, y, 2);
            return true;
        }

        if (packet.Player.IsHeldItemBanned)
        {
            player.Utils.SendRectangle(x, y, 2);

            packet.Player.ReplyError("security.itemBanned", packet.Player.Utils.HeldItem.type);
            return true;
        }

        bool CanPlaceTile()
        {
            if (SecurityManager.TileBans.Contains(type))
            {
                if (SecurityManager.Configuration.AllowedMessages.Contains("security_tile_bans") &&
                    player.CanNotify("security_tile_bans", new TimeSpan(0, 0, 3)))
                {
                    player.ReplyError("security.tileBannedToCreate", type);
                }

                return true;
            }

            return false;
        }
        ;

        bool CanKillTile()
        {
            if (tile.active() && SecurityManager.TileSafety.Contains(tile.type))
            {
                if (SecurityManager.Configuration.AllowedMessages.Contains("security_tile_safety") &&
                    player.CanNotify("security_tile_safety", new TimeSpan(0, 0, 3)))
                {
                    player.ReplyError("security.tileBannedToDestroy", tile.type);
                }

                return true;
            }

            return false;
        }
        ;

        bool CanPlaceWall()
        {
            if (type != 0 && SecurityManager.WallBans.Contains(type))
            {
                if (SecurityManager.Configuration.AllowedMessages.Contains("security_wall_bans") &&
                    player.CanNotify("security_wall_bans", new TimeSpan(0, 0, 3)))
                {
                    player.ReplyError("security.wallBannedToCreate", type);
                }

                return true;
            }

            return false;
        }
        ;

        bool CanKillWall()
        {
            if (tile.wall != 0 && SecurityManager.WallSafety.Contains(tile.wall))
            {
                if (SecurityManager.Configuration.AllowedMessages.Contains("security_wall_safety") &&
                    player.CanNotify("security_wall_safety", new TimeSpan(0, 0, 3)))
                {
                    player.ReplyError("security.wallBannedToDestroy", tile.wall);
                }

                return true;
            }

            return false;
        }
        ;

        bool NetworkResetTilesIf(bool value, bool threshold)
        {
            if (value)
            {
                player.Utils.SendRectangle(x, y, 2);
            }

            if (threshold)
            {
                player.Jail.SetTemp(TimeSpan.FromSeconds(5));
            }

            return value;
        }

        switch (interactType)
        {
            #region Tiles
            case TileInteractType.KillTile:
            case TileInteractType.KillTileV3:
            case TileInteractType.KillTileNoItem:

                return NetworkResetTilesIf(!player.Utils.InCenteredCube(x, y, SecurityManager.Configuration.KillTileRange!.Value) ||
                        CanKillTile(), player._securityThreshold.Fire(0));

            case TileInteractType.PlaceTile:

                return NetworkResetTilesIf(!player.Utils.InCenteredCube(x, y, SecurityManager.Configuration.PlaceTileRange!.Value) ||
                        CanPlaceTile(), player._securityThreshold.Fire(1));

            case TileInteractType.ReplaceTile:

                return NetworkResetTilesIf(!player.Utils.InCenteredCube(x, y, SecurityManager.Configuration.ReplaceTileRange!.Value) ||
                        CanKillTile() || CanPlaceTile(), player._securityThreshold.Fire(2));
            #endregion

            #region Walls

            case TileInteractType.KillWall:

                return NetworkResetTilesIf(!player.Utils.InCenteredCube(x, y, SecurityManager.Configuration.KillWallRange!.Value) ||
                        CanKillWall(), player._securityThreshold.Fire(3));

            case TileInteractType.PlaceWall:

                return NetworkResetTilesIf(!player.Utils.InCenteredCube(x, y, SecurityManager.Configuration.PlaceWallRange!.Value) ||
                        CanPlaceWall(), player._securityThreshold.Fire(4));

            case TileInteractType.ReplaceWall:

                return NetworkResetTilesIf(!player.Utils.InCenteredCube(x, y, SecurityManager.Configuration.ReplaceWallRange!.Value) ||
                        CanKillWall() || CanPlaceWall(), player._securityThreshold.Fire(5));

            #endregion

            default: return false;
        }
    }

    public void Unload(NetworkInstance net)
    {
        net.SecureIncoming[17].Remove(OnTileInteract);
    }
}
