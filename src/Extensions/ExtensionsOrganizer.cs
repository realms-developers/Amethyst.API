using Amethyst.Extensions.Modules.Repositories;
using Amethyst.Extensions.Plugins.Repositories;
using Amethyst.Extensions.Repositories;
using Amethyst.Extensions.Result;

namespace Amethyst.Extensions;

public static class ExtensionsOrganizer
{
    public static RepositorySet Plugins { get; } = new RepositorySet();
    public static RepositorySet Modules { get; } = new RepositorySet();

    internal static void Initialize()
    {
        Modules.AddRepository(new ModulesRepository());
        Plugins.AddRepository(new PluginsRepository());
    }

    public static IEnumerable<ExtensionHandleResult> LoadModules()
    {
        foreach (var repo in Modules.Repositories)
        {
            foreach (var result in repo.LoadExtensions())
            {
                yield return result;
            }
        }
    }

    public static IEnumerable<ExtensionHandleResult> LoadPlugins()
    {
        foreach (var repo in Plugins.Repositories)
        {
            foreach (var result in repo.LoadExtensions())
            {
                yield return result;
            }
        }
    }

    public static IEnumerable<ExtensionHandleResult> UnloadPlugins()
    {
        foreach (var repo in Plugins.Repositories)
        {
            foreach (var result in repo.UnloadExtensions())
            {
                yield return result;
            }
        }
    }
}
