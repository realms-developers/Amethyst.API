using Amethyst.Systems.Base.Users;
using Amethyst.Systems.Users.Base.Extensions;
using Amethyst.Systems.Users.Base.Messages;
using Amethyst.Systems.Users.Base.Permissions;

namespace Amethyst.Systems.Users.Base;

public interface IUsersService<TUser, TMetadata> where TUser : IAmethystUser
{
    IProviderBuilder<IMessageProvider> MessageProviderBuilder { get; set; }
    IProviderBuilder<IPermissionProvider> PermissionProviderBuilder { get; set; }
    IProviderBuilder<IExtensionProvider> ExtensionProviderBuilder { get; set; }

    TUser CreateUser(TMetadata metadata);
}
