namespace Amethyst.Systems.Commands.Dynamic.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class CommandPermissionAttribute(string permission) : Attribute
{
    public string Permission { get; } = permission;
}
