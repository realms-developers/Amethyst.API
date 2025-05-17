using Amethyst.Extensions.Repositories;

namespace Amethyst.Extensions.Plugins;

public sealed class PluginsRepositoryRuler : IRepositoryRuler
{
    public bool AllowExtension(string name)
    {
        PluginsConfiguration.Configuration.Modify((ref PluginsConfiguration cfg) => cfg.AllowedPlugins.Add(name));
        return true;
    }

    public bool DisallowExtension(string name)
    {
        PluginsConfiguration.Configuration.Modify((ref PluginsConfiguration cfg) => cfg.AllowedPlugins.Remove(name));
        return true;
    }

    public IEnumerable<string> GetAllowedExtensions()
    {
        return PluginsConfiguration.Instance.AllowedPlugins;
    }

    public bool IsExtensionAllowed(string name)
    {
        return PluginsConfiguration.Instance.AllowedPlugins.Contains(name);
    }
}
