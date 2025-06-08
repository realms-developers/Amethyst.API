using Amethyst.Network.Handling.Base;
using Amethyst.Network.Handling.Packets.Handshake;
using Amethyst.Network.Packets;
using Amethyst.Server.Entities.Players;
using Terraria;
using Terraria.DataStructures;

namespace Amethyst.Network.Handling.Packets.Items;

public sealed class ItemsHandler : INetworkHandler
{
    public string Name => "net.amethyst.ItemsHandler";

    public void Load()
    {
        NetworkManager.SetMainHandler<ItemUpdateInstanced>(OnItemUpdateInstanced);
        NetworkManager.SetMainHandler<ItemUpdateDefault>(OnItemUpdateDefault);
        NetworkManager.SetMainHandler<ItemUpdateNoPickup>(OnItemUpdateNoPickup);
        NetworkManager.SetMainHandler<ItemUpdateShimmer>(OnItemUpdateShimmer);
    }

    private void OnItemUpdateShimmer(PlayerEntity plr, ref ItemUpdateShimmer packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
            return;

        if (packet.ItemIndex < 0 || packet.ItemIndex >= Main.item.Length ||
            (packet.ItemIndex == 400 && packet.ItemType <= 0))
        {
            return;
        }

        int itemIndex = packet.ItemIndex;

        if (Main.timeItemSlotCannotBeReusedFor[itemIndex] > 0)
            return;

        if (packet.ItemIndex == 400)
        {
            Item item3 = new Item();
            item3.netDefaults(packet.ItemType);
            itemIndex = Item.NewItem(new EntitySource_Sync(), (int)packet.Position.X, (int)packet.Position.Y, item3.width, item3.height, item3.type, packet.ItemStack, noBroadcast: true);
        }

        Item item4 = Main.item[itemIndex];
        item4.netDefaults(packet.ItemType);
        item4.Prefix(packet.ItemPrefix);
        item4.stack = packet.ItemStack;
        item4.position = packet.Position;
        item4.velocity = packet.Velocity;
        item4.active = true;
        item4.playerIndexTheItemIsReservedFor = Main.myPlayer;
        item4.timeLeftInWhichTheItemCannotBeTakenByEnemies = 0;
        item4.shimmered = packet.IsShimmered;
        item4.shimmerTime = packet.ShimmerTime;

        if (packet.ItemIndex == 400)
        {
            PlayerUtils.BroadcastPacketBytes(PacketSendingUtility.CreateSyncItemDefaultPacket(itemIndex)!);

            if (packet.OwnIgnore == 0)
            {
                Main.item[itemIndex].ownIgnore = plr.Index;
                Main.item[itemIndex].ownTime = 100;
            }
            Main.item[itemIndex].FindOwner(itemIndex);
        }
        else
        {
            PlayerUtils.BroadcastPacketBytes(PacketSendingUtility.CreateSyncItemDefaultPacket(itemIndex)!, plr.Index);
        }
    }

    private void OnItemUpdateNoPickup(PlayerEntity plr, ref ItemUpdateNoPickup packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
            return;

        if (packet.ItemIndex < 0 || packet.ItemIndex >= Main.item.Length ||
            (packet.ItemIndex == 400 && packet.ItemType <= 0))
        {
            return;
        }

        int itemIndex = packet.ItemIndex;

        if (Main.timeItemSlotCannotBeReusedFor[itemIndex] > 0)
            return;

        if (packet.ItemIndex == 400)
        {
            Item item3 = new Item();
            item3.netDefaults(packet.ItemType);
            itemIndex = Item.NewItem(new EntitySource_Sync(), (int)packet.Position.X, (int)packet.Position.Y, item3.width, item3.height, item3.type, packet.ItemStack, noBroadcast: true);
        }

        Item item4 = Main.item[itemIndex];
        item4.netDefaults(packet.ItemType);
        item4.Prefix(packet.ItemPrefix);
        item4.stack = packet.ItemStack;
        item4.position = packet.Position;
        item4.velocity = packet.Velocity;
        item4.active = true;
        item4.playerIndexTheItemIsReservedFor = Main.myPlayer;
        item4.timeLeftInWhichTheItemCannotBeTakenByEnemies = packet.TimeEnemiesNoPickup;

        if (packet.ItemIndex == 400)
        {
            PlayerUtils.BroadcastPacketBytes(PacketSendingUtility.CreateSyncItemDefaultPacket(itemIndex)!);

            if (packet.OwnIgnore == 0)
            {
                Main.item[itemIndex].ownIgnore = plr.Index;
                Main.item[itemIndex].ownTime = 100;
            }
            Main.item[itemIndex].FindOwner(itemIndex);
        }
        else
        {
            PlayerUtils.BroadcastPacketBytes(PacketSendingUtility.CreateSyncItemDefaultPacket(itemIndex)!, plr.Index);
        }
    }

