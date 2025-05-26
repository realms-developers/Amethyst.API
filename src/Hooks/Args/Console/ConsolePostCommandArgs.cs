using Amethyst.Hooks.Autoloading;

namespace Amethyst.Hooks.Args.Console;

[AutoloadHook(true, false)]
public sealed class ConsolePostCommandArgs
{
    public ConsolePostCommandArgs(string command)
    {
        Command = command;
    }

    public string Command { get; }
}
