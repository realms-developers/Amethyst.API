using Amethyst.Hooks;
using Amethyst.Hooks.Args.Players;
using Amethyst.Hooks.Context;
using Amethyst.Network;
using Amethyst.Server.Entities.Base;

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

        _insertHook.Invoke(new PlayerTrackerInsertArgs(entity));

        AmethystLog.System.Info(nameof(PlayerManager), $"[{Tracker.Count()}/{NetworkManager.MaxPlayers}] INS => player_{entity.Index}");
    }

    public void Remove(int index)
    {
        if (_tracker._players[index] == null)
            throw new InvalidOperationException($"Player at index {index} does not exist.");

        _removeHook.Invoke(new PlayerTrackerRemoveArgs(_tracker._players[index]));
        AmethystLog.System.Info(nameof(PlayerManager), $"[{Tracker.Count() - 1}/{NetworkManager.MaxPlayers}] RMV => player_{index} ({_tracker._players[index].Name ?? "not_identified"})");

        _tracker._players[index].Dispose();
        _tracker._players[index] = null!;
    }
}
