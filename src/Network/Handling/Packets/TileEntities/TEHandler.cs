using Amethyst.Network.Handling.Base;
using Amethyst.Network.Handling.Packets.Handshake;
using Amethyst.Network.Packets;
using Amethyst.Network.Utilities;
using Amethyst.Server.Entities.Players;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;
using Terraria.Localization;

namespace Amethyst.Network.Handling.Packets.TileEntities;

public sealed class TEHandler : INetworkHandler
{
    public string Name => "net.amethyst.TEHandler";

    public void Load()
    {
        NetworkManager.AddDirectHandler(86, OnReadTileEntity);
        NetworkManager.AddHandler<TEClearAnchor>(OnClearAnchor);
        NetworkManager.AddHandler<TEPlaceEntity>(OnPlaceEntity);
        NetworkManager.AddHandler<TETryPlaceItemDisplayDoll>(OnTryPlaceItemDisplayDoll);
        NetworkManager.AddHandler<TETryPlaceItemFoodPlatter>(OnTryPlaceItemFoodPlatter);
        NetworkManager.AddHandler<TETryPlaceItemHatRack>(OnTryPlaceItemHatRack);
        NetworkManager.AddHandler<TETryPlaceItemItemFrame>(OnTryPlaceItemItemFrame);
        NetworkManager.AddHandler<TETryPlaceItemWeaponsRack>(OnTryPlaceItemWeaponsRack);
    }

    private void OnTryPlaceItemWeaponsRack(PlayerEntity plr, ref TETryPlaceItemWeaponsRack packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected || !(packet.X, packet.Y).IsInWorld())
            return;

        TEWeaponsRack.TryPlacing(packet.X, packet.Y, packet.ItemType, packet.ItemPrefix, packet.ItemStack);
    }

    private void OnTryPlaceItemItemFrame(PlayerEntity plr, ref TETryPlaceItemItemFrame packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected || !(packet.X, packet.Y).IsInWorld())
            return;

        TEItemFrame.TryPlacing(packet.X, packet.Y, packet.ItemType, packet.ItemPrefix, packet.ItemStack);
    }

    private void OnTryPlaceItemHatRack(PlayerEntity plr, ref TETryPlaceItemHatRack packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
            return;

        if (TileEntity.ByID.TryGetValue(packet.TEIndex, out var unparsedTE) && unparsedTE is TEHatRack te)
        {
            bool isDye = packet.SlotIndex >= 2;
            int slot = isDye ? packet.SlotIndex - 2 : packet.SlotIndex;

            Item item = new();
            item.SetDefaults(packet.ItemType);
            item.stack = packet.ItemStack;
            item.Prefix(packet.ItemPrefix);

            if (isDye)
            {
                te._dyes[slot] = item;
            }
            else
            {
                te._items[slot] = item;
            }
        }
    }

    private void OnTryPlaceItemFoodPlatter(PlayerEntity plr, ref TETryPlaceItemFoodPlatter packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected || !(packet.X, packet.Y).IsInWorld())
            return;

        TEFoodPlatter.TryPlacing(packet.X, packet.Y, packet.ItemType, packet.ItemPrefix, packet.ItemStack);
    }

    private void OnTryPlaceItemDisplayDoll(PlayerEntity plr, ref TETryPlaceItemDisplayDoll packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
            return;

        if (TileEntity.ByID.TryGetValue(packet.TEIndex, out var unparsedTE) && unparsedTE is TEDisplayDoll te)
        {
            bool isDye = packet.SlotIndex >= 8;
            int slot = isDye ? packet.SlotIndex - 8 : packet.SlotIndex;

            Item item = new();
            item.SetDefaults(packet.ItemType);
            item.stack = packet.ItemStack;
            item.Prefix(packet.ItemPrefix);

            if (isDye)
            {
                te._dyes[slot] = item;
            }
            else
            {
                te._items[slot] = item;
            }
        }
    }

    private void OnPlaceEntity(PlayerEntity plr, ref TEPlaceEntity packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected || !(packet.X, packet.Y).IsInWorld())
            return;

        if (WorldGen.InWorld(packet.X, packet.Y) && !TileEntity.ByPosition.ContainsKey(new Point16(packet.X, packet.Y)))
		{
			TileEntity.PlaceEntityNet(packet.X, packet.Y, packet.Type);
		}
    }

    private void OnClearAnchor(PlayerEntity plr, ref TEClearAnchor packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
            return;

        if (packet.TEIndex == -1)
        {
            Main.player[plr.Index].tileEntityAnchor.Clear();
            NetMessage.TrySendData(122, -1, -1, NetworkText.Empty, packet.TEIndex, plr.Index);
            return;
        }
        if (!TileEntity.IsOccupied(packet.TEIndex, out var _) && TileEntity.ByID.TryGetValue(packet.TEIndex, out var value6))
        {
            Main.player[plr.Index].tileEntityAnchor.Set(packet.TEIndex, value6.Position.X, value6.Position.Y);
            NetMessage.TrySendData(122, -1, -1, NetworkText.Empty, packet.TEIndex, plr.Index);
        }
    }

    private void OnReadTileEntity(PlayerEntity plr, ReadOnlySpan<byte> data, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
            return;

        FastPacketReader reader = new(data);

		int num67 = reader.ReadInt32();
		if (!reader.ReadBoolean())
		{
			if (TileEntity.ByID.TryGetValue(num67, out var value3))
			{
				TileEntity.ByID.Remove(num67);
				TileEntity.ByPosition.Remove(value3.Position);
			}
		}
		else
		{
            var stream = reader.StreamOpen();
            BinaryReader breader = new(stream);
			TileEntity tileEntity = TileEntity.Read(breader, networkSend: true);
            breader.Dispose();
            reader.StreamClose(stream);

            if (!(tileEntity.Position.X, tileEntity.Position.Y).IsInWorld())
            {
                return;
            }

			tileEntity.ID = num67;
			TileEntity.ByID[tileEntity.ID] = tileEntity;
			TileEntity.ByPosition[tileEntity.Position] = tileEntity;
		}
    }

    public void Unload()
    {
        NetworkManager.RemoveDirectHandler(86, OnReadTileEntity);
        NetworkManager.RemoveHandler<TEClearAnchor>(OnClearAnchor);
        NetworkManager.RemoveHandler<TEPlaceEntity>(OnPlaceEntity);
        NetworkManager.RemoveHandler<TETryPlaceItemDisplayDoll>(OnTryPlaceItemDisplayDoll);
        NetworkManager.RemoveHandler<TETryPlaceItemFoodPlatter>(OnTryPlaceItemFoodPlatter);
        NetworkManager.RemoveHandler<TETryPlaceItemHatRack>(OnTryPlaceItemHatRack);
        NetworkManager.RemoveHandler<TETryPlaceItemItemFrame>(OnTryPlaceItemItemFrame);
        NetworkManager.RemoveHandler<TETryPlaceItemWeaponsRack>(OnTryPlaceItemWeaponsRack);
    }
}
