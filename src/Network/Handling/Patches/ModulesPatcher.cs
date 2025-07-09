using Amethyst.Server.Entities;
using Amethyst.Server.Entities.Players;

namespace Amethyst.API.Network.Handling.Patches;

public static class ModulesPatcher
{
    public static HashSet<int> AllowedModules { get; } = [ 1 ];

    internal static unsafe void Initialize()
    {
        On.Terraria.Net.NetManager.Broadcast_NetPacket_int += (orig, self, packet, plrIndex) =>
        {
            if (AllowedModules.Contains(packet.Id))
            {
                PlayerUtils.BroadcastPacketBytes(packet.Buffer.Data, plrIndex);
            }
        };

        On.Terraria.Net.NetManager.Broadcast_NetPacket_BroadcastCondition_int += (orig, self, packet, cond, plrIndex) =>
        {
            if (AllowedModules.Contains(packet.Id) && cond(plrIndex))
            {
                PlayerUtils.BroadcastPacketBytes(packet.Buffer.Data, plrIndex);
            }
        };

        On.Terraria.Net.NetManager.SendToClient += (orig, self, packet, plrIndex) =>
        {
            if (AllowedModules.Contains(packet.Id))
            {
                EntityTrackers.Players[plrIndex]?.SendPacketBytes(packet.Buffer.Data);
            }
        };
    }
}