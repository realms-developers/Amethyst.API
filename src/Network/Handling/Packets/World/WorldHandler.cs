using Amethyst.Network.Handling.Base;
using Amethyst.Network.Handling.Packets.Handshake;
using Amethyst.Network.Packets;
using Amethyst.Server.Entities.Players;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;

namespace Amethyst.Network.Handling.Packets.World;

public sealed class WorldHandler : INetworkHandler
{
    public string Name => "net.amethyst.WorldHandler";

    public void Load()
    {
        NetworkManager.SetMainHandler<WorldLockSomething>(OnWorldLockSomething);
        NetworkManager.SetMainHandler<WorldMassWireOperation>(OnWorldMassWireOperation);
        NetworkManager.SetMainHandler<WorldPaintTile>(OnWorldPaintTile);
        NetworkManager.SetMainHandler<WorldPaintWall>(OnWorldPaintWall);
        NetworkManager.SetMainHandler<WorldTileInteract>(OnWorldTileInteract);
        NetworkManager.SetMainHandler<WorldTileRectangle>(OnWorldTileRectangle);
        NetworkManager.SetMainHandler<WorldToggleGemLock>(OnWorldToggleGemLock);
        NetworkManager.SetMainHandler<WorldWiringHitSwitch>(OnWorldWiringHitSwitch);
        NetworkManager.SetMainHandler<WorldAddLiquid>(OnWorldAddLiquid);
        NetworkManager.SetMainHandler<WorldDoorInteract>(OnWorldDoorInteract);
        NetworkManager.SetMainHandler<WorldPlaceObject>(OnWorldPlaceObject);
    }

    private void OnWorldPlaceObject(PlayerEntity plr, ref WorldPlaceObject packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected || packet.TileX <= 5 || packet.TileY <= 5 || packet.TileX >= Main.maxTilesX - 5 || packet.TileY >= Main.maxTilesY - 5)
            return;

