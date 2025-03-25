using System.Globalization;

namespace Amethyst.Commands.Parsing;

public static class ParsingNode
{
    private static Dictionary<Type, ArgumentParser> Parsers = new Dictionary<Type, ArgumentParser>();

    internal static void Initialize()
    {
        Parsers.Add(typeof(string), (sender, input) => {
            return ParseResult.Success(input);
        });

        Parsers.Add(typeof(bool), (sender, input) => {
            if (bool.TryParse(input, out bool value) == true)
                return ParseResult.Success(value);

            return ParseResult.InvalidSyntax("$ParsingNode.InvalidSyntax");
        });

        Parsers.Add(typeof(byte), (sender, input) => {
            if (byte.TryParse(input, out byte value) == true)
                return ParseResult.Success(value);

            return ParseResult.InvalidSyntax("$ParsingNode.InvalidSyntax");
        });

        Parsers.Add(typeof(int), (sender, input) => {
            if (int.TryParse(input, out int value) == true)
                return ParseResult.Success(value);

            return ParseResult.InvalidSyntax("$ParsingNode.InvalidSyntax");
        });

        Parsers.Add(typeof(double), (sender, input) => {
            if (double.TryParse(input, out double value) == true)
                return ParseResult.Success(value);

            return ParseResult.InvalidSyntax("$ParsingNode.InvalidSyntax");
        });

        Parsers.Add(typeof(float), (sender, input) => {
            if (float.TryParse(input, out float value) == true)
                return ParseResult.Success(value);

            return ParseResult.InvalidSyntax("$ParsingNode.InvalidSyntax");
        });
    }
    

    internal static ParseResult TryParse(Type type, ICommandSender sender, string input)
    {
        if (Parsers.ContainsKey(type) == false) return new ParseResult(ParseResultType.NoParser, null, null, null);

        var parser = Parsers[type];
        return parser(sender, input);
    }

    public static void AddParser(Type type, ArgumentParser parser) => Parsers.TryAdd(type, parser);
    public static void RemoveParser(Type type) => Parsers.Remove(type);
}