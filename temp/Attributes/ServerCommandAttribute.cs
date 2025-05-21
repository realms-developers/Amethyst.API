using Amethyst.Systems.Commands;

namespace Amethyst.Systems.Commands.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ServerCommandAttribute(CommandType cmdType, string name, string description, string? permission) : Attribute
{
    public CommandType Type { get; } = cmdType;
    public string Name { get; } = name;
    public string Description { get; } = description;
    public string? Permission { get; } = permission;
}
