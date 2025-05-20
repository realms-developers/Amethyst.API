using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Extensions;
using Amethyst.Systems.Users.Base.Messages;
using Amethyst.Systems.Users.Base.Permissions;

namespace Amethyst.Systems.Users.Artificial;

public sealed class ArtificialUsersService : IUsersService<ArtificialUser, ArtificialUserMetadata>
{
    public ArtificialUsersService(IProviderBuilder<IMessageProvider> messageBuilder,
        IProviderBuilder<IPermissionProvider> permissionBuilder,
        IProviderBuilder<IExtensionProvider> extensionBuilder)
    {
        MessageProviderBuilder = messageBuilder;
        PermissionProviderBuilder = permissionBuilder;
        ExtensionProviderBuilder = extensionBuilder;
    }

    public IProviderBuilder<IMessageProvider> MessageProviderBuilder { get; set; }

    public IProviderBuilder<IPermissionProvider> PermissionProviderBuilder { get; set;  }

    public IProviderBuilder<IExtensionProvider> ExtensionProviderBuilder { get; set;  }

    public ArtificialUser CreateUser(ArtificialUserMetadata metadata,
        IProviderBuilder<IMessageProvider>? messageBuilder = null,
        IProviderBuilder<IPermissionProvider>? permissionBuilder = null,
        IProviderBuilder<IExtensionProvider>? extensionBuilder = null)
    {
        return new ArtificialUser(
            metadata.Name,
            messageBuilder ?? MessageProviderBuilder,
            permissionBuilder ?? PermissionProviderBuilder,
            extensionBuilder ?? ExtensionProviderBuilder);
    }
}
