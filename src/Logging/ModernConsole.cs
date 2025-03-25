using Amethyst.Logging;

namespace Amethyst.Logging;

public static class ModernConsole
{
    private static IReadOnlyDictionary<string, byte> ColorKeys = new Dictionary<string, byte>()
    {
        { "!r", 0 },
        { "!b", 1 },
        { "!d", 2 },
        { "!g", 2 },
        { "!i", 3 },
        { "!u", 4 },
        { "!c", 9 },

        { "r", 31 },
        { "g", 32 },
        { "y", 33 },
        { "b", 34 },
        { "m", 35 },
        { "c", 36 },
        { "w", 37 },

        { "@r", 41 },
        { "@g", 42 },
        { "@y", 43 },
        { "@b", 44 },
        { "@m", 45 },
        { "@c", 46 },
        { "@w", 47 },
    };

    internal static IReadOnlyDictionary<LogLevel, string> LevelPrefix = new Dictionary<LogLevel, string>
    {
        { LogLevel.Critical, "$r$!bCritical" },
        { LogLevel.Error, "$rError   " },
        { LogLevel.Warning, "$bWarning " },
        { LogLevel.Info, "$yInfo    " },
        { LogLevel.Verbose, "$!dVerbose " },
        { LogLevel.Debug, "$wDebug   " },
    }.AsReadOnly();

    public static void WriteLine(string text)
    {
        foreach (var col in ColorKeys)
        {
            text = text.Replace($"${col.Key}", $"\x1b[{col.Value}m");
        }
        
        text += "\x1b[0m";

        Console.WriteLine(text);
    }

    public static string ClearText(string text)
    {
        foreach (var col in ColorKeys)
        {
            text = text.Replace($"${col.Key}", string.Empty);
        }

        return text;
    }
}