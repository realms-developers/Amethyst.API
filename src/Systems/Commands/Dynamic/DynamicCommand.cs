using System.Reflection;
using Amethyst.Systems.Commands.Base;
using Amethyst.Systems.Commands.Base.Metadata;

namespace Amethyst.Systems.Commands.Dynamic;

public sealed class DynamicCommand : ICommand
{
    internal DynamicCommand(Guid loadIdentifier, MethodInfo methodInfo, CommandRepository repository, CommandMetadata metadata, Type preferredUser)
    {
        LoadIdentifier = loadIdentifier;
        Repository = repository;
        Metadata = metadata;
        PreferredUser = preferredUser;

        Invoker = new DynamicCommandInvoker(this, methodInfo);
    }

    public Guid LoadIdentifier { get; }

    public CommandRepository Repository { get; }

    public ICommandInvoker Invoker { get; }

    public CommandMetadata Metadata { get; }

    public Type PreferredUser { get; }
}
