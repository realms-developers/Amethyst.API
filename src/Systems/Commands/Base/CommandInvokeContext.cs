using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Messages;
using Amethyst.Systems.Users.Base.Permissions;

namespace Amethyst.Systems.Commands.Base;

public sealed class CommandInvokeContext
{
    internal CommandInvokeContext(ICommand command, IAmethystUser user, string[] args)
    {
        Command = command;
        User = user;
        Args = args;

        Permissions = user.Permissions;
        Messages = user.Messages;
    }

    public ICommand Command { get; }
    public IAmethystUser User { get; }

    public string[] Args { get; }

    public IPermissionProvider Permissions { get; set; }
    public IMessageProvider Messages { get; set; }
}
