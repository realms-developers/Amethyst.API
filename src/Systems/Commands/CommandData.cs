using System.Reflection;

namespace Amethyst.Commands;

public record CommandData(Guid? PluginIdentifier, string Name, string Description, MethodInfo Method, CommandSettings Settings, CommandType Type, string? Permission, string[]? Syntax);
