namespace Amethyst.Systems.Commands.Base.Metadata;

[Flags]
public enum CommandRules : byte
{
    None = 0,

    Disabled = 1,

    NoLogging = 2,

    Hidden = 4
}
