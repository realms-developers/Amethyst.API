using System.Linq.Expressions;
using MongoDB.Driver;

namespace Amethyst.Storages.Mongo;

public sealed class MongoModels<TModel> where TModel : DataModel
{
    internal MongoModels(MongoDatabase db, string? name = null)
    {
        CollectionName = name ?? $"{typeof(TModel).Name}Collection";
        Database = db;
        InternalCollection = Database.InternalDb.GetCollection<TModel>(CollectionName);
    }

    public string CollectionName { get; }
    public MongoDatabase Database { get; }
    public IMongoCollection<TModel> InternalCollection { get; }

    public TModel? Find(string name) => Find(m => m.Name == name);

    public TModel? Find(Expression<Func<TModel, bool>> predicate)
    {
        IFindFluent<TModel, TModel> found = InternalCollection.Find(predicate);
        return !found.Any() ? null : found.First();
    }

    public IEnumerable<TModel> FindAll() => FindAll(m => true);

    public IEnumerable<TModel> FindAll(Expression<Func<TModel, bool>> predicate)
    {
        IFindFluent<TModel, TModel> found = InternalCollection.Find(predicate);
        return found.ToEnumerable();
    }

    public bool Remove(string name) => InternalCollection.DeleteOne(m => m.Name == name).DeletedCount > 0;

    public bool Remove(Expression<Func<TModel, bool>> predicate) => InternalCollection.DeleteMany(predicate).DeletedCount > 0;

    public void Save(TModel model)
    {
        if (Find(model.Name) != null)
        {
            InternalCollection.ReplaceOne(m => m.Name == model.Name, model);
        }
        else
        {
            InternalCollection.InsertOne(model);
        }
    }
}
