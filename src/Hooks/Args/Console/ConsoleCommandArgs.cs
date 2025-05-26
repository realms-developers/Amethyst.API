using Amethyst.Hooks.Autoloading;

namespace Amethyst.Hooks.Args.Console;

[AutoloadHook(true, false)]
public sealed class ConsoleCommandArgs
{
    public ConsoleCommandArgs(string command)
    {
        Command = command;
    }

    public string Command { get; set; }
}
