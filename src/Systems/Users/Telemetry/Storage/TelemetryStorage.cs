using Amethyst.Storages;
using Amethyst.Storages.Mongo;

namespace Amethyst.Systems.Users.Telemetry.Storage;

public static class TelemetryStorage
{
    public static MongoDatabase Database { get; } = new MongoDatabase(
        TelemetryConfiguration.Configuration.Data.MongoConnection ?? StorageConfiguration.Configuration.Data.MongoConnection,
        TelemetryConfiguration.Configuration.Data.MongoDatabaseName ?? StorageConfiguration.Configuration.Data.MongoDatabaseName
    );

    public static MongoModels<UserInfoModel> Users { get; } = Database.Get<UserInfoModel>();
}