		WorldGen.PlaceObject(packet.TileX, packet.TileY, packet.Type, mute: false, packet.Style, packet.Alternate, packet.Random, packet.Direction ? 1 : -1);
		NetMessage.SendObjectPlacement(plr.Index, packet.TileX, packet.TileY, packet.Type, packet.Style, packet.Alternate, packet.Random, packet.Direction ? 1 : -1);
    }

    private void OnWorldDoorInteract(PlayerEntity plr, ref WorldDoorInteract packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected || packet.TileX <= 5 || packet.TileY <= 5 || packet.TileX >= Main.maxTilesX - 5 || packet.TileY >= Main.maxTilesY - 5)
            return;

        int above = (packet.PlayerAbove != 0) ? 1 : -1;
        switch (packet.Action)
        {
            case 0:
                WorldGen.OpenDoor(packet.TileX, packet.TileY, above);
                break;
            case 1:
                WorldGen.CloseDoor(packet.TileX, packet.TileY, forced: true);
                break;
            case 2:
                WorldGen.ShiftTrapdoor(packet.TileX, packet.TileY, above == 1, 1);
                break;
            case 3:
                WorldGen.ShiftTrapdoor(packet.TileX, packet.TileY, above == 1, 0);
                break;
            case 4:
                WorldGen.ShiftTallGate(packet.TileX, packet.TileY, closing: false, forced: true);
                break;
            case 5:
                WorldGen.ShiftTallGate(packet.TileX, packet.TileY, closing: true, forced: true);
                break;
        }
        if (Main.netMode == 2)
        {
            NetMessage.TrySendData(19, -1, plr.Index, NetworkText.Empty, packet.Action, packet.TileX, packet.TileY, (above == 1) ? 1 : 0);
        }
    }

    private void OnWorldAddLiquid(PlayerEntity plr, ref WorldAddLiquid packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected || packet.TileX <= 5 || packet.TileY <= 5 || packet.TileX >= Main.maxTilesX - 5 || packet.TileY >= Main.maxTilesY - 5)
            return;

        if (Main.tile[packet.TileX, packet.TileY] == null)
        {
            Main.tile[packet.TileX, packet.TileY] = new Tile();
        }

        lock (Main.tile[packet.TileX, packet.TileY])
        {
            Main.tile[packet.TileX, packet.TileY].liquid = packet.LiquidCount;
            Main.tile[packet.TileX, packet.TileY].liquidType(packet.LiquidType);

            WorldGen.SquareTileFrame(packet.TileX, packet.TileY);
            if (packet.LiquidCount == 0)
            {
                NetMessage.SendData(48, -1, plr.Index, NetworkText.Empty, packet.TileX, packet.TileY);
            }
        }
    }

    private void OnWorldWiringHitSwitch(PlayerEntity plr, ref WorldWiringHitSwitch packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected || packet.TileX <= 5 || packet.TileY <= 5 || packet.TileX >= Main.maxTilesX - 5 || packet.TileY >= Main.maxTilesY - 5)
            return;

        Wiring.SetCurrentUser(plr.Index);
        Wiring.HitSwitch(packet.TileX, packet.TileY);
        Wiring.SetCurrentUser();

        NetMessage.TrySendData(59, -1, plr.Index, NetworkText.Empty, packet.TileX, packet.TileY);
    }

    private void OnWorldToggleGemLock(PlayerEntity plr, ref WorldToggleGemLock packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected || packet.TileX <= 5 || packet.TileY <= 5 || packet.TileX >= Main.maxTilesX - 5 || packet.TileY >= Main.maxTilesY - 5)
            return;

        WorldGen.ToggleGemLock(packet.TileX, packet.TileY, packet.IsOn);
    }

    private unsafe void OnWorldTileRectangle(PlayerEntity plr, ref WorldTileRectangle packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected || packet.StartX <= 5 || packet.StartY <= 5 || packet.StartX >= Main.maxTilesX - 5 || packet.StartY >= Main.maxTilesY - 5 ||
            packet.StartX + packet.SizeX <= 5 || packet.StartY + packet.SizeY <= 5 || packet.StartX + packet.SizeX >= Main.maxTilesX - 5 || packet.StartY + packet.SizeY >= Main.maxTilesY - 5)
            return;

        if (packet.SizeX > 5 || packet.SizeY > 5)
        {
            ignore = true;
            return;
        }

        for (int x = packet.StartX; x < packet.StartX + packet.SizeX; x++)
        {
            for (int y = packet.StartY; y < packet.StartY + packet.SizeY; y++)
            {
                if (Main.tile[x, y] == null)
                {
                    Main.tile[x, y] = new Tile();
                }
                Structures.NetTile netTile = packet.Tiles[x, y];
                TileData* tilePtr = Main.tile[x, y].ptr;

                tilePtr->active(netTile.Active);
                tilePtr->type = netTile.TileID;
                tilePtr->wall = netTile.Wall;
                tilePtr->liquid = netTile.Liquid;
                tilePtr->liquidType(netTile.LiquidID);
                tilePtr->wire(netTile.Wire);
                tilePtr->wire2(netTile.Wire2);
                tilePtr->wire3(netTile.Wire3);
                tilePtr->wire4(netTile.Wire4);
                tilePtr->halfBrick(netTile.HalfBrick);
                tilePtr->actuator(netTile.Actuator);
                tilePtr->inActive(netTile.Inactive);
                tilePtr->slope(netTile.Slope);
                tilePtr->color(netTile.TileColor);
                tilePtr->wallColor(netTile.WallColor);
                tilePtr->fullbrightBlock(netTile.Fullbright);
                tilePtr->fullbrightWall(netTile.FullbrightWall);
                tilePtr->invisibleBlock(netTile.Invisible);
                tilePtr->invisibleWall(netTile.InvisibleWall);
                tilePtr->frameX = netTile.FrameX;
                tilePtr->frameY = netTile.FrameY;
            }
        }
    }

    private void OnWorldTileInteract(PlayerEntity plr, ref WorldTileInteract packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected ||
            packet.TileX <= 5 || packet.TileY <= 5 ||
            packet.TileX >= Main.maxTilesX - 5 || packet.TileY >= Main.maxTilesY - 5)
            return;

        bool shouldSendTileSquare = false;
        bool fail = packet.Type == 1;

        switch (packet.Action)
        {
            case 0:
                WorldGen.KillTile(packet.TileX, packet.TileY, fail);
                if (Main.netMode == 1 && packet.Flags != 1)
                    HitTile.ClearAllTilesAtThisLocation(packet.TileX, packet.TileY);
                break;
            case 1:
                {
                    bool forced = !WorldGen.CheckTileBreakability2_ShouldTileSurvive(packet.TileX, packet.TileY);
                    if (!forced)
                        shouldSendTileSquare = true;
                    WorldGen.PlaceTile(packet.TileX, packet.TileY, packet.Type, mute: false, forced, -1, packet.Flags);
                    break;
                }
            case 2:
                WorldGen.KillWall(packet.TileX, packet.TileY, fail);
                break;
            case 3:
                WorldGen.PlaceWall(packet.TileX, packet.TileY, packet.Type);
                break;
            case 4:
                WorldGen.KillTile(packet.TileX, packet.TileY, fail, effectOnly: false, noItem: true);
                break;
            case 5:
                WorldGen.PlaceWire(packet.TileX, packet.TileY);
                break;
            case 6:
                WorldGen.KillWire(packet.TileX, packet.TileY);
                break;
            case 7:
                WorldGen.PoundTile(packet.TileX, packet.TileY);
                break;
            case 8:
                WorldGen.PlaceActuator(packet.TileX, packet.TileY);
                break;
            case 9:
                WorldGen.KillActuator(packet.TileX, packet.TileY);
                break;
            case 10:
                WorldGen.PlaceWire2(packet.TileX, packet.TileY);
                break;
            case 11:
                WorldGen.KillWire2(packet.TileX, packet.TileY);
                break;
            case 12:
                WorldGen.PlaceWire3(packet.TileX, packet.TileY);
                break;
            case 13:
                WorldGen.KillWire3(packet.TileX, packet.TileY);
                break;
            case 14:
                WorldGen.SlopeTile(packet.TileX, packet.TileY, packet.Type);
                break;
            case 15:
                Minecart.FrameTrack(packet.TileX, packet.TileY, pound: true);
                break;
            case 16:
                WorldGen.PlaceWire4(packet.TileX, packet.TileY);
                break;
            case 17:
                WorldGen.KillWire4(packet.TileX, packet.TileY);
                break;
            case 18:
                Wiring.SetCurrentUser(plr.Index);
                Wiring.PokeLogicGate(packet.TileX, packet.TileY);
                Wiring.SetCurrentUser();
                return;
            case 19:
                Wiring.SetCurrentUser(plr.Index);
                Wiring.Actuate(packet.TileX, packet.TileY);
                Wiring.SetCurrentUser();
                return;
            case 20:
                int prevType = Main.tile[packet.TileX, packet.TileY].type;
                WorldGen.KillTile(packet.TileX, packet.TileY, fail);
                packet.Type = (short)((Main.tile[packet.TileX, packet.TileY].active() && Main.tile[packet.TileX, packet.TileY].type == prevType) ? 1 : 0);

                NetMessage.TrySendData(17, -1, -1, NetworkText.Empty, packet.Action, packet.TileX, packet.TileY, packet.Type, packet.Flags);
                return;
            case 21:
                WorldGen.ReplaceTile(packet.TileX, packet.TileY, (ushort)packet.Type, packet.Flags);
                break;
            case 22:
                WorldGen.ReplaceWall(packet.TileX, packet.TileY, (ushort)packet.Type);
                break;
            case 23:
                WorldGen.SlopeTile(packet.TileX, packet.TileY, packet.Type);
                WorldGen.PoundTile(packet.TileX, packet.TileY);
                break;
        }

        if (shouldSendTileSquare)
        {
            NetMessage.SendTileSquare(-1, packet.TileX, packet.TileY, 5);
        }
        else if ((packet.Action != 1 && packet.Action != 21) || !TileID.Sets.Falling[packet.Type] || Main.tile[packet.TileX, packet.TileY].active())
        {
            PlayerUtils.BroadcastPacketBytes(WorldTileInteractPacket.Serialize(packet));
        }
    }

    private void OnWorldPaintWall(PlayerEntity plr, ref WorldPaintWall packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected || packet.TileX <= 5 || packet.TileY <= 5 || packet.TileX >= Main.maxTilesX - 5 || packet.TileY >= Main.maxTilesY - 5)
            return;

        Tile tile = Main.tile[packet.TileX, packet.TileY];
        if (tile == null)
        {
            return;
        }

        if (packet.IsCoating == 1)
        {
            WorldGen.paintCoatWall(packet.TileX, packet.TileY, packet.PaintType);
        }
        else
        {
            WorldGen.paintWall(packet.TileX, packet.TileY, packet.PaintType);
        }

        PlayerUtils.BroadcastPacketBytes(WorldPaintWallPacket.Serialize(packet));
    }

    private void OnWorldPaintTile(PlayerEntity plr, ref WorldPaintTile packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected || packet.TileX <= 5 || packet.TileY <= 5 || packet.TileX >= Main.maxTilesX - 5 || packet.TileY >= Main.maxTilesY - 5)
            return;

        Tile tile = Main.tile[packet.TileX, packet.TileY];
        if (tile == null)
        {
            return;
        }

        if (packet.IsCoating == 1)
        {
            WorldGen.paintCoatTile(packet.TileX, packet.TileY, packet.PaintType);
        }
        else
        {
            WorldGen.paintTile(packet.TileX, packet.TileY, packet.PaintType);
        }

        PlayerUtils.BroadcastPacketBytes(WorldPaintTilePacket.Serialize(packet));
    }

    private void OnWorldMassWireOperation(PlayerEntity plr, ref WorldMassWireOperation packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected || packet.StartX <= 5 || packet.StartY <= 5 || packet.StartX >= Main.maxTilesX - 5 || packet.StartY >= Main.maxTilesY - 5 ||
            packet.EndX <= 5 || packet.EndY <= 5 || packet.EndX >= Main.maxTilesX - 5 || packet.EndY >= Main.maxTilesY - 5)
            return;

        WiresUI.Settings.MultiToolMode toolMode2 = WiresUI.Settings.ToolMode;
        WiresUI.Settings.ToolMode = (WiresUI.Settings.MultiToolMode)packet.ToolMode;
        Wiring.MassWireOperation(new(packet.StartX, packet.StartY), new(packet.EndX, packet.EndY), Main.player[plr.Index]);
        WiresUI.Settings.ToolMode = toolMode2;
    }

    private void OnWorldLockSomething(PlayerEntity plr, ref WorldLockSomething packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected || packet.TileX <= 5 || packet.TileY <= 5 || packet.TileX >= Main.maxTilesX - 5 || packet.TileY >= Main.maxTilesY - 5)
            return;

        switch (packet.TargetType)
        {
            case 1:
                Chest.Unlock(packet.TileX, packet.TileY);
                break;
            case 2:
                WorldGen.UnlockDoor(packet.TileX, packet.TileY);
                break;
            case 3:
                Chest.Lock(packet.TileX, packet.TileY);
                break;
            default:
                return;
        }

        PlayerUtils.BroadcastPacketBytes(WorldLockSomethingPacket.Serialize(packet));
        NetMessage.SendTileSquare(-1, packet.TileX, packet.TileY, 2);
    }

    public void Unload()
    {
        NetworkManager.SetMainHandler<WorldPlaceObject>(null);
        NetworkManager.SetMainHandler<WorldLockSomething>(null);
        NetworkManager.SetMainHandler<WorldMassWireOperation>(null);
        NetworkManager.SetMainHandler<WorldPaintTile>(null);
        NetworkManager.SetMainHandler<WorldPaintWall>(null);
        NetworkManager.SetMainHandler<WorldTileInteract>(null);
        NetworkManager.SetMainHandler<WorldTileRectangle>(null);
        NetworkManager.SetMainHandler<WorldToggleGemLock>(null);
        NetworkManager.SetMainHandler<WorldWiringHitSwitch>(null);
        NetworkManager.SetMainHandler<WorldAddLiquid>(null);
        NetworkManager.SetMainHandler<WorldDoorInteract>(null);
    }
}
