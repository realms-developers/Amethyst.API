namespace Amethyst.Systems.Commands.Dynamic.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class CommandPermissionAttribute : Attribute
{
    public CommandPermissionAttribute(string permission)
    {
        Permission = permission;
    }

    public string Permission { get; }
}
