using System.Collections;
using Amethyst.Server.Entities.Base;

namespace Amethyst.Server.Entities.Players.Tracking;

public sealed class PlayerTracker : IEntityTracker<PlayerEntity>
{
    public PlayerTracker() {}

    public PlayerEntity this[int index] => _activeEntities[index];

    public IEnumerable<PlayerEntity> ActiveEntities => _activeEntities.Where(e => e != null);

    public ITrackerManager<PlayerEntity>? Manager => new PlayerTrackerManager(this);

    internal PlayerEntity[] _activeEntities = new PlayerEntity[255];

    public IEnumerator<PlayerEntity> GetEnumerator() => ActiveEntities.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
