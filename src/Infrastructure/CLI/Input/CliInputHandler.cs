namespace Amethyst.Infrastructure.CLI.Input;

public static class CliInputHandler
{
    private static readonly List<InputHandler> _handlers = [];

    public static void RegisterHandler(InputHandler handler)
    {
        if (!_handlers.Contains(handler))
        {
            _handlers.Add(handler);
        }
    }

    public static void UnregisterHandler(InputHandler handler) => _handlers.Remove(handler);

    internal static void Initialize() => Task.Run(CliTask);

    private static void CliTask()
    {
        while (true)
        {
            string? input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                continue;
            }

            CancellationTokenSource tokenSource = new();

            foreach (InputHandler handler in _handlers)
            {
                Task task = handler(input!, tokenSource.Token);
                task.ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        AmethystLog.Startup.Error(nameof(CliInputHandler), $"Failed to handle command '{input}':");
                        AmethystLog.Startup.Error(nameof(CliInputHandler), t.Exception.ToString());
                    }
                });

                if (tokenSource.Token.IsCancellationRequested)
                {
                    break;
                }
            }
        }
    }
}
