namespace Amethyst.Extensions.Modules;

[AttributeUsage(AttributeTargets.Class)]
public sealed class AmethystModuleAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
