using Amethyst.Extensions.Base.Repositories;
using Amethyst.Storages.Config;

namespace Amethyst.Extensions.Plugins.Repositories;

public sealed class PluginsRepositoryRuler : IRepositoryRuler
{
    internal readonly Configuration<PluginsConfiguration> _pluginCfg = null!;

    public IEnumerable<string> AllowedExtensions => _pluginCfg.Data.AllowedPlugins;

    public PluginsRepositoryRuler()
    {
        _pluginCfg = new(nameof(PluginsConfiguration), new());

        _pluginCfg.Load();

        if (_pluginCfg.Data.AllowedPlugins.Count == 0)
        {
            AllowExtension("ExamplePlugin1");
            AllowExtension("ExamplePlugin2");
        }
    }

    public PluginsRepositoryRuler(string identifier)
    {
        _pluginCfg = new($"{nameof(PluginsConfiguration)}.{identifier}", new());

        _pluginCfg.Load();
    }

    public void AllowExtension(string name)
    {
        if (_pluginCfg.Data.AllowedPlugins.Contains(name))
            return;

        _pluginCfg.Data.AllowedPlugins.Add(name);
    }

    public bool DisallowExtension(string name)
    {
        return _pluginCfg.Data.AllowedPlugins.Remove(name);
    }

    public bool IsExtensionAllowed(string name) => _pluginCfg.Data.AllowedPlugins.Contains(name);

}
