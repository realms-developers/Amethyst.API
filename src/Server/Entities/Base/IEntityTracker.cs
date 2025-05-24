namespace Amethyst.Server.Entities.Base;

public interface IEntityTracker<TEntity> : IEnumerable<TEntity> where TEntity : IServerEntity
{
    TEntity this[int index] { get; }

    IEntityManager<TEntity>? Manager { get; }
}
