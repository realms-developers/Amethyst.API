namespace Amethyst.Commands;

[AttributeUsage(AttributeTargets.Method)]
public sealed class CommandsSettingsAttribute : Attribute
{
    public CommandsSettingsAttribute(CommandSettings settings)
    {   
        Settings = settings;
    }

    public readonly CommandSettings Settings;
}
