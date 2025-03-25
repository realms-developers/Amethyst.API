namespace Amethyst.Core.Arguments;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ArgumentCommandAttribute : Attribute
{
    public ArgumentCommandAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public readonly string Name;
    public readonly string Description;
}