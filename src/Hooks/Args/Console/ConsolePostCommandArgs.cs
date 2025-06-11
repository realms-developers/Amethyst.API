using Amethyst.Hooks.Autoloading;

namespace Amethyst.Hooks.Args.Console;

[AutoloadHook(true, false)]
public sealed class ConsolePostCommandArgs(string command)
{
    public string Command { get; } = command;
}
