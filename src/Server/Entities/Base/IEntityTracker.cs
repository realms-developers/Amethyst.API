namespace Amethyst.Server.Entities.Base;

public interface IEntityTracker<TEntity> : IEnumerable<TEntity> where TEntity : IServerEntity
{
    TEntity this[int index] { get; }

    ITrackerManager<TEntity>? Manager { get; }
}
