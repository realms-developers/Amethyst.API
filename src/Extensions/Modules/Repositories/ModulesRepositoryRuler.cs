using Amethyst.Extensions.Base.Repositories;
using Amethyst.Storages.Config;

namespace Amethyst.Extensions.Modules.Repositories;

public sealed class ModulesRepositoryRuler : IRepositoryRuler
{
    internal readonly Configuration<ModulesConfiguration> _modulesCfg;

    public IEnumerable<string> AllowedExtensions => _modulesCfg.Data.AllowedModules;

    public ModulesRepositoryRuler()
    {
        _modulesCfg = new(typeof(ModulesConfiguration).FullName!, new());
        _modulesCfg.Load();

        if (_modulesCfg.Data.AllowedModules.Count == 0)
        {
            AllowExtension("ExampleModule1");
            AllowExtension("ExampleModule2");
        }
    }

    public ModulesRepositoryRuler(string identifier)
    {
        _modulesCfg = new($"{typeof(ModulesConfiguration).FullName!}.{identifier}", new());
        _modulesCfg.Load();
    }

    public bool ToggleExtension(string name)
    {
        bool isNowAllowed = false;

        _modulesCfg.Modify((ref cfg) =>
        {
            isNowAllowed = !cfg.AllowedModules.Remove(name);

            if (isNowAllowed)
            {
                cfg.AllowedModules.Add(name);
            }
        });

        return isNowAllowed;
    }

    public void AllowExtension(string name) =>
        _modulesCfg.Modify((ref cfg) => cfg.AllowedModules.Add(name));

    public bool DisallowExtension(string name)
    {
        bool removed = false;
        _modulesCfg.Modify((ref cfg) => removed = cfg.AllowedModules.Remove(name));
        return removed;
    }

    public bool IsExtensionAllowed(string name) =>
        _modulesCfg.Data.AllowedModules.Contains(name);
}
