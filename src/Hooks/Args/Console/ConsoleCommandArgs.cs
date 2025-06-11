using Amethyst.Hooks.Autoloading;

namespace Amethyst.Hooks.Args.Console;

[AutoloadHook(true, false)]
public sealed class ConsoleCommandArgs(string command)
{
    public string Command { get; set; } = command;
}
