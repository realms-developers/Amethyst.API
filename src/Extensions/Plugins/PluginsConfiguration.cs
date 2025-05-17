using Amethyst.Storages.Config;

namespace Amethyst.Extensions.Plugins;

public sealed class PluginsConfiguration
{
    static PluginsConfiguration()
    {
        Configuration.Load();
    }

    public static Configuration<PluginsConfiguration> Configuration { get; } = new Configuration<PluginsConfiguration>("Plugins", new PluginsConfiguration());
    public static PluginsConfiguration Instance => Configuration.Data;

    public List<string> AllowedPlugins { get; set; } = [ "ExamplePlugin1", "ExamplePlugin2" ];
}
