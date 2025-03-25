using MongoDB.Bson.Serialization.Attributes;

namespace Amethyst.Storages.Mongo;

[BsonIgnoreExtraElements]
public abstract class DataModel
{
    public DataModel(string name)
    {
        Name = name;
    }
    
    public string Name;

    public abstract void Save();
    public abstract void Remove();
}