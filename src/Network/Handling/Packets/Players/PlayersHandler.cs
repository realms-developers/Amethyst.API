using Amethyst.Hooks;
using Amethyst.Hooks.Args.Players;
using Amethyst.Hooks.Context;
using Amethyst.Network.Handling.Base;
using Amethyst.Network.Handling.Packets.Handshake;
using Amethyst.Network.Packets;
using Amethyst.Server.Entities.Players;

namespace Amethyst.Network.Handling.Packets.Players;

public sealed class PlayersHandler : INetworkHandler
{
    public string Name => "net.amethyst.PlayersHandler";

    public void Load()
    {
        NetworkManager.SetMainHandler<PlayerUpdate>(OnPlayerUpdate);
        NetworkManager.SetMainHandler<PlayerItemRotation>(OnPlayerItemRotation);
        NetworkManager.SetMainHandler<PlayerSpawn>(OnPlayerSpawn);

        HookRegistry.GetHook<PlayerFullyJoinedArgs>()
            ?.Register(SyncPlayer);
    }

    private void OnPlayerSpawn(PlayerEntity plr, ref PlayerSpawn packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        throw new NotImplementedException();
    }

    private void OnPlayerItemRotation(PlayerEntity plr, ref PlayerItemRotation packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
            return;

        if (packet.PlayerIndex != plr.Index)
        {
            plr.Kick("network.invalidPlayerIndex");
            ignore = true;
            return;
        }

        if (HandlersConfiguration.Instance.SyncPlayers)
        {
            PacketSendingUtility.ExcludeBroadcastConnected(plr.Index, PlayerItemRotationPacket.Serialize(packet)); // Serialize, because rawPacket can be shitty
        }
    }

    private void SyncPlayer(in PlayerFullyJoinedArgs args, HookResult<PlayerFullyJoinedArgs> result)
    {
        if (HandlersConfiguration.Instance.SyncPlayers)
        {
            args.Player.SyncFromAll();
            args.Player.SyncToAll();
        }
    }

    private void OnPlayerUpdate(PlayerEntity plr, ref PlayerUpdate packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
            return;

        if (packet.PlayerIndex != plr.Index)
        {
            plr.Kick("network.invalidPlayerIndex");
            ignore = true;
            return;
        }

        plr.Position = packet.Position;
        plr.Velocity = packet.Velocity ?? new(0, 0);

        plr.PlayerUpdateInfo = packet;

        if (HandlersConfiguration.Instance.SyncPlayers)
        {
            PacketSendingUtility.ExcludeBroadcastConnected(plr.Index, PlayerUpdatePacket.Serialize(packet)); // Serialize, because rawPacket can be shitty
        }
    }

    public void Unload()
    {
        HookRegistry.GetHook<PlayerFullyJoinedArgs>()
            ?.Unregister(SyncPlayer);

        NetworkManager.SetMainHandler<PlayerItemRotation>(null);
        NetworkManager.SetMainHandler<PlayerUpdate>(null);
    }
}
