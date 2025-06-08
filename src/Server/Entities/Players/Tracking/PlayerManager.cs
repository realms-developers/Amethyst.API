using Amethyst.Hooks;
using Amethyst.Hooks.Args.Players;
using Amethyst.Hooks.Context;
using Amethyst.Network;
using Amethyst.Network.Handling.Packets.Handshake;
using Amethyst.Server.Entities.Base;
using Amethyst.Systems.Chat;
using Amethyst.Systems.Chat.Misc.Context;
using Terraria;

namespace Amethyst.Server.Entities.Players.Tracking;

public sealed class PlayerManager : IEntityManager<PlayerEntity>
{
    public PlayerManager(PlayerTracker tracker)
    {
        _tracker = tracker;
    }

    public IEntityTracker<PlayerEntity> Tracker => _tracker;

    private PlayerTracker _tracker;

    private static Func<PlayerTrackerInsertArgs, HookResult<PlayerTrackerInsertArgs>> _insertHook = HookRegistry.GetHook<PlayerTrackerInsertArgs>()!.Invoke;
    private static Func<PlayerTrackerRemoveArgs, HookResult<PlayerTrackerRemoveArgs>> _removeHook = HookRegistry.GetHook<PlayerTrackerRemoveArgs>()!.Invoke;

    public void Insert(int index, PlayerEntity entity)
    {
        if (_tracker._players[index] != null)
            throw new InvalidOperationException($"Player at index {index} already exists.");

        _tracker._players[index] = entity;

        Main.player[index] = new();
        Main.player[index].active = true;
        Main.player[index].whoAmI = index;

        _insertHook.Invoke(new PlayerTrackerInsertArgs(entity));

        AmethystLog.System.Info(nameof(PlayerManager), $"[{Tracker.Count()}/{NetworkManager.MaxPlayers}] INS => player_{entity.Index}");
    }

    public void Remove(int index)
    {
        var plr = _tracker._players[index];
        if (plr == null)
            throw new InvalidOperationException($"Player at index {index} does not exist.");

        if (plr.Phase == ConnectionPhase.Connected)
            ServerChat.MessagePlayerLeft.Invoke(new PlayerLeftMessageContext(plr));

        if (Main.player[index] != null)
            Main.player[index].active = false;

        _removeHook.Invoke(new PlayerTrackerRemoveArgs(plr));
        AmethystLog.System.Info(nameof(PlayerManager), $"[{Tracker.Count() - 1}/{NetworkManager.MaxPlayers}] RMV => player_{index} ({plr.Name ?? "not_identified"})");

        plr.Dispose();
        _tracker._players[index] = null!;
    }
}
