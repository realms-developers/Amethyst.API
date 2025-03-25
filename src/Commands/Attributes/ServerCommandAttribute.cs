namespace Amethyst.Commands;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ServerCommandAttribute : Attribute
{
    public ServerCommandAttribute(CommandType cmdType, string name, string description, string? permission)
    {   
        Type = cmdType;
        Name = name;
        Description = description;
        Permission = permission;
    }

    public readonly CommandType Type;
    public readonly string Name;
    public readonly string Description;
    public readonly string? Permission;
}
