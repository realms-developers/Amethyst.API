using Amethyst.Systems.Base.Users;
using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Extensions;
using Amethyst.Systems.Users.Base.Messages;
using Amethyst.Systems.Users.Base.Permissions;
using Amethyst.Systems.Users.Players.Extensions;

namespace Amethyst.Systems.Users.Players;

public sealed class PlayersUsersService : IUsersService<PlayerUser, PlayerUserMetadata>
{
    public IProviderBuilder<IMessageProvider> MessageProviderBuilder { get; set;  }

    public IProviderBuilder<IPermissionProvider> PermissionProviderBuilder { get; set;  }

    public IProviderBuilder<IExtensionProvider> ExtensionProviderBuilder { get; set;  } = new PlayerExtensionBuilder();

    public PlayerUser CreateUser(PlayerUserMetadata metadata)
    {
        throw new NotImplementedException();
    }
}
