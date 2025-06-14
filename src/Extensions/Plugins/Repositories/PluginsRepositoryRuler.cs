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
    }

    public PluginsRepositoryRuler(string identifier)
    {
        _pluginCfg = new($"{nameof(PluginsConfiguration)}.{identifier}", new());

        _pluginCfg.Load();
    }

    public void AllowExtension(string name)
    {
        if (_pluginCfg.Data.AllowedPlugins.Contains(name))
        {
            return;
        }

        _pluginCfg.Data.AllowedPlugins.Add(name);
        _pluginCfg.Save();
    }

    public bool DisallowExtension(string name)
    {
        bool wasRemoved = _pluginCfg.Data.AllowedPlugins.Remove(name);
        if (wasRemoved)
        {
            _pluginCfg.Save();
        }
        return wasRemoved;
    }

    public bool IsExtensionAllowed(string name) => _pluginCfg.Data.AllowedPlugins.Contains(name);

}
