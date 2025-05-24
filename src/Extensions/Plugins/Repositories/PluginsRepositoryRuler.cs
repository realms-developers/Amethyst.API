using Amethyst.Extensions.Base.Repositories;
using Amethyst.Storages.Config;

namespace Amethyst.Extensions.Plugins.Repositories;

public sealed class PluginsRepositoryRuler : IRepositoryRuler
{
    internal readonly Configuration<PluginsConfiguration> _pluginCfg = null!;

    public IEnumerable<string> AllowedExtensions => _pluginCfg.Data.AllowedPlugins;

    public PluginsRepositoryRuler()
    {
        _pluginCfg = new(typeof(PluginsConfiguration).FullName!, new());

        _pluginCfg.Load();

        if (_pluginCfg.Data.AllowedPlugins.Count == 0)
        {
            AllowExtension("ExamplePlugin1");
            AllowExtension("ExamplePlugin2");
        }
    }

    public PluginsRepositoryRuler(string identifier)
    {
        _pluginCfg = new($"{typeof(PluginsConfiguration).FullName!}.{identifier}", new());

        _pluginCfg.Load();
    }

    public bool ToggleExtension(string name)
    {
        bool isNowAllowed = false;

        _pluginCfg.Modify((ref cfg) =>
        {
            isNowAllowed = !cfg.AllowedPlugins.Remove(name);

            if (isNowAllowed)
            {
                cfg.AllowedPlugins.Add(name);
            }
        });

        return isNowAllowed;
    }

    public void AllowExtension(string name) => _pluginCfg.Modify((ref cfg) => cfg.AllowedPlugins.Add(name));

    public bool DisallowExtension(string name)
    {
        bool removed = false;

        _pluginCfg.Modify((ref cfg) =>
        {
            removed = cfg.AllowedPlugins.Remove(name);
        });

        return removed;
    }

    public bool IsExtensionAllowed(string name) => _pluginCfg.Data.AllowedPlugins.Contains(name);

}
