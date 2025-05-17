using Amethyst.Extensions.Repositories;

namespace Amethyst.Extensions.Modules;

public sealed class ModulesRepositoryRuler : IRepositoryRuler
{
    public bool AllowExtension(string name)
    {
        ModulesConfiguration.Configuration.Modify((ref ModulesConfiguration cfg) => cfg.AllowedModules.Add(name));
        return true;
    }

    public bool DisallowExtension(string name)
    {
        ModulesConfiguration.Configuration.Modify((ref ModulesConfiguration cfg) => cfg.AllowedModules.Remove(name));
        return true;
    }

    public IEnumerable<string> GetAllowedExtensions()
    {
        return ModulesConfiguration.Instance.AllowedModules;
    }

    public bool IsExtensionAllowed(string name)
    {
        return ModulesConfiguration.Instance.AllowedModules.Contains(name);
    }
}
