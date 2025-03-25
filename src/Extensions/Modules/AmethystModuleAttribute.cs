namespace Amethyst.Extensions.Modules;

[AttributeUsage(AttributeTargets.Class)]
public sealed class AmethystModuleAttribute : Attribute
{
    public AmethystModuleAttribute(string name, string[]? dependencies = null)
    {
        Name = name;
        Dependencies = dependencies;
    }

    public readonly string Name;
    public readonly string[]? Dependencies;
}