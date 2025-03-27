using System.Data.Entity.Infrastructure.Design;
using System.Globalization;
using System.Transactions;
using Amethyst.Commands.Arguments;
using Amethyst.Players;

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

        Parsers.Add(typeof(PlayerReference), (sender, input) => {
            if (input.StartsWith("$me", StringComparison.InvariantCultureIgnoreCase) && sender is NetPlayer)
                return ParseResult.Success(new PlayerReference((sender as NetPlayer)!.Index));

            if (input.StartsWith('$') && byte.TryParse(input.AsSpan(1), out byte index))
                return ParseResult.Success(index);

            var plr = PlayerManager.Tracker.FirstOrDefault((p) => p != null && p.IsActive && p.Name.Equals(input, StringComparison.OrdinalIgnoreCase)) ?? 
                      PlayerManager.Tracker.FirstOrDefault((p) => p != null && p.IsActive && p.Name.StartsWith(input, StringComparison.InvariantCultureIgnoreCase) == true);

            if (plr != null)
                return ParseResult.Success(new PlayerReference(plr.Index));

            return ParseResult.ObjectNotFound();
        });

        Parsers.Add(typeof(ItemReference), (sender, input) => {
            if (input.StartsWith("$held", StringComparison.InvariantCultureIgnoreCase) && sender is NetPlayer)
                return ParseResult.Success(new ItemReference((sender as NetPlayer)!.Utils.HeldItem.type));

            var enList = Localization.Items.FindItem(false, input);
            if (enList.Count == 1)
                return ParseResult.Success(new ItemReference(enList[0].ItemID));
            else if (enList.Count > 1)
                return ParseResult.TooManyVariants(enList.Select(p => p.Name).ToList());

            var ruList = Localization.Items.FindItem(true, input);
            if (ruList.Count == 1)
                return ParseResult.Success(new ItemReference(ruList[0].ItemID));
            else if (ruList.Count > 1)
                return ParseResult.TooManyVariants(ruList.Select(p => p.Name).ToList());

            return ParseResult.ObjectNotFound();
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