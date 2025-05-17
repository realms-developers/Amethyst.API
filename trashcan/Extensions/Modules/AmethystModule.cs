namespace Amethyst.Extensions.Modules;

internal sealed class AmethystModule
{
    internal AmethystModule(string name, List<ModuleInitializeDelegate> initializeDelegate)
    {
        Name = name;
        InitializeDelegate = initializeDelegate;
    }

    internal readonly string Name;
    internal readonly List<ModuleInitializeDelegate> InitializeDelegate;
}
