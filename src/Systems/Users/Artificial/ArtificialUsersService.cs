using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Commands;
using Amethyst.Systems.Users.Base.Extensions;
using Amethyst.Systems.Users.Base.Messages;
using Amethyst.Systems.Users.Base.Permissions;
using Amethyst.Systems.Users.Base.Suspension;
using Amethyst.Systems.Users.Common.Permissions;

namespace Amethyst.Systems.Users.Artificial;

public sealed class ArtificialUsersService : IUsersService<ArtificialUser, ArtificialUserMetadata>
{
    public ArtificialUsersService(IProviderBuilder<IMessageProvider> messageBuilder,
        IProviderBuilder<IPermissionProvider> permissionBuilder,
        IProviderBuilder<IExtensionProvider> extensionBuilder,
        IProviderBuilder<ICommandProvider> commandBuilder,
        IProviderBuilder<ISuspensionProvider>? suspensionBuilder = null)
    {
        MessageProviderBuilder = messageBuilder;
        PermissionProviderBuilder = permissionBuilder;
        ExtensionProviderBuilder = extensionBuilder;
        SuspensionProviderBuilder = suspensionBuilder;
        CommandProviderBuilder = commandBuilder;
    }

    public IProviderBuilder<IMessageProvider> MessageProviderBuilder { get; set; }

    public IProviderBuilder<IPermissionProvider> PermissionProviderBuilder { get; set;  }

    public IProviderBuilder<IExtensionProvider> ExtensionProviderBuilder { get; set; }

    public IProviderBuilder<ISuspensionProvider>? SuspensionProviderBuilder { get; set; }

    public IProviderBuilder<ICommandProvider> CommandProviderBuilder { get; set; }

    public ArtificialUser CreateUser(ArtificialUserMetadata metadata,
        IProviderBuilder<IMessageProvider>? messageBuilder = null,
        IProviderBuilder<IPermissionProvider>? permissionBuilder = null,
        IProviderBuilder<IExtensionProvider>? extensionBuilder = null,
        IProviderBuilder<ISuspensionProvider>? suspensionBuilder = null,
        IProviderBuilder<ICommandProvider>? commandBuilder = null)
    {
        return new ArtificialUser(
            metadata.Name,
            messageBuilder ?? MessageProviderBuilder,
            permissionBuilder ?? PermissionProviderBuilder,
            extensionBuilder ?? ExtensionProviderBuilder,
            commandBuilder ?? CommandProviderBuilder,
            suspensionBuilder ?? SuspensionProviderBuilder);
    }
}
