namespace Amethyst.Commands.Parsing;

public enum ParseResultType
{
    Success,
    
    NoParser,
    InvalidSyntax,
    EmptyArgument,
    ObjectNotFound,
    TooManyVariants
}