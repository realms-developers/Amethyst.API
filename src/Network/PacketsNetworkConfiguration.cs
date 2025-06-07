using Amethyst.Storages.Config;

namespace Amethyst.Network;

public sealed class PacketsNetworkConfiguration
{
    static PacketsNetworkConfiguration() => Configuration.Load();

    public static Configuration<PacketsNetworkConfiguration> Configuration { get; } = new(typeof(PacketsNetworkConfiguration).FullName!, new());
    public static PacketsNetworkConfiguration Instance => Configuration.Data;

    public string? FakeWorldName { get; set; } = "change yo wld name in config";
    public int? FakeWorldID { get; set; }
}
