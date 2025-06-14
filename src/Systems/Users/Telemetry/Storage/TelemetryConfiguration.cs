using Amethyst.Storages.Config;

namespace Amethyst.Systems.Users.Telemetry.Storage;

public sealed class TelemetryConfiguration
{
    static TelemetryConfiguration() => Configuration.Load();

    public static Configuration<TelemetryConfiguration> Configuration { get; } = new("TelemetryConfiguration", new());
    public static TelemetryConfiguration Instance => Configuration.Data;

    public string? MongoConnection { get; set; }
    public string? MongoDatabaseName { get; set; }
}
