using Amethyst.Network.Handling.Base;
using Amethyst.Network.Handling.Packets.Handshake;
using Amethyst.Network.Packets;
using Amethyst.Server.Entities.Players;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Amethyst.Network.Handling.Packets.Signs;

public sealed class ChestsHandler : INetworkHandler
{
    public string Name => "net.amethyst.ChestsHandler";

    public void Load()
    {
        NetworkManager.SetMainHandler<ChestInteract>(OnChestInteract);
        NetworkManager.SetMainHandler<ChestItemSync>(OnChestItemSync);
        NetworkManager.SetMainHandler<ChestRequestOpen>(OnChestRequestOpen);
        NetworkManager.SetMainHandler<ChestSetName>(OnChestSetName);
        NetworkManager.SetMainHandler<ChestSync>(OnChestSync);
    }

    private void OnChestSync(PlayerEntity plr, ref ChestSync packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
            return;

        if (packet.Name != null)
        {
            int chest = Main.player[plr.Index].chest;
            Chest chest2 = Main.chest[chest];
            chest2.name = packet.Name;
            NetMessage.TrySendData(69, -1, plr.Index, NetworkText.Empty, chest, chest2.x, chest2.y);
        }
        Main.player[plr.Index].chest = packet.ChestIndex;
        Recipe.FindRecipes(canDelayCheck: true);
        NetMessage.TrySendData(80, -1, plr.Index, NetworkText.Empty, plr.Index, packet.ChestIndex);
    }

    private void OnChestSetName(PlayerEntity plr, ref ChestSetName packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
            return;

        if (packet.ChestIndex < -1 || packet.ChestIndex >= 8000)
            return;

        int chestIndex = packet.ChestIndex == -1 ? Chest.FindChest(packet.ChestX, packet.ChestY) : packet.ChestIndex;
        if (chestIndex == -1)
            return;

        Chest chest = Main.chest[chestIndex];
        if (chest.x == packet.ChestX && chest.y == packet.ChestY)
            NetMessage.TrySendData(69, plr.Index, -1, NetworkText.Empty, chestIndex, packet.ChestX, packet.ChestY);
    }

    private void OnChestRequestOpen(PlayerEntity plr, ref ChestRequestOpen packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
            return;

        int chestIndex = Chest.FindChest(packet.TileX, packet.TileY);

        if (chestIndex > -1 && Chest.UsingChest(chestIndex) == -1)
        {
            for (int num222 = 0; num222 < 40; num222++)
            {
                NetMessage.TrySendData(32, plr.Index, -1, NetworkText.Empty, chestIndex, num222);
            }
            NetMessage.TrySendData(33, plr.Index, -1, NetworkText.Empty, chestIndex);
            plr.TPlayer.chest = chestIndex;
            if (Main.myPlayer == plr.Index)
            {
                Main.recBigList = false;
            }
            NetMessage.TrySendData(80, -1, plr.Index, NetworkText.Empty, plr.Index, chestIndex);
            if (Main.netMode == 2 && WorldGen.IsChestRigged(packet.TileX, packet.TileY))
            {
                Wiring.SetCurrentUser(plr.Index);
                Wiring.HitSwitch(packet.TileX, packet.TileY);
                Wiring.SetCurrentUser();
                NetMessage.TrySendData(59, -1, plr.Index, NetworkText.Empty, packet.TileX, packet.TileY);
            }
        }
    }

    private void OnChestItemSync(PlayerEntity plr, ref ChestItemSync packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
            return;

        if (packet.ChestIndex >= 0 && packet.ChestIndex < 8000)
        {
            if (Main.chest[packet.ChestIndex] == null)
            {
                Main.chest[packet.ChestIndex] = new Chest();
            }
            if (Main.chest[packet.ChestIndex].item[packet.SlotIndex] == null)
            {
                Main.chest[packet.ChestIndex].item[packet.SlotIndex] = new Item();
            }
            Main.chest[packet.ChestIndex].item[packet.SlotIndex].netDefaults(packet.ItemID);
            Main.chest[packet.ChestIndex].item[packet.SlotIndex].Prefix(packet.ItemPrefix);
            Main.chest[packet.ChestIndex].item[packet.SlotIndex].stack = packet.ItemStack;
            Recipe.FindRecipes(canDelayCheck: true);
        }
    }

