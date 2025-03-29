using System.Reflection;

namespace Amethyst.Extensions.Modules;

internal sealed class AmethystModule
{
    internal AmethystModule(string name, string[]? dependencies, List<ModuleInitializeDelegate> initializeDelegate)
    {
        Name = name;
        Dependencies = dependencies;
        InitializeDelegate = initializeDelegate;
    }

    internal readonly string Name;
    internal readonly string[]? Dependencies;
    internal readonly List<ModuleInitializeDelegate> InitializeDelegate;

    internal void LoadDependencies()
    {
        if (Dependencies == null)
        {
            return;
        }

        foreach (string dep in Dependencies)
        {
            Assembly.LoadFrom(dep);
        }
    }
}
