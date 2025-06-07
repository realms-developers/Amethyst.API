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
        NetworkManager.AddHandler<PlayerUpdate>(OnPlayerUpdate);

        HookRegistry.GetHook<PlayerFullyJoinedArgs>()
            ?.Register(SyncPlayer);
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

        // TODO: use it in Amethyst.Security (in future)

        // if (!packet.Position.IsValid() ||
        //     (packet.Velocity != null && !packet.Velocity.Value.IsValid()) ||
        //     (packet.PotionHomePosition != null && !packet.PotionHomePosition.Value.IsValid()) ||
        //     (packet.PotionOriginalPosition != null && !packet.PotionOriginalPosition.Value.IsValid()))
        // {
        //     plr.Kick("network.invalidPlayerUpdate");
        //     AmethystLog.Network.Debug(nameof(PlayersHandler), $"Player {plr.Index} sent an invalid PlayerUpdate packet (vectors): {packet.Position}, {packet.Velocity}, {packet.PotionHomePosition}, {packet.PotionOriginalPosition}");
        //     ignore = true;
        //     return;
        // }

        // if (packet.SelectedItem < 0 || packet.SelectedItem >= 59)
        // {
        //     plr.Kick("network.invalidSelectedItem");
        //     ignore = true;
        //     return;
        // }

        plr.Position = packet.Position;
        plr.Velocity = packet.Velocity ?? new(0, 0);

        plr.PlayerUpdateInfo = packet;

        // if (HandlersConfiguration.Instance.SyncPlayers)
        // {
        //     PacketSendingUtility.ExcludeBroadcastConnected(plr.Index, rawPacket.ToArray());
        // }
    }

    public void Unload()
    {
        HookRegistry.GetHook<PlayerFullyJoinedArgs>()
            ?.Unregister(SyncPlayer);

        NetworkManager.RemoveHandler<PlayerUpdate>(OnPlayerUpdate);
    }
}
