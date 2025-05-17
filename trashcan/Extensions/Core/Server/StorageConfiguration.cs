namespace Amethyst.Core.Server;

public struct StorageConfiguration
{
    public string MongoConnection { get; set; }
    public string MongoDatabaseName { get; set; }

    public string MySQLServer { get; set; }
    public string MySQLDatabase { get; set; }
    public string MySQLUid { get; set; }
    public string MySQLPwd { get; set; }

    public readonly string MySQLConnectionString => $"Server={MySQLServer};Database={MySQLDatabase};Uid={MySQLUid};Pwd={MySQLPwd};";
}
