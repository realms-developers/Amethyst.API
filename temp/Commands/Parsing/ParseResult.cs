namespace Amethyst.Systems.Commands.Parsing;

public sealed class ParseResult
{
    public static ParseResult InvalidSyntax(string? reason) => new(ParseResultType.InvalidSyntax, reason, null, null);
    public static ParseResult EmptyArgument() => new(ParseResultType.InvalidSyntax, null, null, null);
    public static ParseResult ObjectNotFound() => new(ParseResultType.ObjectNotFound, null, null, null);
    public static ParseResult TooManyVariants(List<string> variants) => new(ParseResultType.TooManyVariants, null, variants, null);
    public static ParseResult Success(object value) => new(ParseResultType.Success, null, null, value);

    internal ParseResult(ParseResultType type, string? reason, List<string>? variants, object? result)
    {
        Type = type;
        Reason = reason;
        Variants = variants;
        Result = result;
    }

    public ParseResultType Type { get; }
    public string? Reason { get; }
    public List<string>? Variants { get; }
    public object? Result { get; }
}
