using Amethyst.Extensions.Base.Repositories;
using Amethyst.Extensions.Base.Result;
using Amethyst.Extensions.Modules.Repositories;
using Amethyst.Extensions.Plugins.Repositories;

namespace Amethyst.Extensions;

public static class ExtensionsOrganizer
{
    public static RepositorySet Plugins { get; } = new RepositorySet();
    public static RepositorySet Modules { get; } = new RepositorySet();

    static ExtensionsOrganizer()
    {
        Modules.AddRepository(new ModulesRepository());
        Plugins.AddRepository(new PluginsRepository());
    }

    public static IEnumerable<ExtensionHandleResult> LoadModules() => [.. Modules.Repositories.SelectMany(repo => repo.LoadExtensions())];

    public static IEnumerable<ExtensionHandleResult> LoadPlugins() => [.. Plugins.Repositories.SelectMany(repo => repo.LoadExtensions())];

    public static IEnumerable<ExtensionHandleResult> UnloadPlugins() => [.. Plugins.Repositories.SelectMany(repo => repo.UnloadExtensions())];
}
