using System.Diagnostics;
using Amethyst.Systems.Commands.Base;
using Amethyst.Systems.Commands.Base.Metadata;
using Amethyst.Systems.Users.Base;

namespace Amethyst.Systems.Commands;

public static class CommandsUtility
{
    public static CompletedCommandInfo? RunCommand(CommandRepository[] repositories, IAmethystUser user, string commandText)
    {
        if (commandText.StartsWith('/'))
        {
            commandText = commandText.Substring(1);
        }

        foreach (CommandRepository repository in repositories)
        {
            ICommand? command = repository.FindCommand(commandText, out string remainingText);

            if (command != null)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                RunCommand(command, user, remainingText);
                stopwatch.Stop();

                return new CompletedCommandInfo(command, remainingText, stopwatch.Elapsed, DateTimeOffset.Now);
            }
        }

        user.Messages.ReplyError("commands.commandNotFound");
        return null;
    }

    public static void RunCommand(ICommand command, IAmethystUser user, string args)
    {
        try
        {
            CommandInvokeContext ctx = command.Invoker.CreateContext(user, SplitByArguments(args));
            if (!command.Metadata.Rules.HasFlag(CommandRules.NoLogging))
            {
                AmethystLog.System.Info($"Commands<{user.Name}>", $"/{command.Metadata.Names.First() ?? "unknown"} -> [{string.Join(", ", ctx.Args.Select(p => $"\"{p}\""))}] [{command.Repository.Name}]");
            }

            command.Invoker.Invoke(ctx);
        }
        catch (Exception ex)
        {
            user.Messages.ReplyError("commands.error_tellDevelopers");

            AmethystLog.System.Info($"Commands<{user.Name}>", string.Empty);
            AmethystLog.System.Error($"Commands<{user.Name}>", $"Error while executing command: {ex}");
        }
    }

    public static string[] SplitByArguments(string text)
    {
        if (text.Length == 0)
        {
            return [];
        }

        List<string> args = [""];
        int index = 0;

        bool blockSpace = false;
        bool ignoreFormat = false;
        foreach (char c in text)
        {
            if (c == '"' && !ignoreFormat)
            {
                blockSpace = !blockSpace;
                ignoreFormat = false;
            }
            else if (c == ' ' && !ignoreFormat && !blockSpace)
            {
                args.Add("");
                index++;
                ignoreFormat = false;
            }
            else if (c == '\\' && !ignoreFormat)
            {
                ignoreFormat = true;
            }
            else
            {
                args[index] += c;
            }
        }

        return [.. args];
    }
}
