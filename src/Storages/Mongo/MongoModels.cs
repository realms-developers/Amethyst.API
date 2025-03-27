using System.Linq.Expressions;
using MongoDB.Bson;
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

    public TModel? Find(string name)
    {
        return Find(m => m.Name == name);
    }

    public TModel? Find(ObjectId id)
    {
        return Find(m => m.Id == id);
    }

    public TModel? Find(Expression<Func<TModel, bool>> predicate)
    {   
        var found = InternalCollection.Find(predicate);
        if (found.Any() == false) return null;
        return found.First();
    }

    public IEnumerable<TModel> FindAll()
    {
        return FindAll(m => true);
    }

    public IEnumerable<TModel> FindAll(Expression<Func<TModel, bool>> predicate)
    {
        var found = InternalCollection.Find(predicate);
        return found.ToEnumerable();
    }

    public bool Remove(string name)
    {
        return InternalCollection.DeleteOne(m => m.Name == name).DeletedCount > 0;
    }

    public bool Remove(Expression<Func<TModel, bool>> predicate)
    {
        return InternalCollection.DeleteMany(predicate).DeletedCount > 0;
    }

    public void Save(TModel model)
    {
        if (Find(model.Name) != null)
            InternalCollection.ReplaceOne(m => m.Id == model.Id || m.Name == model.Name, model);
        else
            InternalCollection.InsertOne(model);
    }
}