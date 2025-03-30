namespace Amethyst.Extensions.Modules;

[AttributeUsage(AttributeTargets.Class)]
public sealed class AmethystModuleAttribute(string name, string[]? dependencies = null) : Attribute
{
    public string Name { get; } = name;
    public string[]? Dependencies { get; } = dependencies;
}
