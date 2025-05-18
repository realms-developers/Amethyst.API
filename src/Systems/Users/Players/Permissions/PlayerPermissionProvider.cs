using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Permissions;

namespace Amethyst.Systems.Users.Players.Permissions;

public sealed class PlayerPermissionProvider : IPermissionProvider
{
    internal PlayerPermissionProvider(IAmethystUser user)
    {
        User = user;
    }


    public IAmethystUser User { get; }

    private List<IPermissionProvider> _childProviders = new();
    public IReadOnlyList<IPermissionProvider> ChildProviders => _childProviders;

    public bool SupportsChildProviders => true;

    public void AddChildProvider(IPermissionProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);

        if (_childProviders.Contains(provider))
            return;

        _childProviders.Add(provider);
    }

    public void RemoveChildProvider(IPermissionProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);

        if (!_childProviders.Contains(provider))
            return;

        _childProviders.Remove(provider);
    }

    private PermissionAccess HandlePermission(Func<IPermissionProvider, PermissionAccess> action)
    {
        bool hasPermission = false;

        foreach (var provider in _childProviders)
        {
            var result = action(provider);
            if (result == PermissionAccess.HasPermission)
            {
                hasPermission = true;
                break;
            }
            else if (result == PermissionAccess.Blocked)
            {
                return PermissionAccess.Blocked;
            }
        }

        return hasPermission ? PermissionAccess.HasPermission : PermissionAccess.None;
    }

    public PermissionAccess HasChestEditPermission(int x, int y)
    {
        return HandlePermission(p => p.HasChestEditPermission(x, y));
    }

    public PermissionAccess HasChestPermission(int x, int y)
    {
        return HandlePermission(p => p.HasChestPermission(x, y));
    }

    public PermissionAccess HasPermission(string permission)
    {
        return HandlePermission(p => p.HasPermission(permission));
    }

    public PermissionAccess HasSignEditPermission(int x, int y)
    {
        return HandlePermission(p => p.HasSignEditPermission(x, y));
    }

    public PermissionAccess HasSignPermission(int x, int y)
    {
        return HandlePermission(p => p.HasSignPermission(x, y));
    }

    public PermissionAccess HasTEPermission(int x, int y)
    {
        return HandlePermission(p => p.HasTEPermission(x, y));
    }

    public PermissionAccess HasTilePermission(int x, int y)
    {
        return HandlePermission(p => p.HasTilePermission(x, y));
    }

    public PermissionAccess HasTilePermission(int x, int y, int width, int height)
    {
        return HandlePermission(p => p.HasTilePermission(x, y, width, height));
    }
}
