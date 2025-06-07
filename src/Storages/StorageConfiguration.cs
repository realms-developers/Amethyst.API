using Amethyst.Storages.Config;

namespace Amethyst.Storages;

public sealed class StorageConfiguration
{
    static StorageConfiguration() => Configuration.Load();

    public static Configuration<StorageConfiguration> Configuration { get; } = new(nameof(StorageConfiguration), new());
    public static StorageConfiguration Instance => Configuration.Data;

    public string MongoConnection { get; set; } = "mongodb://localhost:27017";
    public string MongoDatabaseName { get; set; } = "amethyst";

    public string MySQLServer { get; set; } = "localhost";
    public string MySQLDatabase { get; set; } = "amethyst";
    public string MySQLUid { get; set; } = "root";
    public string MySQLPwd { get; set; } = string.Empty;

    public string MySQLConnectionString => $"Server={MySQLServer};Database={MySQLDatabase};Uid={MySQLUid};Pwd={MySQLPwd};";
}
