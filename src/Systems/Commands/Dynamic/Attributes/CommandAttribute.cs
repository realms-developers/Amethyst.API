namespace Amethyst.Systems.Commands.Dynamic.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class CommandAttribute : Attribute
{
    public CommandAttribute(string[] names, string description)
    {
        Names = names;
        Description = description;
    }

    public string[] Names { get; }
    public string Description { get; }
}
