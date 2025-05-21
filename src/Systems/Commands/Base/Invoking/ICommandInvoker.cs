using Amethyst.Systems.Users.Base;

namespace Amethyst.Systems.Commands.Base.Invoking;

public interface ICommandInvoker
{
    public ICommand Command { get; }

    public CommandInvokeContext CreateContext(IAmethystUser user, string[] args);

    public void Invoke(CommandInvokeContext ctx);
}
