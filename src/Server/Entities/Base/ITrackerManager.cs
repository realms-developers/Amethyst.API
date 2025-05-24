namespace Amethyst.Server.Entities.Base;

public interface ITrackerManager<TEntity> where TEntity : IServerEntity
{
    IEntityTracker<TEntity> Tracker { get; }

    void AttachHooks();
    void DeattachHooks();

    void Insert(int index, TEntity entity);
    void Remove(int index);
}
