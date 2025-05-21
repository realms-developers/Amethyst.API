namespace Amethyst.Systems.Commands.Base.Metadata;

public record class CommandMetadata(string[] Names, string Description, CommandSyntax syntax, CommandRules rules, string? Permission);
