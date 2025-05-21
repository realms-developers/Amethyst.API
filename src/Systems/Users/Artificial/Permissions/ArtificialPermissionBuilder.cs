using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Permissions;

namespace Amethyst.Systems.Users.Artificial.Permissions;

public sealed class ArtificialPermissionBuilder : IProviderBuilder<IPermissionProvider>
{
    public IPermissionProvider BuildFor(IAmethystUser user)
    {
        return user is not ArtificialUser
            ? throw new ArgumentException("User is not a ArtificialUser", nameof(user))
            : (IPermissionProvider)new ArtificialPermissionProvider(user);
    }
}
