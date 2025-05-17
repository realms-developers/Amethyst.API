namespace Amethyst.Infrastructure.CLI.Input;

public static class CliInputHandler
{
    private static List<InputHandler> _Handlers = new();

    public static void RegisterHandler(InputHandler handler)
    {
        if (_Handlers.Contains(handler))
        {
            return;
        }
        _Handlers.Add(handler);
    }

    public static void UnregisterHandler(InputHandler handler)
    {
        _Handlers.Remove(handler);
    }

    internal static void Initialize()
    {
        Task.Run(CliTask);
    }

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

            foreach (var handler in _Handlers)
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
