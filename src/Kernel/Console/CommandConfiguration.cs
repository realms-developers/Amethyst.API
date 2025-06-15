using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Amethyst.Kernel.Profiles;

namespace Amethyst.Kernel.Console;

internal static class CommandConfiguration
{
    internal static RootCommand BuildRootCommand()
    {
        RootCommand rootCommand = new($"Amethyst Terraria Server API v{typeof(AmethystKernel).Assembly.GetName().Version}");

        // 1. Profile Command (Required)
        Option<string> profileOption = new(
            ["--profile", "-profile", "-p"],
            "Load server profile")
        {
            IsRequired = true
        };

        // 2. Other options
        Option<string> languageOption = new(
            ["--deflang", "-deflang"],
            "Set default server language");

        Option<string> commandPrefixOption = CreateCommandPrefixOption();

        Option<int> genEvilOption = CreateGenEvilOption();

        Option<int> genGameModeOption = CreateGenGameModeOption();

        Option<int> genWidthOption = CreateGenWidthOption();

        Option<int> genHeightOption = CreateGenHeightOption();

        Option<string> worldPathOption = new(
            ["--worldpath", "-worldpath", "-w"],
            "Set world path to load");

        Option<bool> worldRecreateOption = new(
            ["--worldrecreate", "-worldrecreate"],
            "Recreate world");

        Option<int> netPortOption = CreateNetPortOption();
        Option<int> netSlotsOption = CreateNetSlotsOption();

        Option<bool> debugOption = new(
            ["--debugmode", "-debugmode", "-debug", "--debug", "-d"],
            "Enable debug mode");

        Option<bool> sscOption = new(
            ["--ssc", "-ssc"],
            "Enable server-side characters");

        Option<bool> forceUpdateOption = new(
            ["--forceupdate", "-forceupdate"],
            "Force game update cycle.");

        Option<bool> noFrameDebugOption = new(
            ["--noframedebug", "-noframedebug"],
            "Disable GameUpdate stopwatch");

        // Add options individually
        rootCommand.AddOption(profileOption);
        rootCommand.AddOption(languageOption);
        rootCommand.AddOption(commandPrefixOption);
        rootCommand.AddOption(genEvilOption);
        rootCommand.AddOption(genGameModeOption);
        rootCommand.AddOption(genWidthOption);
        rootCommand.AddOption(genHeightOption);
        rootCommand.AddOption(worldPathOption);
        rootCommand.AddOption(worldRecreateOption);
        rootCommand.AddOption(netPortOption);
        rootCommand.AddOption(netSlotsOption);
        rootCommand.AddOption(debugOption);
        rootCommand.AddOption(sscOption);
        rootCommand.AddOption(forceUpdateOption);
        rootCommand.AddOption(noFrameDebugOption);

        // Set handler with all command implementations
        rootCommand.SetHandler(context =>
        {
            try
            {
                // Handle profile first
                string profile = context.ParseResult.GetValueForOption(profileOption)!;
                AmethystKernel.Profile = new ServerProfile(profile);
                //ModernConsole.WriteLine($"$!bLoaded server profile '{profile}'.");

                #region Handle Option

                HandleOptionWithProfile(context, languageOption, value =>
                {
                    AmethystKernel.Profile.DefaultLanguage = value;
                    //ModernConsole.WriteLine($"$!bDefault server language set to: '{value}'.");
                });

                HandleOptionWithProfile(context, commandPrefixOption, value =>
                {
                    AmethystKernel.Profile.CommandPrefix = value[0];
                    //ModernConsole.WriteLine($"$!bCommand prefix set to: '{value[0]}'.");
                });

                HandleOptionWithProfile(context, genEvilOption, value =>
                {
                    AmethystKernel.Profile.GenerationRules.Evil = value;
                    //ModernConsole.WriteLine($"$!bWorld evil: {value}.");
                });

                HandleOptionWithProfile(context, genGameModeOption, value =>
                {
                    AmethystKernel.Profile.GenerationRules.GameMode = value;
                    //ModernConsole.WriteLine($"$!bWorld gamemode: {value}.");
                });

                HandleOptionWithProfile(context, genWidthOption, value =>
                {
                    AmethystKernel.Profile.GenerationRules.Width = value;
                    //ModernConsole.WriteLine($"$!bWorld width: {value}.");
                });

                HandleOptionWithProfile(context, genHeightOption, value =>
                {
                    AmethystKernel.Profile.GenerationRules.Height = value;
                    //ModernConsole.WriteLine($"$!bWorld height: {value}.");
                });

                HandleOptionWithProfile(context, worldPathOption, value =>
                {
                    AmethystKernel.Profile.WorldToLoad = value;
                    //ModernConsole.WriteLine($"$!bSet world to load: {value}.");
                });

                HandleOptionWithProfile(context, worldRecreateOption, value =>
                {
                    AmethystKernel.Profile.WorldRecreate = value;
                    //ModernConsole.WriteLine($"$!bRecreate world: {value}.");
                });

                HandleOptionWithProfile(context, netPortOption, value =>
                {
                    AmethystKernel.Profile.Port = value;
                    //ModernConsole.WriteLine($"$!bSet server port to {value}.");
                });

                HandleOptionWithProfile(context, netSlotsOption, value =>
                {
                    AmethystKernel.Profile.MaxPlayers = value;
                    //ModernConsole.WriteLine($"$!bSet max players to {value}.");
                });

                HandleOptionWithProfile(context, debugOption, value =>
                {
                    AmethystKernel.Profile.DebugMode = value;
                    // if (value)
                    // {
                    //     ModernConsole.WriteLine("$bWarning: Debug mode enabled. Any player can have root access.");
                    // }
                });

                HandleOptionWithProfile(context, sscOption, value =>
                {
                    AmethystKernel.Profile.SSCMode = value;
                    //ModernConsole.WriteLine($"$!bSSC was {(value ? "enabled" : "disabled")}.");
                });

                HandleOptionWithProfile(context, forceUpdateOption, value =>
                {
                    AmethystKernel.Profile.ForceUpdate = value;
                    //ModernConsole.WriteLine($"$!bGame update was {(value ? "forced" : "online depended")}.");
                });

                HandleOptionWithProfile(context, noFrameDebugOption, value =>
                {
                    AmethystKernel.Profile.DisableFrameDebug = value;
                    //ModernConsole.WriteLine($"$!bFrame debug was {(value ? "disabled" : "enabled")}.");
                });

                #endregion
            }
            catch (Exception ex)
            {
                ModernConsole.WriteLine($"$rError: {ex.Message}");
                context.ExitCode = 1;
            }
        });

        return rootCommand;
    }

