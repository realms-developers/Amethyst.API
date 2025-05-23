using Amethyst.Hooks;
using Amethyst.Server.Entities.Base;
using Amethyst.Server.Entities.Players.Hooks;

namespace Amethyst.Server.Entities.Players.Tracking;

public sealed class PlayerTrackerManager : ITrackerManager<PlayerEntity>
{
    public PlayerTrackerManager(PlayerTracker tracker)
    {
        _tracker = tracker;
    }

    public IEntityTracker<PlayerEntity> Tracker => _tracker;

    private PlayerTracker _tracker;

    public void Insert(int index, PlayerEntity entity)
    {
        if (_tracker._activeEntities[index] != null)
            throw new InvalidOperationException($"Player at index {index} already exists.");

        _tracker._activeEntities[index] = entity;

        HookRegistry.GetHook<PlayerTrackerInsertArgs>()
            ?.Invoke(new PlayerTrackerInsertArgs(entity));
    }

    public void Remove(int index)
    {
        if (_tracker._activeEntities[index] == null)
            throw new InvalidOperationException($"Player at index {index} does not exist.");

        HookRegistry.GetHook<PlayerTrackerRemoveArgs>()
            ?.Invoke(new PlayerTrackerRemoveArgs(_tracker._activeEntities[index]!));

        _tracker._activeEntities[index] = null!;
    }
}
