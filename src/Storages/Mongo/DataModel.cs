using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Amethyst.Storages.Mongo;

public abstract class DataModel(string name)
{
    [BsonId]
    public string Name { get; set; } = name;

    public abstract void Save();
    public abstract void Remove();
}
