using Amethyst.Systems.Commands.Base;
using Amethyst.Systems.Commands.Base.Invoking;
using Amethyst.Systems.Commands.Base.Metadata;

namespace Amethyst.Systems.Commands.Dynamic;

public sealed class DynamicCommand : ICommand
{
    internal DynamicCommand(Guid loadIdentifier, CommandRepository repository, ICommandInvoker invoker, CommandMetadata metadata, Type preferredUser)
    {
        LoadIdentifier = loadIdentifier;
        Repository = repository;
        Invoker = invoker;
        Metadata = metadata;
        PreferredUser = preferredUser;
    }

    public Guid LoadIdentifier { get; }

    public CommandRepository Repository { get; }

    public ICommandInvoker Invoker { get; }

    public CommandMetadata Metadata { get; }

    public Type PreferredUser { get; }
}
