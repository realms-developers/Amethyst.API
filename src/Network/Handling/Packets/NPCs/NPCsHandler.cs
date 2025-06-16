using Amethyst.Network.Handling.Base;
using Amethyst.Network.Handling.Packets.Handshake;
using Amethyst.Network.Packets;
using Amethyst.Server.Entities.Players;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Amethyst.Network.Handling.Packets.NPCs;

public sealed class NPCsHandler : INetworkHandler
{
    public string Name => "net.amethyst.NPCsHandler";

    public void Load()
    {
        NetworkManager.SetMainHandler<NPCAddBuff>(OnNPCAddBuff);
        NetworkManager.SetMainHandler<NPCCatch>(OnNPCCatch);
        NetworkManager.SetMainHandler<NPCFishOut>(OnNPCFishOut);
        NetworkManager.SetMainHandler<NPCMoneyPing>(OnNPCMoneyPing);
        NetworkManager.SetMainHandler<NPCMoveHome>(OnNPCMoveHome);
        // NetworkManager.AddHandler<NPCNameAndVariation>(OnNPCNameAndVariation); // NOT NEEDED
        NetworkManager.SetMainHandler<NPCPortalGunTeleport>(OnNPCPortalGunTeleport);
        NetworkManager.SetMainHandler<NPCRelease>(OnNPCRelease);
        NetworkManager.SetMainHandler<NPCRequestBuffRemoval>(OnNPCRequestBuffRemoval);
        NetworkManager.SetMainHandler<NPCStrike>(OnNPCStrike);
        // NetworkManager.AddHandler<NPCStrikeByItem>(OnNPCStrikeByItem); // UNUSED
    }

    private void OnNPCStrike(PlayerEntity plr, ref NPCStrike packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        if (packet.NPCIndex < 0 || packet.NPCIndex >= Main.npc.Length)
        {
            return;
        }

        NPC npc = Main.npc[packet.NPCIndex];
        if (npc == null || !npc.active)
        {
            return;
        }

        npc.PlayerInteraction(plr.Index);

        if (packet.Damage >= 0)
        {
            npc.StrikeNPC(packet.Damage, packet.Knockback, packet.HitContext - 1, packet.IsCrit, noEffect: false, fromNet: true);
        }
        else
        {
            npc.life = 0;
            npc.HitEffect();
            npc.active = false;
        }

        NetMessage.TrySendData(28, -1, plr.Index, NetworkText.Empty, packet.NPCIndex, packet.Damage, packet.Knockback, packet.HitContext, packet.IsCrit ? 1 : 0);
        if (npc.life <= 0)
        {
            NetMessage.TrySendData(23, -1, -1, NetworkText.Empty, packet.NPCIndex);
        }
        else
        {
            npc.netUpdate = true;
        }
        if (npc.realLife >= 0)
        {
            if (Main.npc[npc.realLife].life <= 0)
            {
                NetMessage.TrySendData(23, -1, -1, NetworkText.Empty, npc.realLife);
            }
            else
            {
                Main.npc[npc.realLife].netUpdate = true;
            }
        }
    }

    private void OnNPCRequestBuffRemoval(PlayerEntity plr, ref NPCRequestBuffRemoval packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        if (packet.NPCIndex < 0 || packet.NPCIndex >= Main.npc.Length)
        {
            return;
        }

        NPC npc = Main.npc[packet.NPCIndex];
        if (npc == null || !npc.active)
        {
            return;
        }

        npc.RequestBuffRemoval(packet.TargetBuffType);
    }

    private void OnNPCRelease(PlayerEntity plr, ref NPCRelease packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        NPC.ReleaseNPC(packet.PositionX, packet.PositionY, packet.NPCType, packet.NPCStyle, plr.Index);
    }