    private static void HandleOptionWithProfile<T>(
    InvocationContext context,
    Option<T> option,
    Action<T> handler)
    {
        if (AmethystKernel.Profile == null)
        {
            ModernConsole.WriteLine("$rError: Profile not loaded");
            context.ExitCode = 1;
            return;
        }

        if (context.ParseResult.HasOption(option))
        {
            handler(context.ParseResult.GetValueForOption(option)!);
        }
    }

    #region Create methods

    private static Option<string> CreateCommandPrefixOption()
    {
        Option<string> option = new(
            ["--cmdprefix", "-cmdprefix"],
            description: "Set command prefix character",
            getDefaultValue: () => "/");

        option.AddValidator(result =>
        {
            string value = result.GetValueOrDefault<string>() ?? "";
            if (string.IsNullOrEmpty(value) || value.Length != 1)
            {
                result.ErrorMessage = "Command prefix must be exactly one character";
            }
        });

        return option;
    }

    private static Option<int> CreateGenEvilOption()
    {
        Option<int> option = new(
            ["--genevil", "-genevil"],
            description: "Set world evil (-1 random, 0 corrupt, 1 crimson)",
            getDefaultValue: () => -1);

        option.AddValidator(result =>
        {
            int value = result.GetValueOrDefault<int>();
            if (value < -1 || value > 1)
            {
                result.ErrorMessage = "Must be -1, 0, or 1";
            }
        });

        option.AddCompletions("-1", "0", "1");
        return option;
    }

    private static Option<int> CreateGenGameModeOption()
    {
        Option<int> option = new(
            ["--gengamemode", "-gengamemode"],
            description: "Set world game-mode (0 classic, 1 expert, 2 master, 3 creative)",
            getDefaultValue: () => 0);

        option.AddValidator(result =>
        {
            int value = result.GetValueOrDefault<int>();
            if (value < 0 || value > 3)
            {
                result.ErrorMessage = "Must be between 0-3";
            }
        });

        option.AddCompletions("0", "1", "2", "3");
        return option;
    }

    private static Option<int> CreateGenWidthOption()
    {
        Option<int> option = new(
            ["--genwidth", "-genwidth"],
            description: "Set world width (4200/6400/8400)");

        option.AddValidator(result =>
        {
            int value = result.GetValueOrDefault<int>();
            if (value != 4200 && value != 6400 && value != 8400)
            {
                result.ErrorMessage = "Must be 4200, 6400, or 8400";
            }
        });

        option.AddCompletions("4200", "6400", "8400");
        return option;
    }

    private static Option<int> CreateGenHeightOption()
    {
        Option<int> option = new(
            ["--genheight", "-genheight"],
            description: "Set world height (1200/1800/2400)");

        option.AddValidator(result =>
        {
            int value = result.GetValueOrDefault<int>();
            if (value != 1200 && value != 1800 && value != 2400)
            {
                result.ErrorMessage = "Must be 1200, 1800, or 2400";
            }
        });

        option.AddCompletions("1200", "1800", "2400");
        return option;
    }

    private static Option<int> CreateNetPortOption()
    {
        Option<int> option = new(
            ["--netport", "-netport"],
            description: "Set listening port",
            getDefaultValue: () => 7777);

        option.AddValidator(result =>
        {
            int value = result.GetValueOrDefault<int>();
            if (value < 1 || value > 65535)
            {
                result.ErrorMessage = "Port must be 1-65535";
            }
        });

        return option;
    }

    private static Option<int> CreateNetSlotsOption()
    {
        Option<int> option = new(
            ["--netslots", "-netslots"],
            description: "Set max players count",
            getDefaultValue: () => 8);

        option.AddValidator(result =>
        {
            int value = result.GetValueOrDefault<int>();
            if (value < 1 || value > 255)
            {
                result.ErrorMessage = "Must be 1-255";
            }
        });

        return option;
    }

    #endregion
}
