using System.Diagnostics;
using Amethyst.Systems.Commands.Base;
using Amethyst.Systems.Commands.Base.Metadata;
using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Commands;

namespace Amethyst.Systems.Commands;

public static class CommandsUtility
{
    public static CompletedCommandInfo? RunCommand(CommandRepository[] repositories, IAmethystUser user, string commandText)
    {
        foreach (var repository in repositories)
        {
            var command = repository.FindCommand(commandText, out string remainingText);
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
    public static void RunCommand(ICommand command, IAmethystUser user, string commandText)
    {
        try
        {
            var ctx = command.Invoker.CreateContext(user, SplitByArguments(commandText));
            if (!command.Metadata.Rules.HasFlag(CommandRules.NoLogging))
                AmethystLog.System.Info($"Commands<{user.Name}>", $"{commandText} [{command.Repository.Name}]");
            command.Invoker.Invoke(ctx);
        }
        catch (Exception ex)
        {
            user.Messages.ReplyError("commands.error_tellDevelopers");

            AmethystLog.System.Info($"Commands<{user.Name}>", $"");
            AmethystLog.System.Error($"Commands<{user.Name}>", $"Error while executing command: {ex.ToString()}");
        }
    }

    public static string[] SplitByArguments(string commandText)
    {
        var args = new List<string>();
        var current = new System.Text.StringBuilder();
        bool inQuotes = false;
        bool escape = false;

        for (int i = 0; i < commandText.Length; i++)
        {
            char c = commandText[i];

            if (escape)
            {
                current.Append(c);
                escape = false;
            }
            else if (c == '\\')
            {
                escape = true;
            }
            else if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (char.IsWhiteSpace(c) && !inQuotes)
            {
                if (current.Length > 0)
                {
                    args.Add(current.ToString());
                    current.Clear();
                }
            }
            else
            {
                current.Append(c);
            }
        }

        if (current.Length > 0)
            args.Add(current.ToString());

        return args.ToArray();
    }
}
