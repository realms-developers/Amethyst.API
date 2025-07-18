using Amethyst.Systems.Users.Base.Commands;
using Amethyst.Systems.Users.Base.Extensions;
using Amethyst.Systems.Users.Base.Messages;
using Amethyst.Systems.Users.Base.Permissions;
using Amethyst.Systems.Users.Base.Requests;
using Amethyst.Systems.Users.Base.Suspension;

namespace Amethyst.Systems.Users.Base;

public interface IUsersService<TUser, TMetadata> where TUser : IAmethystUser
{
    IProviderBuilder<IMessageProvider> MessageProviderBuilder { get; set; }
    IProviderBuilder<IPermissionProvider> PermissionProviderBuilder { get; set; }
    IProviderBuilder<IExtensionProvider> ExtensionProviderBuilder { get; set; }
    IProviderBuilder<ISuspensionProvider>? SuspensionProviderBuilder { get; set; }
    IProviderBuilder<ICommandProvider> CommandProviderBuilder { get; set; }
    IProviderBuilder<IRequestProvider> RequestProviderBuilder { get; set; }

    TUser CreateUser(TMetadata metadata,
        IProviderBuilder<IMessageProvider>? messageBuilder = null,
        IProviderBuilder<IPermissionProvider>? permissionBuilder = null,
        IProviderBuilder<IExtensionProvider>? extensionBuilder = null,
        IProviderBuilder<ISuspensionProvider>? suspensionBuilder = null,
        IProviderBuilder<ICommandProvider>? commandBuilder = null,
        IProviderBuilder<IRequestProvider>? requestBuilder = null);
}
