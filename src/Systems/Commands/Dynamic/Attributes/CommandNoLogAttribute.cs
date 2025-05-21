namespace Amethyst.Systems.Commands.Dynamic.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class CommandNoLogAttribute : Attribute
{
    public CommandNoLogAttribute() {}
}
