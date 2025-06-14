using Amethyst.Extensions.Base.Repositories;
using Amethyst.Storages.Config;

namespace Amethyst.Extensions.Modules.Repositories;

public sealed class ModulesRepositoryRuler : IRepositoryRuler
{
    internal readonly Configuration<ModulesConfiguration> _modulesCfg;

    public IEnumerable<string> AllowedExtensions => _modulesCfg.Data.AllowedModules;

    public ModulesRepositoryRuler()
    {
        _modulesCfg = new(nameof(ModulesConfiguration), new());
        _modulesCfg.Load();
    }

    public ModulesRepositoryRuler(string identifier)
    {
        _modulesCfg = new($"{nameof(ModulesConfiguration)}.{identifier}", new());
        _modulesCfg.Load();
    }

    public void AllowExtension(string name)
    {
        if (_modulesCfg.Data.AllowedModules.Contains(name))
        {
            return;
        }

        _modulesCfg.Data.AllowedModules.Add(name);
        _modulesCfg.Save();
    }

    public bool DisallowExtension(string name)
    {
        bool wasRemoved = _modulesCfg.Data.AllowedModules.Remove(name);
        if (wasRemoved)
        {
            _modulesCfg.Save();
        }
        return wasRemoved;
    }

    public bool IsExtensionAllowed(string name) =>
        _modulesCfg.Data.AllowedModules.Contains(name);
}
