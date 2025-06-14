using Amethyst.Hooks;
using Amethyst.Hooks.Args.Console;

namespace Amethyst.Kernel.CLI;

internal static class ConsoleHooks
{
    internal static void AttachHooks()
    {
        System.Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;

            AmethystSession.Launcher.StopServer();
        };
    }

    internal static void InputTask()
    {
        while (true)
        {
            string? input = System.Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            var commandArgs = new ConsoleCommandArgs(input);

            if (commandArgs.Command.StartsWith('/'))
            {
                commandArgs.Command = commandArgs.Command[1..];
            }

            if (commandArgs.Command.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
                commandArgs.Command.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                AmethystSession.Launcher.StopServer();
                return;
            }

            AmethystLog.System.Info(nameof(ConsoleHooks), $"Executing command: {commandArgs.Command}");

            HookRegistry.GetHook<ConsoleCommandArgs>()?.Invoke(commandArgs);

            HookRegistry.GetHook<ConsolePostCommandArgs>()?.Invoke(new ConsolePostCommandArgs(commandArgs.Command));
        }
    }
}
