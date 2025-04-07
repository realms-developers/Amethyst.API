using System.Globalization;
using Amethyst.Logging;

namespace Amethyst.Core.Arguments.Implementations;

public static class CoreCommands
{
    [ArgumentCommand("-profile", "load server profile")]
    public static bool LoadProfile(string input)
    {
        AmethystKernel.Profile = new Profiles.ServerProfile(input);

        ModernConsole.WriteLine($"$!bLoaded server profile '{input}'.");
        return true;
    }

    [ArgumentCommand("-deflang", "set default server language")]
    public static bool DefaultLanguage(string input)
    {
        if (AmethystKernel.Profile == null)
        {
            ModernConsole.WriteLine("$rError: Please load a server profile first.");

            return false;
        }

        AmethystKernel.Profile.DefaultLanguage = input;

        ModernConsole.WriteLine($"$!bDefault server language set to: '{input}'.");
        return true;
    }

    [ArgumentCommand("-genevil", "set world evil in generation (-1 for random, 0 for corrupt, 1 for crimson)")]
    public static bool GenEvil(string input)
    {
        if (AmethystKernel.Profile == null)
        {
            ModernConsole.WriteLine("$rError: Please load a server profile first.");

            return false;
        }

        AmethystKernel.Profile.GenerationRules.Evil = Math.Clamp(int.Parse(input, CultureInfo.InvariantCulture), -1, 1);

        ModernConsole.WriteLine($"$!bWorld evil: {AmethystKernel.Profile!.GenerationRules.Evil}.");
        return true;
    }

    [ArgumentCommand("-gengamemode", "set world game-mode in generation (0 for classic, 1 for expert, 2 for master, 3 for creative)")]
    public static bool GenGameMode(string input)
    {
        if (AmethystKernel.Profile == null)
        {
            ModernConsole.WriteLine("$rError: Please load a server profile first.");

            return false;
        }

        AmethystKernel.Profile.GenerationRules.GameMode = Math.Clamp(int.Parse(input, CultureInfo.InvariantCulture), 0, 4);

        ModernConsole.WriteLine($"$!bWorld gamemode: {AmethystKernel.Profile!.GenerationRules.GameMode}.");
        return true;
    }

    [ArgumentCommand("-genwidth", "set world width in generation (4200/6400/8400)")]
    public static bool GenWorldWidth(string input)
    {
        if (AmethystKernel.Profile == null)
        {
            ModernConsole.WriteLine("$rError: Please load a server profile first.");

            return false;
        }

        AmethystKernel.Profile.GenerationRules.Width = int.Parse(input, CultureInfo.InvariantCulture);

        ModernConsole.WriteLine($"$!bWorld width: {AmethystKernel.Profile!.GenerationRules.Width}.");
        return true;
    }

    [ArgumentCommand("-genheight", "set world width in generation (1200/1800/2400)")]
    public static bool GenWorldHeight(string input)
    {
        if (AmethystKernel.Profile == null)
        {
            ModernConsole.WriteLine("$rError: Please load a server profile first.");

            return false;
        }

        AmethystKernel.Profile.GenerationRules.Height = int.Parse(input, CultureInfo.InvariantCulture);

        ModernConsole.WriteLine($"$!bWorld height: {AmethystKernel.Profile!.GenerationRules.Height}.");
        return true;
    }

    [ArgumentCommand("-worldpath", "set world path to load")]
    public static bool WorldPath(string input)
    {
        if (AmethystKernel.Profile == null)
        {
            ModernConsole.WriteLine("$rError: Please load a server profile first.");

            return false;
        }

        AmethystKernel.Profile.WorldToLoad = input;

        ModernConsole.WriteLine($"$!bSet world to load: {input}.");
        return true;
    }

    [ArgumentCommand("-worldrecreate", "recreate world")]
    public static bool WorldRecreate(string input)
    {
        if (AmethystKernel.Profile == null)
        {
            ModernConsole.WriteLine("$rError: Please load a server profile first.");

            return false;
        }

        AmethystKernel.Profile.WorldRecreate = bool.Parse(input);

        ModernConsole.WriteLine($"$!bRecreate world: {AmethystKernel.Profile!.WorldRecreate}.");
        return true;
    }

    [ArgumentCommand("-netport", "set listening port")]
    public static bool NetPort(string input)
    {
        if (AmethystKernel.Profile == null)
        {
            ModernConsole.WriteLine("$rError: Please load a server profile first.");

            return false;
        }

        AmethystKernel.Profile.Port = int.Parse(input, CultureInfo.InvariantCulture);

        ModernConsole.WriteLine($"$!bSet server port to {input}.");
        return true;
    }

    [ArgumentCommand("-netslots", "set max players count")]
    public static bool NetSlots(string input)
    {
        if (AmethystKernel.Profile == null)
        {
            ModernConsole.WriteLine("$rError: Please load a server profile first.");

            return false;
        }

        AmethystKernel.Profile.MaxPlayers = int.Parse(input, CultureInfo.InvariantCulture);

        ModernConsole.WriteLine($"$!bSet max players to {input}.");
        return true;
    }

    [ArgumentCommand("-debugmode", "set debug mode")]
    public static bool DebugMode(string input)
    {
        if (AmethystKernel.Profile == null)
        {
            ModernConsole.WriteLine("$rError: Please load a server profile first.");

            return false;
        }

        AmethystKernel.Profile.DebugMode = bool.Parse(input);

        ModernConsole.WriteLine($"$!bDebug mode: {AmethystKernel.Profile.DebugMode}.");
        return true;
    }

    [ArgumentCommand("-ssc", "set server side characters mode")]
    public static bool SSCMode(string input)
    {
        if (AmethystKernel.Profile == null)
        {
            ModernConsole.WriteLine("$rError: Please load a server profile first.");

            return false;
        }

        AmethystKernel.Profile.SSCMode = bool.Parse(input);

        ModernConsole.WriteLine($"$!bSSC mode: {AmethystKernel.Profile.SSCMode}.");
        return true;
    }
}
