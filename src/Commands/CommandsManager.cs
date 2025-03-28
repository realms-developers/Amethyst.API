using System.Reflection;
using Amethyst.Commands.Attributes;
using Amethyst.Commands.Parsing;
using Amethyst.Core;
using Amethyst.Core.Server;
using Amethyst.Extensions.Plugins;

namespace Amethyst.Commands;

public static class CommandsManager
{
    internal static List<CommandRunner> Commands { get; } = [];
    public static ICommandSender ConsoleSender { get; } = new ConsoleSender();

    internal static void Initialize()
    {
        ParsingNode.Initialize();
        ImportCommands(typeof(CommandsManager).Assembly, null);

        PluginLoader.OnPluginLoad += OnPluginLoad;
        PluginLoader.OnPluginUnload += OnPluginUnload;
        ConsoleInput.OnConsoleInput += OnConsoleInput;
    }

    private static void OnConsoleInput(string input, ref bool handled) =>
        handled |= RequestRun(ConsoleSender, input);

    public static bool RequestRun(ICommandSender sender, string text)
    {
        string commandText = text.StartsWith('/') ? text[1..] : text;
        CommandRunner? runner = FindCommand(commandText);

        if (runner is null)
        {
            sender.ReplyError(Localization.Get("commands.commandNotFound", sender.Language));

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

        AmethystLog.System.Verbose("Commands", logMessage);
    }

    private static bool ValidateAndExecuteCommand(ICommandSender sender, string text, CommandRunner runner)
    {
        (bool IsValid, Action HandleError)[] validationResults =
        [
            ValidateDebugMode(sender, runner),
            ValidateIngameOnly(sender, runner),
            ValidatePermissions(sender, runner)
        ];

        if (validationResults.Any(result => !result.IsValid))
        {
            validationResults.First(result => !result.IsValid).HandleError();
            return true;
        }

        try
        {
            string arguments = text[runner.Data.Name.Length..];
            runner.Run(sender, arguments);
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
        () => sender.ReplyError(Localization.Get("commands.noDebugMode", sender.Language)));

    private static (bool IsValid, Action HandleError) ValidateIngameOnly(ICommandSender sender, CommandRunner runner) =>
        (!runner.Data.Settings.HasFlag(CommandSettings.IngameOnly) || sender.Type == SenderType.RealPlayer,
        () => sender.ReplyError(Localization.Get("commands.ingameOnly", sender.Language)));

    private static (bool IsValid, Action HandleError) ValidatePermissions(ICommandSender sender, CommandRunner runner) =>
        (runner.Data.Permission is null || sender.HasPermission(runner.Data.Permission),
        () => sender.ReplyError(Localization.Get("commands.noPermission", sender.Language)));

    private static void HandleCommandException(ICommandSender sender, string text, Exception ex)
    {
        sender.ReplyError(Localization.Get("commands.commandFailed", sender.Language));
        AmethystLog.System.Critical("Commands", $"Command failure '{text}' from {sender.Name} ({sender.Type}):\n{ex}");
    }

    public static CommandRunner? FindCommand(string name) =>
        Commands.FirstOrDefault(cmd => cmd.NameEquals(name));

    private static void OnPluginUnload(PluginContainer container) =>
        Commands.RemoveAll(p => p.Data.PluginID == container.LoadID);

    private static void OnPluginLoad(PluginContainer container) =>
        ImportCommands(container.Assembly, container.LoadID);

    internal static void ImportCommands(Assembly assembly, int? pluginId) =>
        Commands.AddRange(LoadCommands(assembly, pluginId).Select(cmd => new CommandRunner(cmd)));

    internal static IEnumerable<CommandData> LoadCommands(Assembly assembly, int? pluginId) =>
        assembly.GetExportedTypes()
            .SelectMany(type => type.GetMethods())
            .Select(method => (
                Method: method,
                CommandAttr: method.GetCustomAttribute<ServerCommandAttribute>(),
                SyntaxAttr: method.GetCustomAttribute<CommandsSyntaxAttribute>(),
                SettingsAttr: method.GetCustomAttribute<CommandsSettingsAttribute>()))
            .Where(t => t.CommandAttr != null && HasValidParameters(t.Method))
            .Select(t => CreateCommandData(t, pluginId));

    private static bool HasValidParameters(MethodInfo method) =>
        method.GetParameters() is { Length: > 0 } parameters &&
        parameters[0].ParameterType == typeof(CommandInvokeContext);

    private static CommandData CreateCommandData(
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
