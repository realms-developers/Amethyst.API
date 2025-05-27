namespace Amethyst.Server.Entities.Base;

public interface IEntityManager<TEntity> where TEntity : IServerEntity
{
    IEntityTracker<TEntity> Tracker { get; }

    void Insert(int index, TEntity entity);
    void Remove(int index);
}
