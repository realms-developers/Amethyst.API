using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Amethyst.Text;

public static class TextUtility
{
    private const int _minutes = 60;
    private const int _hours = 60 * _minutes;
    private const int _days = 24 * _hours;
    private const int _months = 30 * _days;
    private const int _years = 365 * _days;

    private static readonly Dictionary<char, int> TimeUnits = new()
    {
        ['s'] = 1,
        ['с'] = 1,          // seconds
        ['m'] = _minutes,
        ['м'] = _minutes, // minutes
        ['h'] = _hours,
        ['ч'] = _hours,    // hours
        ['d'] = _days,
        ['д'] = _days,      // days
        ['M'] = _months,
        ['М'] = _months,  // months
        ['y'] = _years,
        ['г'] = _years     // years
    };

    public static int ParseToSeconds(string input)
    {
        int time = 0;
        StringBuilder currentNumber = new();

        foreach (char c in input)
        {
            if (char.IsDigit(c))
            {
                currentNumber.Append(c);
                continue;
            }

            if (TimeUnits.TryGetValue(c, out int multiplier))
            {
                if (currentNumber.Length == 0)
                {
                    continue;
                }

                int value = int.Parse(currentNumber.ToString(), CultureInfo.InvariantCulture);
                time += value * multiplier;
                currentNumber.Clear();
            }
        }

        return time;
    }

    public static List<string> SplitArguments(string text)
    {
        List<string> args = [string.Empty];
        int currentArg = 0;
        bool inQuotes = false;
        bool escapeNext = false;

        foreach (char c in text)
        {
            if (escapeNext)
            {
                args[currentArg] += c;
                escapeNext = false;
                continue;
            }

            switch (c)
            {
                case '\\':
                    escapeNext = true;
                    break;

                case '"':
                    inQuotes = !inQuotes;
                    break;

                case ' ' when !inQuotes:
                    args.Add(string.Empty);
                    currentArg++;
                    break;

                default:
                    args[currentArg] += c;
                    break;
            }
        }

        args.RemoveAll(string.IsNullOrWhiteSpace);

        return args;
    }

    public static string RemoveColorTags(this StringBuilder builder) => builder.ToString().RemoveColorTags();
    public static string RemoveColorTags(this string text)
    {
        StringBuilder stringBuilder = new();
        int num = 0;
        foreach (object obj in ColorTagRegex.Matches(text))
        {
            Match match = (Match)obj;
            stringBuilder.Append(text.AsSpan(num, match.Index - num));
            stringBuilder.Append(match.Groups["text"].Value);
            num = match.Index + match.Length;
        }
        stringBuilder.Append(text.AsSpan(num, text.Length - num));
        return stringBuilder.ToString();
    }
    internal static readonly Regex ColorTagRegex = new("(?<!\\\\)\\[c(olor)?(\\/(?<options>[^:]+))?:(?<text>.+?)(?<!\\\\)\\]", RegexOptions.Compiled);

    public static string SelfHash(this string input)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(input);

        using HMACSHA256 hmac = new(bytes);
        byte[] hash = hmac.ComputeHash(bytes);

        return Convert.ToBase64String(hash);
    }
}
