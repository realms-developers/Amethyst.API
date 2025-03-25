using System.Reflection;
using Amethyst.Commands.Data;
using Amethyst.Commands.Parsing;
using Amethyst.Core;
using Amethyst.Core.Server;
using Amethyst.Extensions.Plugins;

namespace Amethyst.Commands;

public static class CommandsManager
{
    internal static List<CommandRunner> Commands = new List<CommandRunner>();
    public static ICommandSender ConsoleSender { get; } = new ConsoleSender();

    internal static void Initialize()
    {
        ParsingNode.Initialize();
        ImportCommands(typeof(CommandsManager).Assembly, null);

        PluginLoader.OnPluginLoad += OnPluginLoad;
        PluginLoader.OnPluginUnload += OnPluginUnload;

        ConsoleInput.OnConsoleInput += OnConsoleInput;
    }

    private static void OnConsoleInput(string input, ref bool handled) 
    {
        if (RequestRun(ConsoleSender, input))
        {
            handled = true;
        }
    }

    public static bool RequestRun(ICommandSender sender, string text)
    {
        if (text.StartsWith('/'))
            text = text.Substring(1);

        var runner = FindCommand(text);

        if (runner == null)
        {
            sender.ReplyError("$LOCALIZE commands.commandNotFound");
            return false;
        }

        //var args = TextUtility.SplitArguments(text).Skip(runner.Data.Name.Split(' ').Length).ToList();

        if (runner.Data.Settings.HasFlag(CommandSettings.HideLog) == false)
            AmethystLog.System.Verbose("Commands", $"{sender.Name} [{sender.Type}]: $!r$!d/$!r$!b$r{text}");
        else
            AmethystLog.System.Verbose("Commands", $"{sender.Name} [{sender.Type}]: $!r$!d/$!r$!b$r{runner.Data.Name} $!r$!d(hidden log)");

        if (runner.Data.Type == CommandType.Debug && AmethystSession.Profile.DebugMode == false)
        {
            sender.ReplyError("$LOCALIZE commands.noDebugMode");
            return true;
        }

        if (runner.Data.Settings.HasFlag(CommandSettings.IngameOnly) && sender.Type != SenderType.RealPlayer)
        {
            sender.ReplyError("$LOCALIZE commands.ingameOnly");
            return true;
        }

        if (runner.Data.Permission != null && !sender.HasPermission(runner.Data.Permission))
        {
            sender.ReplyError("$LOCALIZE commands.noPermission");
            return true;
        }

        try
        {
            runner.Run(sender, text.Substring(runner.Data.Name.Length));
        }
        catch (Exception ex)
        {
            sender.ReplyError("$LOCALIZE commands.commandFailed");

            AmethystLog.System.Critical("Commands", $"Failed in running command '{text}' from {sender.Name} ({sender.Type}):");
            AmethystLog.System.Critical("Commands", ex.ToString());
        }

        return true;
    }

    public static CommandRunner? FindCommand(string name)
    {
        List<CommandRunner> cmds = Commands;

        foreach (CommandRunner cmd in cmds)
            if (cmd.NameEquals(name))
                return cmd;

        return null;
    }

    private static void OnPluginUnload(PluginContainer container)
    {
        Commands.RemoveAll(p => p.Data.PluginID == container.LoadID);
    }

    private static void OnPluginLoad(PluginContainer container)
    {
        ImportCommands(container.Assembly, container.LoadID);
    }

    internal static void ImportCommands(Assembly assembly, int? pluginId)
    {
        foreach (var cmd in LoadCommands(assembly, pluginId))
        {
            CommandRunner runner = new CommandRunner(cmd);
            Commands.Add(runner);
        }
    }

    internal static IEnumerable<CommandData> LoadCommands(Assembly assembly, int? pluginId)
    {
        foreach (Type type in assembly.GetExportedTypes())
            foreach (MethodInfo methodInfo in type.GetMethods())
            {
                ServerCommandAttribute? cmdAttr = methodInfo.GetCustomAttribute<ServerCommandAttribute>();
                if (cmdAttr == null) continue;

                ParameterInfo[] parameters = methodInfo.GetParameters();
                if (parameters.Length < 1 || parameters[0].ParameterType != typeof(CommandInvokeContext))
                {
                    yield break;
                }

                CommandsSyntaxAttribute? syntaxAttr = methodInfo.GetCustomAttribute<CommandsSyntaxAttribute>();
                CommandsSettingsAttribute? settingsAttr = methodInfo.GetCustomAttribute<CommandsSettingsAttribute>();

                yield return new CommandData(
                    pluginId,
                    cmdAttr.Name,
                    cmdAttr.Description,
                    methodInfo,
                    settingsAttr?.Settings ?? default,
                    cmdAttr.Type,
                    cmdAttr.Permission,
                    syntaxAttr?.Syntax
                );
            }

        yield break;
    }
}