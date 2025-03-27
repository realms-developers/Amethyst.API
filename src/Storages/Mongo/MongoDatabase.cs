using Amethyst.Core;
using MongoDB.Driver;

namespace Amethyst.Storages.Mongo;

public sealed class MongoDatabase
{
    public static MongoDatabase Main { get; } = new MongoDatabase(
        AmethystSession.StorageConfiguration.MongoConnection,
        AmethystSession.StorageConfiguration.MongoDatabaseName);

    public MongoDatabase(string uri, string name)
    {
        Uri = uri;
        Name = name;
        
        Client = new MongoClient(uri);
        InternalDb = Client.GetDatabase(name);
    }

    public string Uri { get; }
    public string Name { get; }

    public MongoClient Client { get; }
    public IMongoDatabase InternalDb { get; }
    
    public MongoModels<TModel> Get<TModel>(string? name = null) where TModel : DataModel
    {
        MongoModels<TModel> storage = new(this, name);
        return storage;
    }

}