    private void OnChestInteract(PlayerEntity plr, ref ChestInteract packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
            return;


        switch (packet.Action)
        {
            case 0:
                {
                    int num209 = WorldGen.PlaceChest(packet.ChestX, packet.ChestY, 21, notNearOtherChests: false, packet.ChestStyle);
                    if (num209 == -1)
                    {
                        NetMessage.TrySendData(34, plr.Index, -1, NetworkText.Empty, packet.Action, packet.ChestX, packet.ChestY, packet.ChestStyle, num209);
                        Item.NewItem(new EntitySource_TileBreak(packet.ChestX, packet.ChestY), packet.ChestX * 16, packet.ChestY * 16, 32, 32, Chest.chestItemSpawn[packet.ChestStyle], 1, noBroadcast: true);
                    }
                    else
                    {
                        NetMessage.TrySendData(34, -1, -1, NetworkText.Empty, packet.Action, packet.ChestX, packet.ChestY, packet.ChestStyle, num209);
                    }
                    break;
                }
            case 1:
                if (Main.tile[packet.ChestX, packet.ChestY].type == 21)
                {
                    Tile tile = Main.tile[packet.ChestX, packet.ChestY];
                    if (tile.frameX % 36 != 0)
                    {
                        packet.ChestX--;
                    }
                    if (tile.frameY % 36 != 0)
                    {
                        packet.ChestY--;
                    }
                    int number = Chest.FindChest(packet.ChestX, packet.ChestY);
                    WorldGen.KillTile(packet.ChestX, packet.ChestY);
                    if (!tile.active())
                    {
                        NetMessage.TrySendData(34, -1, -1, NetworkText.Empty, packet.Action, packet.ChestX, packet.ChestY, 0f, number);
                    }
                    break;
                }
                goto default;
            default:
                switch (packet.Action)
                {
                    case 2:
                        {
                            int num207 = WorldGen.PlaceChest(packet.ChestX, packet.ChestY, 88, notNearOtherChests: false, packet.ChestStyle);
                            if (num207 == -1)
                            {
                                NetMessage.TrySendData(34, plr.Index, -1, NetworkText.Empty, packet.Action, packet.ChestX, packet.ChestY, packet.ChestStyle, num207);
                                Item.NewItem(new EntitySource_TileBreak(packet.ChestX, packet.ChestY), packet.ChestX * 16, packet.ChestY * 16, 32, 32, Chest.dresserItemSpawn[packet.ChestStyle], 1, noBroadcast: true);
                            }
                            else
                            {
                                NetMessage.TrySendData(34, -1, -1, NetworkText.Empty, packet.Action, packet.ChestX, packet.ChestY, packet.ChestStyle, num207);
                            }
                            break;
                        }
                    case 3:
                        if (Main.tile[packet.ChestX, packet.ChestY].type == 88)
                        {
                            Tile tile2 = Main.tile[packet.ChestX, packet.ChestY];
                            packet.ChestX -= (short)(tile2.frameX % 54 / 18);
                            if (tile2.frameY % 36 != 0)
                            {
                                packet.ChestY--;
                            }
                            int number2 = Chest.FindChest(packet.ChestX, packet.ChestY);
                            WorldGen.KillTile(packet.ChestX, packet.ChestY);
                            if (!tile2.active())
                            {
                                NetMessage.TrySendData(34, -1, -1, NetworkText.Empty, packet.Action, packet.ChestX, packet.ChestY, 0f, number2);
                            }
                            break;
                        }
                        goto default;
                    default:
                        switch (packet.Action)
                        {
                            case 4:
                                {
                                    int num208 = WorldGen.PlaceChest(packet.ChestX, packet.ChestY, 467, notNearOtherChests: false, packet.ChestStyle);
                                    if (num208 == -1)
                                    {
                                        NetMessage.TrySendData(34, plr.Index, -1, NetworkText.Empty, packet.Action, packet.ChestX, packet.ChestY, packet.ChestStyle, num208);
                                        Item.NewItem(new EntitySource_TileBreak(packet.ChestX, packet.ChestY), packet.ChestX * 16, packet.ChestY * 16, 32, 32, Chest.chestItemSpawn2[packet.ChestStyle], 1, noBroadcast: true);
                                    }
                                    else
                                    {
                                        NetMessage.TrySendData(34, -1, -1, NetworkText.Empty, packet.Action, packet.ChestX, packet.ChestY, packet.ChestStyle, num208);
                                    }
                                    break;
                                }
                            case 5:
                                if (Main.tile[packet.ChestX, packet.ChestY].type == 467)
                                {
                                    Tile tile3 = Main.tile[packet.ChestX, packet.ChestY];
                                    if (tile3.frameX % 36 != 0)
                                    {
                                        packet.ChestX--;
                                    }
                                    if (tile3.frameY % 36 != 0)
                                    {
                                        packet.ChestY--;
                                    }
                                    int number3 = Chest.FindChest(packet.ChestX, packet.ChestY);
                                    WorldGen.KillTile(packet.ChestX, packet.ChestY);
                                    if (!tile3.active())
                                    {
                                        NetMessage.TrySendData(34, -1, -1, NetworkText.Empty, packet.Action, packet.ChestX, packet.ChestY, 0f, number3);
                                    }
                                }
                                break;
                        }
                        break;
                }
                break;
        }


        switch (packet.Action)
        {
            case 0:
                if (packet.ChestAdditionalValue == -1)
                {
                    WorldGen.KillTile(packet.ChestX, packet.ChestY);
                    break;
                }
                WorldGen.PlaceChestDirect(packet.ChestX, packet.ChestY, 21, packet.ChestStyle, packet.ChestAdditionalValue);
                break;
            case 2:
                if (packet.ChestAdditionalValue == -1)
                {
                    WorldGen.KillTile(packet.ChestX, packet.ChestY);
                    break;
                }
                WorldGen.PlaceDresserDirect(packet.ChestX, packet.ChestY, 88, packet.ChestStyle, packet.ChestAdditionalValue);
                break;
            case 4:
                if (packet.ChestAdditionalValue == -1)
                {
                    WorldGen.KillTile(packet.ChestX, packet.ChestY);
                    break;
                }
                WorldGen.PlaceChestDirect(packet.ChestX, packet.ChestY, 467, packet.ChestStyle, packet.ChestAdditionalValue);
                break;
            default:
                Chest.DestroyChestDirect(packet.ChestX, packet.ChestY, packet.ChestAdditionalValue);
                WorldGen.KillTile(packet.ChestX, packet.ChestY);
                break;
        }
    }

    public void Unload()
    {
        NetworkManager.SetMainHandler<ChestInteract>(null);
        NetworkManager.SetMainHandler<ChestItemSync>(null);
        NetworkManager.SetMainHandler<ChestRequestOpen>(null);
        NetworkManager.SetMainHandler<ChestSetName>(null);
        NetworkManager.SetMainHandler<ChestSync>(null);
    }
}
