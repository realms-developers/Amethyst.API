namespace Amethyst.Server.Entities.Base;

public interface IEntityModule<TEntity, TOperations>
    where TEntity : IServerEntity
    where TOperations : class
{
    TEntity BaseEntity { get; }
    TOperations Operations { get; }
}
