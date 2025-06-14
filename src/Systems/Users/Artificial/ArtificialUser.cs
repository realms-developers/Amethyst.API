using Amethyst.Systems.Characters.Base;
using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Commands;
using Amethyst.Systems.Users.Base.Extensions;
using Amethyst.Systems.Users.Base.Messages;
using Amethyst.Systems.Users.Base.Permissions;
using Amethyst.Systems.Users.Base.Requests;
using Amethyst.Systems.Users.Base.Suspension;

namespace Amethyst.Systems.Users.Artificial;

public sealed class ArtificialUser : IAmethystUser
{
    public ArtificialUser(string name, IProviderBuilder<IMessageProvider> messageBuilder,
        IProviderBuilder<IPermissionProvider> permissionBuilder,
        IProviderBuilder<IExtensionProvider> extensionBuilder,
        IProviderBuilder<ICommandProvider> commandBuilder,
        IProviderBuilder<IRequestProvider> requestBuilder,
        IProviderBuilder<ISuspensionProvider>? suspensionBuilder = null)
    {
        Name = name;
        Messages = messageBuilder.BuildFor(this);
        Permissions = permissionBuilder.BuildFor(this);
        Extensions = extensionBuilder.BuildFor(this);
        Suspensions = suspensionBuilder?.BuildFor(this);
        Commands = commandBuilder.BuildFor(this);
        Requests = requestBuilder.BuildFor(this);
    }


    public string Name { get; }

    public IMessageProvider Messages { get; }

    public IPermissionProvider Permissions { get; }

    public IExtensionProvider Extensions { get; }

    public ICharacterProvider? Character { get; }

    public ISuspensionProvider? Suspensions { get; }

    public ICommandProvider Commands { get; }

    public IRequestProvider Requests { get; }
}
