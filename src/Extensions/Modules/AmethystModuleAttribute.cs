namespace Amethyst.Extensions.Modules;

[AttributeUsage(AttributeTargets.Class)]
public sealed class AmethystModuleAttribute : Attribute
{
    [Obsolete("Use ModuleInitializeAttribute without name instead.")]
    public AmethystModuleAttribute(string name)
    {
    }

    public AmethystModuleAttribute()
    {
    }
}
