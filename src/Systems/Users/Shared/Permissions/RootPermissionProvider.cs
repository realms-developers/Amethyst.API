using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Permissions;

namespace Amethyst.Systems.Users.Shared.Permissions;

public sealed class RootPermissionProvider : IPermissionProvider
{
    public RootPermissionProvider(IAmethystUser user)
    {
        User = user;
    }

    public IAmethystUser User { get; }

    public bool SupportsChildProviders => false;

    public void AddChild(IPermissionProvider provider)
    {
        throw new NotSupportedException("RootPermissionProvider does not support child providers.");
    }

    public void RemoveChild(IPermissionProvider provider)
    {
        throw new NotSupportedException("RootPermissionProvider does not support child providers.");
    }

    public PermissionAccess HasPermission(string permission)
    {
        return PermissionAccess.HasPermission;
    }

    public PermissionAccess HasPermission(PermissionAccess type, int x, int y)
    {
        return PermissionAccess.HasPermission;
    }

    public PermissionAccess HasPermission(PermissionAccess type, int x, int y, int width, int height)
    {
        return PermissionAccess.HasPermission;
    }
}
