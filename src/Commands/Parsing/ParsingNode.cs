using Amethyst.Commands.Arguments;
using Amethyst.Players;

namespace Amethyst.Commands.Parsing;

public static class ParsingNode
{
    private static readonly Dictionary<Type, ArgumentParser> _parsers = [];

    internal static void Initialize()
    {
        // Generic numeric parsers - replaced ForEach with raw loop
        Type[] numericTypes = [typeof(byte), typeof(int), typeof(double), typeof(float), typeof(bool)];
        foreach (Type t in numericTypes)
        {
            _parsers.Add(t, CreateTryParseParser(t));
        }

        // String parser
        _parsers[typeof(string)] = (sender, input) => ParseResult.Success(input);

        // Specialized parsers
        _parsers[typeof(PlayerReference)] = ParsePlayerReference;
        _parsers[typeof(ItemReference)] = ParseItemReference;
    }

    private static ArgumentParser CreateTryParseParser(Type type) => (_, input) =>
        type.GetMethod("TryParse", [typeof(string), type.MakeByRefType()]) is { } tryParse &&
        (bool)tryParse.Invoke(null, [input, null])!
        ? ParseResult.Success(Convert.ChangeType(input, type, null))
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

        // Manual player search without LINQ
        NetPlayer? exactMatch = null;
        NetPlayer? prefixMatch = null;

        foreach (NetPlayer p in PlayerManager.Tracker)
        {
            if (p?.IsActive != true)
            {
                continue;
            }

            if (exactMatch == null && p.Name.Equals(input, StringComparison.OrdinalIgnoreCase))
            {
                exactMatch = p;
                break; // Found exact match, exit early
            }

            if (prefixMatch == null && p.Name.StartsWith(input, StringComparison.OrdinalIgnoreCase))
            {
                prefixMatch = p;
            }
        }

        NetPlayer? player = exactMatch ?? prefixMatch;
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

        if (items.Count == 1)
        {
            return ParseResult.Success(new ItemReference(items[0].ItemID));
        }

        if (items.Count > 1)
        {
            // Manual variant collection without LINQ Select
            List<string> variantNames = new(items.Count);
            foreach (Localization.ItemFindData item in items)
            {
                variantNames.Add(item.Name);
            }
            return ParseResult.TooManyVariants(variantNames);
        }

        return null;
    }

    internal static ParseResult TryParse(Type type, ICommandSender sender, string input) =>
        _parsers.TryGetValue(type, out ArgumentParser? parser)
            ? parser(sender, input)
            : new ParseResult(ParseResultType.NoParser, null, null, null);

    public static void AddParser(Type type, ArgumentParser parser) => _parsers.TryAdd(type, parser);
    public static void RemoveParser(Type type) => _parsers.Remove(type);
}
