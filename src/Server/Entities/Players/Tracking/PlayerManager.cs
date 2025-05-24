using Amethyst.Hooks;
using Amethyst.Hooks.Args.Players;
using Amethyst.Hooks.Context;
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

    public void Insert(int index, PlayerEntity entity)
    {
        if (_tracker._players[index] != null)
            throw new InvalidOperationException($"Player at index {index} already exists.");

        _tracker._players[index] = entity;

        HookRegistry.GetHook<PlayerTrackerInsertArgs>()
            ?.Invoke(new PlayerTrackerInsertArgs(entity));
    }

    public void Remove(int index)
    {
        if (_tracker._players[index] == null)
            throw new InvalidOperationException($"Player at index {index} does not exist.");

        HookRegistry.GetHook<PlayerTrackerRemoveArgs>()
            ?.Invoke(new PlayerTrackerRemoveArgs(_tracker._players[index]!));

        _tracker._players[index] = null!;
    }

    public void AttachHooks()
    {
        HookRegistry.GetHook<PlayerSocketConnectArgs>().Register(OnPlayerConnect);
        HookRegistry.GetHook<PlayerSocketDisconnectArgs>().Register(OnPlayerDisconnect);
    }

    public void DeattachHooks()
    {
        HookRegistry.GetHook<PlayerSocketConnectArgs>().Unregister(OnPlayerConnect);
        HookRegistry.GetHook<PlayerSocketDisconnectArgs>().Unregister(OnPlayerDisconnect);
    }

    private void OnPlayerConnect(in PlayerSocketConnectArgs args, HookResult<PlayerSocketConnectArgs> result)
    {
        PlayerEntity player = new PlayerEntity(args.Index);
        Insert(args.Index, player);
    }

    private void OnPlayerDisconnect(in PlayerSocketDisconnectArgs args, HookResult<PlayerSocketDisconnectArgs> result)
    {
        Remove(args.Index);
    }
}
