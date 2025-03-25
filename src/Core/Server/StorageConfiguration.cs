namespace Amethyst.Core.Server;

public struct StorageConfiguration
{
    public string MongoConnection;
    public string MongoDatabaseName;

    public string MySQLServer;
    public string MySQLDatabase;
    public string MySQLUid;
    public string MySQLPwd;

    public readonly string MySQLConnectionString => $"Server={MySQLServer};Database={MySQLDatabase};Uid={MySQLUid};Pwd={MySQLPwd};";
}
