namespace Amethyst.Extensions.Modules;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ModuleInitializeAttribute : Attribute
{
    public ModuleInitializeAttribute()
    {
    }
}