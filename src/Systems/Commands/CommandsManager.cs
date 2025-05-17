using System.Reflection;
using Amethyst.Commands.Attributes;
using Amethyst.Commands.Implementations;
using Amethyst.Commands.Parsing;
using Amethyst.Core;
using Amethyst.Core.Server;
using Amethyst.Extensions.Plugins;
using Microsoft.Extensions.Configuration;

namespace Amethyst.Commands;

public static class CommandsManager
{
    internal const string LanguageCFG = "meta.ini";

    public const char CommandPrefix = '/';

    internal static List<CommandRunner> Commands { get; } = [];
    public static ICommandSender ConsoleSender { get; } = new ConsoleSender();

    internal static void Initialize()
    {
        ParsingNode.Initialize();
        ImportCommands(typeof(CommandsManager).Assembly, null);
        CreateLanguageCommands();

        PluginLoader.OnPluginLoad += OnPluginLoad;
        PluginLoader.OnPluginUnload += OnPluginUnload;
        ConsoleInput.OnConsoleInput += OnConsoleInput;
    }

    private static void OnConsoleInput(string input, ref bool handled) =>
        handled |= RequestRun(ConsoleSender, input);

    public static bool RequestRun(ICommandSender sender, string text)
    {
        string commandText = text.StartsWith(CommandPrefix) ? text[1..] : text;
        CommandRunner? runner = FindCommand(commandText);

        if (runner is null)
        {
            sender.ReplyError("commands.commandNotFound");
            return false;
        }

        LogCommandExecution(sender, commandText, runner);
        return ValidateAndExecuteCommand(sender, commandText, runner);
    }

    private static void LogCommandExecution(ICommandSender sender, string text, CommandRunner runner)
    {
        string logMessage = runner.Data.Settings.HasFlag(CommandSettings.HideLog)
            ? $"{sender.Name} [{sender.Type}]: $!r$!d/$!r$!b$r{runner.Data.Name} $!r$!d(hidden log)"
            : $"{sender.Name} [{sender.Type}]: $!r$!d/$!r$!b$r{text}";

        AmethystLog.System.Verbose(nameof(CommandsManager), logMessage);
    }

    private static bool ValidateAndExecuteCommand(ICommandSender sender, string text, CommandRunner runner)
    {
        (bool IsValid, Action HandleError)[] validationResults =
        [
            ValidateDebugMode(sender, runner),
            ValidateIngameOnly(sender, runner),
            ValidatePermissions(sender, runner),
            ValidateRCON(sender, runner)
        ];

        for (int i = 0; i < validationResults.Length; i++)
        {
            if (!validationResults[i].IsValid)
            {
                validationResults[i].HandleError();
                return true;
            }
        }

        try
        {
            string arguments = text.Substring(runner.Data.Name.Length).TrimStart();
            runner.Run(sender, arguments, runner.Data.Name);
            return true;
        }
        catch (Exception ex)
        {
            HandleCommandException(sender, text, ex);
            return true;
        }
    }

    private static (bool IsValid, Action HandleError) ValidateDebugMode(ICommandSender sender, CommandRunner runner) =>
        (runner.Data.Type != CommandType.Debug || AmethystSession.Profile.DebugMode,
        () => sender.ReplyError("commands.noDebugMode"));

    private static (bool IsValid, Action HandleError) ValidateIngameOnly(ICommandSender sender, CommandRunner runner) =>
        (!runner.Data.Settings.HasFlag(CommandSettings.IngameOnly) || sender.Type == SenderType.RealPlayer,
        () => sender.ReplyError("commands.ingameOnly"));

    private static (bool IsValid, Action HandleError) ValidateRCON(ICommandSender sender, CommandRunner runner) =>
        (runner.Data.Type != CommandType.Console || sender.Type == SenderType.Console,
        () => sender.ReplyError("commands.rconOnly"));

    private static (bool IsValid, Action HandleError) ValidatePermissions(ICommandSender sender, CommandRunner runner) =>
        (runner.Data.Permission is null || sender.HasPermission(runner.Data.Permission),
        () => sender.ReplyError("commands.noPermission"));

    private static void HandleCommandException(ICommandSender sender, string text, Exception ex)
    {
        sender.ReplyError("commands.commandFailed", Localization.Get(ex.Message, sender.Language));

        AmethystLog.System.Critical(nameof(CommandsManager), $"Command failure '{text}' from {sender.Name} ({sender.Type}):\n{ex}");
    }

    public static CommandRunner? FindCommand(string name)
    {
        foreach (CommandRunner cmd in Commands)
        {
            if (cmd.NameEquals(name))
            {
                return cmd;
            }
        }
        return null;
    }

    private static void OnPluginUnload(PluginContainer container) =>
        container.PluginInstance!.UnregisterCommands();

    private static void OnPluginLoad(PluginContainer container) =>
        ImportCommands(container.Assembly, container.LoadID);

    private static void CreateLanguageCommands()
    {
        string[] cultureDirectories = Directory.GetDirectories(Localization.Directory);

        foreach (string cultureDirectory in cultureDirectories)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddIniFile(Path.Combine(cultureDirectory, LanguageCFG))
                .Build();

            CommandData data = new(null, $"lang {new DirectoryInfo(cultureDirectory).Name}", config["meta:description"]!,
                typeof(BasicCommands).GetMethod(nameof(BasicCommands.Language))!, 0, CommandType.Shared, null, null);

            Commands.Add(new(data));
        }
    }

    internal static void ImportCommands(Assembly assembly, int? pluginId)
    {
        foreach (CommandData cmd in LoadCommands(assembly, pluginId))
        {
            Commands.Add(new(cmd));
        }
    }

    internal static IEnumerable<CommandData> LoadCommands(Assembly assembly, int? pluginId)
    {
        var commands = new List<CommandData>();
        Type[] exportedTypes = assembly.GetExportedTypes();

        foreach (Type type in exportedTypes)
        {
            MethodInfo[] methods = type.GetMethods();

            foreach (MethodInfo method in methods)
            {
                ServerCommandAttribute? commandAttr = method.GetCustomAttribute<ServerCommandAttribute>();
                CommandsSyntaxAttribute? syntaxAttr = method.GetCustomAttribute<CommandsSyntaxAttribute>();
                CommandsSettingsAttribute? settingsAttr = method.GetCustomAttribute<CommandsSettingsAttribute>();

                if (commandAttr != null && HasValidParameters(method))
                {
                    commands.Add(CreateCommandData(
                        (method, commandAttr, syntaxAttr, settingsAttr),
                        pluginId
                    ));
                }
            }
        }

        return commands;
    }

    private static bool HasValidParameters(MethodInfo method)
    {
        ParameterInfo[] parameters = method.GetParameters();
        return parameters.Length > 0 && parameters[0].ParameterType == typeof(CommandInvokeContext);
    }

    public static CommandData CreateCommandData(
        (MethodInfo Method, ServerCommandAttribute? CommandAttr,
         CommandsSyntaxAttribute? SyntaxAttr, CommandsSettingsAttribute? SettingsAttr) t,
        int? pluginId) =>
        new(
            pluginId,
            t.CommandAttr!.Name,
            t.CommandAttr.Description,
            t.Method,
            t.SettingsAttr?.Settings ?? default,
            t.CommandAttr.Type,
            t.CommandAttr.Permission,
            t.SyntaxAttr?.Syntax
        );
}
