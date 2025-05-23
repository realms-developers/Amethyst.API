namespace Amethyst.Server.Entities.Base;

public interface IEntityTracker<TEntity> : IEnumerable<TEntity> where TEntity : IServerEntity
{
    TEntity this[int index] { get; }

    IEnumerable<TEntity> ActiveEntities { get; }

    ITrackerManager<TEntity>? Manager { get; }
}
