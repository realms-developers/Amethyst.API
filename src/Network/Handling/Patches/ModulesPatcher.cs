using Amethyst.Network.Handling.Packets.Handshake;
using Amethyst.Server.Entities;
using Amethyst.Server.Entities.Players;
using Terraria.GameContent.NetModules;
using Terraria.Net;

namespace Amethyst.API.Network.Handling.Patches;

public static class ModulesPatcher
{
    public static HashSet<int> AllowedModules { get; } = [ 0 ];

    internal static unsafe void Initialize()
    {
        On.Terraria.Net.NetManager.Broadcast_NetPacket_int += (orig, self, packet, plrIndex) =>
        {
            if (AllowedModules.Contains(packet.Id))
            {
                packet.ShrinkToFit();
                PlayerUtils.BroadcastPacketBytes(packet.Buffer.Data, 0, packet.Length, plrIndex);
            }
        };

        On.Terraria.Net.NetManager.Broadcast_NetPacket_BroadcastCondition_int += (orig, self, packet, cond, plrIndex) =>
        {
            if (AllowedModules.Contains(packet.Id) && cond(plrIndex))
            {
                packet.ShrinkToFit();
                PlayerUtils.BroadcastPacketBytes(packet.Buffer.Data, 0, packet.Length, plrIndex);
            }
        };

        On.Terraria.Net.NetManager.SendToClient += (orig, self, packet, plrIndex) =>
        {
            if (AllowedModules.Contains(packet.Id))
            {
                packet.ShrinkToFit();
                EntityTrackers.Players[plrIndex]?.SendPacketBytes(packet.Buffer.Data, 0, packet.Length);
            }
        };

        On.Terraria.GameContent.NetModules.NetLiquidModule.PrepareAndSendToEachPlayerSeparately += (orig) =>
        {
            if (AllowedModules.Contains(0))
            {
                foreach (PlayerEntity plr in EntityTrackers.Players)
                {
                    if (plr.Phase != ConnectionPhase.Connected)
                    {
                        continue;
                    }
                    
                    NetPacket packet = NetLiquidModule.SerializeForPlayer(plr.Index);
                    packet.ShrinkToFit();
                    plr.SendPacketBytes(packet.Buffer.Data, 0, packet.Length);
                }
            }
        };
    }
}