using Amethyst.Network.Handling.Base;
using Amethyst.Network.Handling.Packets.Handshake;
using Amethyst.Network.Packets;
using Amethyst.Network.Structures;
using Amethyst.Server.Entities;
using Amethyst.Server.Entities.Players;
using Terraria;
using Terraria.GameContent.UI;

namespace Amethyst.Network.Handling.Packets.Other;

public sealed class OtherHandler : INetworkHandler
{
    public string Name => "net.amethyst.OtherHandler";

    public void Load()
    {
        NetworkManager.SetMainHandler<PlayerOrEntityTeleport>(OnPlayerOrEntityTeleport);
        NetworkManager.SetMainHandler<GolfPutBallInCup>(OnGolfPutBallInCup);
        NetworkManager.SetMainHandler<VisualCreateEmoteBubble>(OnVisualCreateEmoteBubble);
    }

    private void OnVisualCreateEmoteBubble(PlayerEntity plr, ref VisualCreateEmoteBubble packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        if (packet.EmoteID >= 0 && packet.EmoteID < EmoteID.Count)
        {
            EmoteBubble.NewBubble(packet.EmoteID, new WorldUIAnchor(plr.TPlayer), 360);
            EmoteBubble.CheckForNPCsToReactToEmoteBubble(packet.EmoteID, plr.TPlayer);
        }
    }

    private void OnGolfPutBallInCup(PlayerEntity plr, ref GolfPutBallInCup packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        PacketSendingUtility.ExcludeBroadcastConnected(-1, GolfPutBallInCupPacket.Serialize(packet with { PlayerIndex = (byte)plr.Index }));
    }

    private void OnPlayerOrEntityTeleport(PlayerEntity plr, ref PlayerOrEntityTeleport packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        byte type = 0;
        NetBitsByte bb = packet.Flags;
        if (bb[0])
        {
            type++;
        }
        if (bb[1])
        {
            type += 2;
        }

        NetVector2 targetPosition = packet.TargetPosition;
        if (bb[2])
        {
            targetPosition = EntityTrackers.Players[(byte)packet.EntityIndex]?.Position ?? targetPosition;
        }

        switch (type)
        {
            case 0:
                plr.TPlayer.Teleport(packet.TargetPosition, packet.Style);
                break;
            case 1:
                if (packet.EntityIndex < 0 || packet.EntityIndex > Main.npc.Length - 1)
                {
                    return;
                }

                Main.npc[packet.EntityIndex].Teleport(targetPosition, packet.Style, packet.ExtraInfo ?? 0);
                break;
            case 2:
                plr.TPlayer.Teleport(targetPosition, packet.Style, packet.ExtraInfo ?? 0);
                break;
        }
    }

    public void Unload()
    {
        NetworkManager.SetMainHandler<PlayerOrEntityTeleport>(null);
        NetworkManager.SetMainHandler<GolfPutBallInCup>(null);
        NetworkManager.SetMainHandler<VisualCreateEmoteBubble>(null);
    }
}
