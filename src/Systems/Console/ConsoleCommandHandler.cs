using Amethyst.Hooks;
using Amethyst.Hooks.Args.Console;
using Amethyst.Hooks.Base;
using Amethyst.Systems.Users;

namespace Amethyst.Systems.Console;

public static class ConsoleCommandHandler
{
    public static void Attach()
    {
        HookRegistry.GetHook<ConsoleCommandArgs>()
            ?.Register(OnConsoleCommand);
    }
    public static void Deattach()
    {
        HookRegistry.GetHook<ConsoleCommandArgs>()
            ?.Unregister(OnConsoleCommand);
    }

    private static void OnConsoleCommand(in ConsoleCommandArgs args, HookResult<ConsoleCommandArgs> result)
    {
        UsersOrganizer.ConsoleUser.Commands.RunCommand(args.Command);
    }
}
