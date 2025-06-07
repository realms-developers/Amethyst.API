using Amethyst.Storages.Config;

namespace Amethyst.Network.Handling;

public sealed class HandlersConfiguration
{
    static HandlersConfiguration() => Configuration.Load();

    public static Configuration<HandlersConfiguration> Configuration { get; } = new(nameof(HandlersConfiguration), new());
    public static HandlersConfiguration Instance => Configuration.Data;


    public List<string> DisabledHandlers { get; set; } = [];

    public bool SyncPlayers { get; set; } = true;
}
