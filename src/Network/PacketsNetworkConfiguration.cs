using Amethyst.Storages.Config;

namespace Amethyst.Network;

public sealed class PacketsNetworkConfiguration
{
    static PacketsNetworkConfiguration() => Configuration.Load();

    public static Configuration<PacketsNetworkConfiguration> Configuration { get; } = new(nameof(PacketsNetworkConfiguration), new());
    public static PacketsNetworkConfiguration Instance => Configuration.Data;

    public string? FakeWorldName { get; set; } = "Amethyst.Server";
    public int? FakeWorldID { get; set; }
}
