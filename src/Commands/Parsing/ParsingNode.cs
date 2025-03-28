using Amethyst.Commands.Arguments;
using Amethyst.Players;

namespace Amethyst.Commands.Parsing;

public static class ParsingNode
{
    private static readonly Dictionary<Type, ArgumentParser> _parsers = [];

    internal static void Initialize()
    {
        _parsers.Add(typeof(string), (sender, input) =>
        {
            return ParseResult.Success(input);
        });

        _parsers.Add(typeof(bool), (sender, input) =>
        {
            return bool.TryParse(input, out bool value) == true
                ? ParseResult.Success(value)
                : ParseResult.InvalidSyntax("$ParsingNode.InvalidSyntax");
        });

        _parsers.Add(typeof(byte), (sender, input) =>
        {
            return byte.TryParse(input, out byte value) == true
                ? ParseResult.Success(value)
                : ParseResult.InvalidSyntax("$ParsingNode.InvalidSyntax");
        });

        _parsers.Add(typeof(int), (sender, input) =>
        {
            return int.TryParse(input, out int value) == true
                ? ParseResult.Success(value)
                : ParseResult.InvalidSyntax("$ParsingNode.InvalidSyntax");
        });

        _parsers.Add(typeof(double), (sender, input) =>
        {
            return double.TryParse(input, out double value) == true
                ? ParseResult.Success(value)
                : ParseResult.InvalidSyntax("$ParsingNode.InvalidSyntax");
        });

        _parsers.Add(typeof(float), (sender, input) =>
        {
            return float.TryParse(input, out float value) == true
                ? ParseResult.Success(value)
                : ParseResult.InvalidSyntax("$ParsingNode.InvalidSyntax");
        });

        _parsers.Add(typeof(PlayerReference), (sender, input) =>
        {
            if (input.StartsWith("$me", StringComparison.InvariantCultureIgnoreCase) && sender is NetPlayer)
            {
                return ParseResult.Success(new PlayerReference((sender as NetPlayer)!.Index));
            }

            if (input.StartsWith('$') && byte.TryParse(input.AsSpan(1), out byte index))
            {
                return ParseResult.Success(index);
            }

            NetPlayer? plr = PlayerManager.Tracker.FirstOrDefault((p) => p != null && p.IsActive && p.Name.Equals(input, StringComparison.OrdinalIgnoreCase)) ??
                      PlayerManager.Tracker.FirstOrDefault((p) => p != null && p.IsActive && p.Name.StartsWith(input, StringComparison.InvariantCultureIgnoreCase) == true);

            return plr != null ? ParseResult.Success(new PlayerReference(plr.Index)) : ParseResult.ObjectNotFound();
        });

        _parsers.Add(typeof(ItemReference), (sender, input) =>
        {
            if (input.StartsWith("$held", StringComparison.InvariantCultureIgnoreCase) && sender is NetPlayer)
            {
                return ParseResult.Success(new ItemReference((sender as NetPlayer)!.Utils.HeldItem.type));
            }

            List<Localization.ItemFindData> enList = Localization.Items.FindItem(false, input);

            if (enList.Count == 1)
            {
                return ParseResult.Success(new ItemReference(enList[0].ItemID));
            }
            else if (enList.Count > 1)
            {
                return ParseResult.TooManyVariants([.. enList.Select(p => p.Name)]);
            }

            List<Localization.ItemFindData> ruList = Localization.Items.FindItem(true, input);

            if (ruList.Count == 1)
            {
                return ParseResult.Success(new ItemReference(ruList[0].ItemID));
            }
            else if (ruList.Count > 1)
            {
                return ParseResult.TooManyVariants([.. ruList.Select(p => p.Name)]);
            }

            return ParseResult.ObjectNotFound();
        });
    }


    internal static ParseResult TryParse(Type type, ICommandSender sender, string input)
    {
        if (_parsers.ContainsKey(type) == false)
        {
            return new ParseResult(ParseResultType.NoParser, null, null, null);
        }

        ArgumentParser parser = _parsers[type];

        return parser(sender, input);
    }

    public static void AddParser(Type type, ArgumentParser parser) => _parsers.TryAdd(type, parser);
    public static void RemoveParser(Type type) => _parsers.Remove(type);
}
