using System.Collections.Concurrent;
using System.Diagnostics;
using Amethyst.Systems.Commands;
using Amethyst.Systems.Commands.Base;
using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Commands;
using Amethyst.Text;

namespace Amethyst.Systems.Users.Common.Commands;

public sealed class CommonCommandProvider : ICommandProvider, IDisposable
{
    public CommonCommandProvider(IAmethystUser user, int delay, List<string> repositories)
    {
        User = user;
        Delay = delay;
        Repositories = repositories;

        _ = Task.Run(CommandQueueHandler);
    }

    public IAmethystUser User { get; }

    public CommandHistory History { get; } = [];

    public PagesCollection? ActivePage { get; set; }

    public int Delay { get; set; }

    public List<string> Repositories { get; set; }

    private readonly BlockingCollection<Func<CompletedCommandInfo?>> _commandQueue = [];
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private void CommandQueueHandler()
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            try
            {
                Func<CompletedCommandInfo?> commandFunc = _commandQueue.Take(_cancellationTokenSource.Token);
                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    break;
                }

                CompletedCommandInfo? info = commandFunc();
                CompletedCommandInfo? last = History.GetLast();
                if (info != null)
                {
                    History.Add(info);
                }

                if (Delay > 0)
                {
                    Task.Delay(Delay).Wait();
                }
            }
            catch {}
        }
    }

    public void RunCommand(string commandText)
    {
        _commandQueue.Add(() =>
        {
            return CommandsUtility.RunCommand(
                CommandsOrganizer.Repositories.Where(repo => Repositories.Contains(repo.Name)).ToArray(),
                User,
                commandText
            );
        });
    }

    public void RunCommand(ICommand command, string commandArgs)
    {
        _commandQueue.Add(() =>
        {
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

    public void Dispose()
    {
        try {  _cancellationTokenSource.Cancel(); } catch { }
        _cancellationTokenSource.Dispose();
        _commandQueue.Dispose();
    }
}
