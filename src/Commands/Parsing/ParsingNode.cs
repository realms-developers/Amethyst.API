using Amethyst.Commands.Arguments;
using Amethyst.Players;

namespace Amethyst.Commands.Parsing;

public static class ParsingNode
{
    private static readonly Dictionary<Type, ArgumentParser> _parsers = [];

    internal static void Initialize()
    {
        // Generic numeric parsers
        new[] { typeof(byte), typeof(int), typeof(double), typeof(float), typeof(bool) }
            .ForEach(t => _parsers.Add(t, CreateTryParseParser(t)));

        // String parser
        _parsers[typeof(string)] = (sender, input) => ParseResult.Success(input);

        // Specialized parsers
        _parsers[typeof(PlayerReference)] = ParsePlayerReference;
        _parsers[typeof(ItemReference)] = ParseItemReference;
    }

    private static ArgumentParser CreateTryParseParser(Type type) => (_, input) =>
        type.GetMethod("TryParse", [typeof(string), type.MakeByRefType()]) is { } tryParse &&
        (bool)tryParse.Invoke(null, [input, null])!
        ? ParseResult.Success(Convert.ChangeType(input, type.ReflectedType!, null))
        : ParseResult.InvalidSyntax("$ParsingNode.InvalidSyntax");

    private static ParseResult ParsePlayerReference(ICommandSender sender, string input)
    {
        if (input.StartsWith("$me", StringComparison.OrdinalIgnoreCase) && sender is NetPlayer netPlayer)
        {
            return ParseResult.Success(new PlayerReference(netPlayer.Index));
        }

        if (input.StartsWith('$') && byte.TryParse(input.AsSpan(1), out byte index))
        {
            return ParseResult.Success(new PlayerReference(index));
        }

        NetPlayer? player = PlayerManager.Tracker
            .Where(p => p?.IsActive == true)
            .FirstOrDefault(p => p.Name.Equals(input, StringComparison.OrdinalIgnoreCase)) ??
            PlayerManager.Tracker
            .FirstOrDefault(p => p?.IsActive == true && p.Name.StartsWith(input, StringComparison.OrdinalIgnoreCase));

        return player != null
            ? ParseResult.Success(new PlayerReference(player.Index))
            : ParseResult.ObjectNotFound();
    }

    private static ParseResult ParseItemReference(ICommandSender sender, string input)
    {
        return input.StartsWith("$held", StringComparison.OrdinalIgnoreCase) && sender is NetPlayer netPlayer
            ? ParseResult.Success(new ItemReference(netPlayer.Utils.HeldItem.type))
            : FindLocalizedItem(input, false)
            ?? FindLocalizedItem(input, true)
            ?? ParseResult.ObjectNotFound();
    }

    private static ParseResult? FindLocalizedItem(string input, bool isRussian)
    {
        List<Localization.ItemFindData> items = Localization.Items.FindItem(isRussian, input);

        return items.Count switch
        {
            1 => ParseResult.Success(new ItemReference(items[0].ItemID)),
            > 1 => ParseResult.TooManyVariants([.. items.Select(p => p.Name)]),
            _ => null
        };
    }

    internal static ParseResult TryParse(Type type, ICommandSender sender, string input) =>
        _parsers.TryGetValue(type, out ArgumentParser? parser)
            ? parser(sender, input)
            : new ParseResult(ParseResultType.NoParser, null, null, null);

    public static void AddParser(Type type, ArgumentParser parser) => _parsers.TryAdd(type, parser);
    public static void RemoveParser(Type type) => _parsers.Remove(type);
}

// Extension method for ForEach on IEnumerable<T>
public static class EnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (T? item in source)
        {
            action(item);
        }
    }
}
