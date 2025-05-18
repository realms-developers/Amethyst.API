using Amethyst.Systems.Base.Users;
using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Extensions;
using Amethyst.Systems.Users.Base.Messages;
using Amethyst.Systems.Users.Base.Permissions;
using Amethyst.Systems.Users.Players.Extensions;
using Amethyst.Systems.Users.Players.Messages;
using Amethyst.Systems.Users.Players.Permissions;

namespace Amethyst.Systems.Users.Players;

public sealed class PlayersUsersService : IUsersService<PlayerUser, PlayerUserMetadata>
{
    public IProviderBuilder<IMessageProvider> MessageProviderBuilder { get; set;  } = new PlayerMessageBuilder();

    public IProviderBuilder<IPermissionProvider> PermissionProviderBuilder { get; set; } = new PlayerPermissionBuilder();

    public IProviderBuilder<IExtensionProvider> ExtensionProviderBuilder { get; set;  } = new PlayerExtensionBuilder();

    public PlayerUser CreateUser(PlayerUserMetadata metadata)
    {
        return new PlayerUser(
            metadata.Name,
            metadata.NetIndex,
            metadata.IP,
            metadata.UUID,
            this);
    }
}
