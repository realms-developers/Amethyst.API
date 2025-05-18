using System.Runtime.Loader;
using Amethyst.Infrastructure.Kernel;

namespace Amethyst.Extensions.Modules;

public sealed class ModuleLoadContext : AssemblyLoadContext
{
    public static readonly ModuleLoadContext Instance = new();

    private ModuleLoadContext()
        : base("ApplicationModules", isCollectible: false)
    {
        // Resolve dependencies from shared context
        Resolving += (_, name) =>
            SharedDependencyContext.Instance.LoadFromAssemblyName(name);
    }
}
