using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Permissions;

namespace Amethyst.Systems.Users.Players.Permissions;

public sealed class PlayerPermissionBuilder : IProviderBuilder<IPermissionProvider>
{
    public IPermissionProvider BuildFor(IAmethystUser user)
    {
        return user is not PlayerUser
            ? throw new ArgumentException("User is not a PlayerUser", nameof(user))
            : (IPermissionProvider)new PlayerPermissionProvider(user);
    }
}