    private void OnNPCPortalGunTeleport(PlayerEntity plr, ref NPCPortalGunTeleport packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        if (packet.NPCIndex < 0 || packet.NPCIndex >= Main.npc.Length)
        {
            return;
        }

        NPC npc = Main.npc[packet.NPCIndex];
        if (npc == null || !npc.active)
        {
            return;
        }

        npc.lastPortalColorIndex = packet.TeleportExtraInfo + ((packet.TeleportExtraInfo % 2 == 0) ? 1 : (-1));
        npc.Teleport(packet.Position, 4, packet.TeleportExtraInfo);
        npc.velocity = packet.Velocity;
        npc.netOffset *= 0f;
    }

    private void OnNPCMoveHome(PlayerEntity plr, ref NPCMoveHome packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        if (packet.NPCIndex < 0 || packet.NPCIndex >= Main.npc.Length)
        {
            return;
        }

        if (Main.npc[packet.NPCIndex]?.active != true)
        {
            return;
        }

        switch (packet.Action)
        {
            case 1:
                WorldGen.kickOut(packet.NPCIndex);
                break;

            case 2:
                WorldGen.moveRoom(packet.NPCIndex, packet.TileX, packet.TileY);
                break;
        }
    }

    private void OnNPCMoneyPing(PlayerEntity plr, ref NPCMoneyPing packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        if (packet.NPCIndex < 0 || packet.NPCIndex >= Main.npc.Length)
        {
            return;
        }

        NPC npc = Main.npc[packet.NPCIndex];
        if (npc == null || !npc.active)
        {
            return;
        }

        npc.extraValue += packet.ExtraValue;
        NetMessage.TrySendData(92, -1, -1, NetworkText.Empty, packet.NPCIndex, npc.extraValue, packet.Position.X, packet.Position.Y);
    }

    private void OnNPCFishOut(PlayerEntity plr, ref NPCFishOut packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (packet.NPCType == 682)
        {
            if (NPC.unlockedSlimeRedSpawn)
            {
                return;
            }

            NPC.unlockedSlimeRedSpawn = true;
            PacketSendingUtility.ExcludeBroadcastConnected(-1, PacketSendingUtility.DirectCreateWorldInfoPacket());
        }

        int x = packet.TileX * 16;
        int y = packet.TileY * 16;

        NPC nPC3 = new();
        nPC3.SetDefaults(packet.NPCType);
        int type5 = nPC3.type;
        int netID = nPC3.netID;
        int num96 = NPC.NewNPC(new EntitySource_FishedOut(plr.TPlayer), x, y, packet.NPCType);
        if (netID != type5)
        {
            Main.npc[num96].SetDefaults(netID);
            NetMessage.TrySendData(23, -1, -1, NetworkText.Empty, num96);
        }
        if (packet.NPCType == 682)
        {
            WorldGen.CheckAchievement_RealEstateAndTownSlimes();
        }
    }

    private void OnNPCCatch(PlayerEntity plr, ref NPCCatch packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        NPC.CatchNPC(packet.NPCIndex, plr.Index);
    }

    private void OnNPCAddBuff(PlayerEntity plr, ref NPCAddBuff packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        if (packet.NPCIndex < 0 || packet.NPCIndex >= Main.npc.Length)
        {
            return;
        }

        NPC npc = Main.npc[packet.NPCIndex];
        if (npc == null || !npc.active)
        {
            return;
        }

        npc.AddBuff(packet.BuffType, packet.BuffTime, true);

        NetMessage.TrySendData(54, -1, -1, NetworkText.Empty, packet.NPCIndex);
    }

    public void Unload()
    {
        NetworkManager.SetMainHandler<NPCAddBuff>(null);
        NetworkManager.SetMainHandler<NPCCatch>(null);
        NetworkManager.SetMainHandler<NPCFishOut>(null);
        NetworkManager.SetMainHandler<NPCMoneyPing>(null);
        NetworkManager.SetMainHandler<NPCMoveHome>(null);
        NetworkManager.SetMainHandler<NPCPortalGunTeleport>(null);
        NetworkManager.SetMainHandler<NPCRelease>(null);
        NetworkManager.SetMainHandler<NPCRequestBuffRemoval>(null);
        NetworkManager.SetMainHandler<NPCStrike>(null);
    }
}
