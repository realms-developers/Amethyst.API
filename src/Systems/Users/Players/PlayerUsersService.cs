using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Commands;
using Amethyst.Systems.Users.Base.Extensions;
using Amethyst.Systems.Users.Base.Messages;
using Amethyst.Systems.Users.Base.Permissions;
using Amethyst.Systems.Users.Base.Suspension;

namespace Amethyst.Systems.Users.Players;

public sealed class PlayersUsersService : IUsersService<PlayerUser, PlayerUserMetadata>
{
    public PlayersUsersService(IProviderBuilder<IMessageProvider> messageBuilder,
        IProviderBuilder<IPermissionProvider> permissionBuilder,
        IProviderBuilder<IExtensionProvider> extensionBuilder,
        IProviderBuilder<ISuspensionProvider> suspensionBuilder,
        IProviderBuilder<ICommandProvider> commandBuilder)
    {
        MessageProviderBuilder = messageBuilder;
        PermissionProviderBuilder = permissionBuilder;
        ExtensionProviderBuilder = extensionBuilder;
        SuspensionProviderBuilder = suspensionBuilder;
        CommandProviderBuilder = commandBuilder;
    }

    public IProviderBuilder<IMessageProvider> MessageProviderBuilder { get; set; }

    public IProviderBuilder<IPermissionProvider> PermissionProviderBuilder { get; set; }

    public IProviderBuilder<IExtensionProvider> ExtensionProviderBuilder { get; set;  }

    public IProviderBuilder<ISuspensionProvider>? SuspensionProviderBuilder { get; set; }

    public IProviderBuilder<ICommandProvider> CommandProviderBuilder { get; set; }

    public PlayerUser CreateUser(PlayerUserMetadata metadata,
        IProviderBuilder<IMessageProvider>? messageBuilder = null,
        IProviderBuilder<IPermissionProvider>? permissionBuilder = null,
        IProviderBuilder<IExtensionProvider>? extensionBuilder = null,
        IProviderBuilder<ISuspensionProvider>? suspensionBuilder = null,
        IProviderBuilder<ICommandProvider>? commandBuilder = null)
    {
        return new PlayerUser(
            metadata.Name,
            metadata.NetIndex,
            metadata.IP,
            metadata.UUID,
            messageBuilder ?? MessageProviderBuilder,
            permissionBuilder ?? PermissionProviderBuilder,
            extensionBuilder ?? ExtensionProviderBuilder,
            commandBuilder ?? CommandProviderBuilder,
            suspensionBuilder ?? SuspensionProviderBuilder
        );
    }
}
