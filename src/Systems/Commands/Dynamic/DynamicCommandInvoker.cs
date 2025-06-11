using System.Reflection;
using System.Reflection.Emit;
using Amethyst.Systems.Commands.Base;
using Amethyst.Systems.Commands.Base.Metadata;
using Amethyst.Systems.Commands.Dynamic.Utilities;
using Amethyst.Systems.Users.Base;

namespace Amethyst.Systems.Commands.Dynamic;

public class DynamicCommandInvoker : ICommandInvoker
{
    internal DynamicCommandInvoker(DynamicCommand command, MethodInfo method)
    {
        Method = method;
        Command = command;

        _dynamicMethod = InvokingUtility.CreateDynamicMethod(method);
        _invokeAction = InvokingUtility.CreateInvoker(_dynamicMethod);
    }

    public MethodInfo Method { get; }

    public ICommand Command { get; }

    private readonly DynamicMethod _dynamicMethod;
    private readonly Action<object?[]> _invokeAction;

    public CommandInvokeContext CreateContext(IAmethystUser user, string[] args)
    {
        return new CommandInvokeContext(Command, user, args);
    }

    public void Invoke(CommandInvokeContext ctx)
    {
        if (!VerifyExecution(ctx.User) ||
            !VerifyPermission(ctx.User) ||
            !CreateArguments(ctx, out object?[]? args))
        {
            return;
        }

        _invokeAction(args!);
    }

    private bool CreateArguments(CommandInvokeContext ctx, out object?[]? args)
    {
        args = InvokingUtility.CreateArguments(this, ctx);
        return args != null;
    }

    private bool VerifyExecution(IAmethystUser user)
    {
        if (Command.Metadata.Rules.HasFlag(CommandRules.Disabled))
        {
            user.Messages.ReplyError("commands.disabled");
            return false;
        }

        if (!Command.PreferredUser.IsInstanceOfType(user))
        {
            user.Messages.ReplyError("commands.wrongUserType");
            return false;
        }

        return true;
    }

    private bool VerifyPermission(IAmethystUser user)
    {
        if (Command.Metadata.Permission == null)
        {
            return true;
        }

        if (user.Permissions == null)
        {
            user.Messages.ReplyError("commands.noPermissionHandler");
            return false;
        }

        if (user.Permissions.HasPermission(Command.Metadata.Permission) == Users.Base.Permissions.PermissionAccess.HasPermission)
        {
            return true;
        }

        user.Messages.ReplyError("commands.noPermission");
        return false;
    }
}
