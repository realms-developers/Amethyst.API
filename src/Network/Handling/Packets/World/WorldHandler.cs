using Amethyst.Network.Handling.Base;
using Amethyst.Network.Handling.Packets.Handshake;
using Amethyst.Network.Packets;
using Amethyst.Server.Entities.Players;
using Terraria;
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
    }

    private void OnWorldDoorInteract(PlayerEntity plr, ref WorldDoorInteract packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected || packet.TileX <= 5 || packet.TileY <= 5 || packet.TileX >= Main.maxTilesX - 5 || packet.TileY >= Main.maxTilesY - 5)
            return;

        int above = ((packet.PlayerAbove != 0) ? 1 : (-1));
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
    }

    private void OnWorldWiringHitSwitch(PlayerEntity plr, ref WorldWiringHitSwitch packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected || packet.TileX <= 5 || packet.TileY <= 5 || packet.TileX >= Main.maxTilesX - 5 || packet.TileY >= Main.maxTilesY - 5)
            return;
    }

    private void OnWorldToggleGemLock(PlayerEntity plr, ref WorldToggleGemLock packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected || packet.TileX <= 5 || packet.TileY <= 5 || packet.TileX >= Main.maxTilesX - 5 || packet.TileY >= Main.maxTilesY - 5)
            return;
    }

    private void OnWorldTileRectangle(PlayerEntity plr, ref WorldTileRectangle packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected || packet.StartX <= 5 || packet.StartY <= 5 || packet.StartX >= Main.maxTilesX - 5 || packet.StartY >= Main.maxTilesY - 5 ||
            packet.StartX + packet.SizeX <= 5 || packet.StartY + packet.SizeY <= 5 || packet.StartX + packet.SizeX >= Main.maxTilesX - 5 || packet.StartY + packet.SizeY >= Main.maxTilesY - 5)
            return;
    }

    private void OnWorldTileInteract(PlayerEntity plr, ref WorldTileInteract packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected || packet.TileX <= 5 || packet.TileY <= 5 || packet.TileX >= Main.maxTilesX - 5 || packet.TileY >= Main.maxTilesY - 5)
            return;
    }

    private void OnWorldPaintWall(PlayerEntity plr, ref WorldPaintWall packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected || packet.TileX <= 5 || packet.TileY <= 5 || packet.TileX >= Main.maxTilesX - 5 || packet.TileY >= Main.maxTilesY - 5)
            return;
    }

    private void OnWorldPaintTile(PlayerEntity plr, ref WorldPaintTile packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected || packet.TileX <= 5 || packet.TileY <= 5 || packet.TileX >= Main.maxTilesX - 5 || packet.TileY >= Main.maxTilesY - 5)
            return;
    }

    private void OnWorldMassWireOperation(PlayerEntity plr, ref WorldMassWireOperation packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected || packet.StartX <= 5 || packet.StartY <= 5 || packet.StartX >= Main.maxTilesX - 5 || packet.StartY >= Main.maxTilesY - 5 ||
            packet.EndX <= 5 || packet.EndY <= 5 || packet.EndX >= Main.maxTilesX - 5 || packet.EndY >= Main.maxTilesY - 5)
            return;


        if (Main.tile[packet.TileX, packet.TileY] == null)
        {
            Main.tile[packet.TileX, packet.TileY] = new Tile();
        }
        lock (Main.tile[packet.TileX, packet.TileY])
        {
            Main.tile[packet.TileX, packet.TileY].liquid = b15;
            Main.tile[packet.TileX, packet.TileY].liquidType(liquidType);
            if (Main.netMode == 2)
            {
                WorldGen.SquareTileFrame(packet.TileX, packet.TileY);
                if (b15 == 0)
                {
                    NetMessage.SendData(48, -1, whoAmI, null, packet.TileX, packet.TileY);
                }
            }
            break;
        }
    }

    private void OnWorldLockSomething(PlayerEntity plr, ref WorldLockSomething packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected || packet.TileX <= 5 || packet.TileY <= 5 || packet.TileX >= Main.maxTilesX - 5 || packet.TileY >= Main.maxTilesY - 5)
            return;
    }

    public void Unload()
    {

    }
}
