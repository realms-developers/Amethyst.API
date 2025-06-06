using System.Diagnostics;
using Amethyst.Systems.Commands;
using Amethyst.Systems.Commands.Base;
using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Commands;

namespace Amethyst.Systems.Users.Common.Commands;

public sealed class CommonCommandProvider : ICommandProvider
{
    public CommonCommandProvider(IAmethystUser user, int delay, List<string> repositories)
    {
        User = user;
        Delay = delay;
        Repositories = repositories;
    }

    public IAmethystUser User { get; }

    public CommandHistory History { get; } = new();

    public int Delay { get; set; }

    public List<string> Repositories { get; set; }

    private List<Task> _commandTasks = new();

    public void RunCommand(string commandText)
    {
        RunCommand(() =>
        {
            AmethystLog.System.Debug("CCP", $"Running command '{commandText}' for user '{User.Name}'");
            return CommandsUtility.RunCommand(
                CommandsOrganizer.Repositories.Where(repo => Repositories.Contains(repo.Name)).ToArray(),
                User,
                commandText
            );
        });
    }

    public void RunCommand(ICommand command, string commandArgs)
    {
        RunCommand(() =>
        {
            AmethystLog.System.Debug("CCP", $"Running command '{command.Metadata.Names.First()}' with args '{commandArgs}' for user '{User.Name}'");
            Stopwatch stopwatch = Stopwatch.StartNew();
            CommandsUtility.RunCommand(
                command,
                User,
                commandArgs
            );
            stopwatch.Stop();

            return new CompletedCommandInfo(
                command,
                commandArgs, // Remaining text is not applicable here
                stopwatch.Elapsed,
                DateTimeOffset.Now
            );
        });
    }

    private void RunCommand(Func<CompletedCommandInfo?> func)
    {
        if (Delay == 0)
        {
            func();
            return;
        }

        _commandTasks.Add(Task.Run(async () =>
        {
            if (_commandTasks.Count > 1)
            {
                await _commandTasks[_commandTasks.Count - 2];
            }

            CompletedCommandInfo? info = func();
            CompletedCommandInfo? last = History.GetLast();
            if (info != null && info.CommandArgs != last?.CommandArgs)
            {
                History.Add(info);
            }

            if (Delay > 0)
                await Task.Delay(Delay);
        }));
    }
}
