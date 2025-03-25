namespace Amethyst.Core.Server;

public static class ConsoleInput
{
    public static event ConsoleInputHandler? OnConsoleInput;

    internal static void Initialize()
    {
        Thread thread = new Thread(CliTask);
        thread.Start();
    }

    private static void CliTask()
    {
        while (true)
        {
            string? input = Console.ReadLine();
            if (string.IsNullOrEmpty(input)) continue;

            try
            {
                bool handled = false;
                OnConsoleInput?.Invoke(input, ref handled);
            }
            catch (Exception ex)
            {
                AmethystLog.Startup.Error("ConsoleInput", $"Failed to handle command {input}:");
                AmethystLog.Startup.Error("ConsoleInput", ex.ToString());
            }
        }
    }
}