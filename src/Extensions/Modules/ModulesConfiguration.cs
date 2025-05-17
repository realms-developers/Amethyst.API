using Amethyst.Storages.Config;

namespace Amethyst.Extensions.Modules;

public sealed class ModulesConfiguration
{
    static ModulesConfiguration()
    {
        Configuration.Load();
    }

    public static Configuration<ModulesConfiguration> Configuration { get; } = new Configuration<ModulesConfiguration>("Modules", new ModulesConfiguration());
    public static ModulesConfiguration Instance => Configuration.Data;

    public List<string> AllowedModules { get; set; } = [ "ExampleModule1", "ExampleModule2" ];
}
