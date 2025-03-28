namespace Amethyst.Core.Arguments;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ArgumentCommandAttribute(string name, string description) : Attribute
{
    public string Name { get; } = name;
    public string Description { get; } = description;
}
