using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Commands;
using Amethyst.Systems.Users.Base.Extensions;
using Amethyst.Systems.Users.Base.Messages;
using Amethyst.Systems.Users.Base.Permissions;
using Amethyst.Systems.Users.Base.Requests;
using Amethyst.Systems.Users.Base.Suspension;

namespace Amethyst.Systems.Users.Artificial;

public sealed class ArtificialUsersService(IProviderBuilder<IMessageProvider> messageBuilder,
    IProviderBuilder<IPermissionProvider> permissionBuilder,
    IProviderBuilder<IExtensionProvider> extensionBuilder,
    IProviderBuilder<ICommandProvider> commandBuilder,
    IProviderBuilder<IRequestProvider> requestBuilder,
    IProviderBuilder<ISuspensionProvider>? suspensionBuilder = null) : IUsersService<ArtificialUser, ArtificialUserMetadata>
{
    public IProviderBuilder<IMessageProvider> MessageProviderBuilder { get; set; } = messageBuilder;

    public IProviderBuilder<IPermissionProvider> PermissionProviderBuilder { get; set; } = permissionBuilder;

    public IProviderBuilder<IExtensionProvider> ExtensionProviderBuilder { get; set; } = extensionBuilder;

    public IProviderBuilder<ISuspensionProvider>? SuspensionProviderBuilder { get; set; } = suspensionBuilder;

    public IProviderBuilder<ICommandProvider> CommandProviderBuilder { get; set; } = commandBuilder;

    public IProviderBuilder<IRequestProvider> RequestProviderBuilder { get; set; } = requestBuilder;

    public ArtificialUser CreateUser(ArtificialUserMetadata metadata,
        IProviderBuilder<IMessageProvider>? messageBuilder = null,
        IProviderBuilder<IPermissionProvider>? permissionBuilder = null,
        IProviderBuilder<IExtensionProvider>? extensionBuilder = null,
        IProviderBuilder<ISuspensionProvider>? suspensionBuilder = null,
        IProviderBuilder<ICommandProvider>? commandBuilder = null,
        IProviderBuilder<IRequestProvider>? requestBuilder = null)
    {
        return new ArtificialUser(
            metadata.Name,
            messageBuilder ?? MessageProviderBuilder,
            permissionBuilder ?? PermissionProviderBuilder,
            extensionBuilder ?? ExtensionProviderBuilder,
            commandBuilder ?? CommandProviderBuilder,
            requestBuilder ?? RequestProviderBuilder,
            suspensionBuilder ?? SuspensionProviderBuilder);
    }
}
