using Amethyst.Kernel;
using Amethyst.Logging;

namespace Amethyst;

public static class AmethystLog
{
    /// <summary>
    /// Logger used for logging startup.
    /// </summary>
    public static ServerLogger Startup { get; } = new(Path.Combine(AmethystSession.Profile.SavePath, "startup.log"),
        AmethystSession.Profile.DebugMode ? LogLevel.Debug : LogLevel.Verbose);

    /// <summary>
    /// Logger used for logging main messages.
    /// </summary>
    public static ServerLogger Main { get; } = new(Path.Combine(AmethystSession.Profile.SavePath, "main.log"),
        AmethystSession.Profile.DebugMode ? LogLevel.Debug : LogLevel.Verbose);

    /// <summary>
    /// Logger used for logging security. Can be used in future SafeNetwork module-plugin complex.
    /// </summary>
    public static ServerLogger Security { get; } = new(Path.Combine(AmethystSession.Profile.SavePath, "security.log"),
        AmethystSession.Profile.DebugMode ? LogLevel.Debug : LogLevel.Verbose);

    /// <summary>
    /// Logger used for logging network.
    /// </summary>
    public static ServerLogger Network { get; } = new(Path.Combine(AmethystSession.Profile.SavePath, "network.log"),
        AmethystSession.Profile.DebugMode ? LogLevel.Debug : LogLevel.Verbose);

    /// <summary>
    /// Logger used for logging system changes. Can be used in future PermissionsEx plugin.
    /// </summary>
    public static ServerLogger System { get; } = new(Path.Combine(AmethystSession.Profile.SavePath, "system.log"),
        AmethystSession.Profile.DebugMode ? LogLevel.Debug : LogLevel.Verbose);
}