    private void OnItemUpdateDefault(PlayerEntity plr, ref ItemUpdateDefault packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
            return;

        if (packet.ItemIndex < 0 || packet.ItemIndex >= Main.item.Length ||
            (packet.ItemIndex == 400 && packet.ItemType <= 0))
        {
            return;
        }

        int itemIndex = packet.ItemIndex;

        if (Main.timeItemSlotCannotBeReusedFor[itemIndex] > 0)
            return;

        if (packet.ItemIndex == 400)
        {
            Item item3 = new Item();
            item3.netDefaults(packet.ItemType);
            itemIndex = Item.NewItem(new EntitySource_Sync(), (int)packet.Position.X, (int)packet.Position.Y, item3.width, item3.height, item3.type, packet.ItemStack, noBroadcast: true);
        }

        Item item4 = Main.item[itemIndex];
        item4.netDefaults(packet.ItemType);
        item4.Prefix(packet.ItemPrefix);
        item4.stack = packet.ItemStack;
        item4.position = packet.Position;
        item4.velocity = packet.Velocity;
        item4.active = true;
        item4.playerIndexTheItemIsReservedFor = Main.myPlayer;
        item4.timeLeftInWhichTheItemCannotBeTakenByEnemies = 0;

        if (packet.ItemIndex == 400)
        {
            PlayerUtils.BroadcastPacketBytes(PacketSendingUtility.CreateSyncItemDefaultPacket(itemIndex)!);

            if (packet.OwnIgnore == 0)
            {
                Main.item[itemIndex].ownIgnore = plr.Index;
                Main.item[itemIndex].ownTime = 100;
            }
            Main.item[itemIndex].FindOwner(itemIndex);
        }
        else
        {
            PlayerUtils.BroadcastPacketBytes(PacketSendingUtility.CreateSyncItemDefaultPacket(itemIndex)!, plr.Index);
        }
    }

    private void OnItemUpdateInstanced(PlayerEntity plr, ref ItemUpdateInstanced packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
            return;

        if (packet.ItemIndex < 0 || packet.ItemIndex >= Main.item.Length ||
            (packet.ItemIndex == 400 && packet.ItemType <= 0))
        {
            return;
        }

        int itemIndex = packet.ItemIndex;

        if (Main.timeItemSlotCannotBeReusedFor[itemIndex] > 0)
            return;

        if (packet.ItemIndex == 400)
        {
            Item item3 = new Item();
            item3.netDefaults(packet.ItemType);
            itemIndex = Item.NewItem(new EntitySource_Sync(), (int)packet.Position.X, (int)packet.Position.Y, item3.width, item3.height, item3.type, packet.ItemStack, noBroadcast: true);
        }

        Item item4 = Main.item[itemIndex];
        item4.netDefaults(packet.ItemType);
        item4.Prefix(packet.ItemPrefix);
        item4.stack = packet.ItemStack;
        item4.position = packet.Position;
        item4.velocity = packet.Velocity;
        item4.active = true;
        item4.playerIndexTheItemIsReservedFor = Main.myPlayer;
        item4.timeLeftInWhichTheItemCannotBeTakenByEnemies = 0;

        if (packet.ItemIndex == 400)
        {
            PlayerUtils.BroadcastPacketBytes(PacketSendingUtility.CreateSyncItemInstancedPacket(itemIndex)!);

            if (packet.OwnIgnore == 0)
            {
                Main.item[itemIndex].ownIgnore = plr.Index;
                Main.item[itemIndex].ownTime = 100;
            }
            Main.item[itemIndex].FindOwner(itemIndex);
        }
        else
        {
            PlayerUtils.BroadcastPacketBytes(PacketSendingUtility.CreateSyncItemInstancedPacket(itemIndex)!, plr.Index);
        }
    }

    public void Unload()
    {
        NetworkManager.SetMainHandler<ItemUpdateInstanced>(null);
        NetworkManager.SetMainHandler<ItemUpdateDefault>(null);
        NetworkManager.SetMainHandler<ItemUpdateNoPickup>(null);
        NetworkManager.SetMainHandler<ItemUpdateShimmer>(null);
    }
}
