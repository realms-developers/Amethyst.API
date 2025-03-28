namespace Amethyst.Commands.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class CommandsSettingsAttribute(CommandSettings settings) : Attribute
{
    public CommandSettings Settings { get; } = settings;
}
