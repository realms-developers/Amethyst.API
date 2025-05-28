using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Permissions;

namespace Amethyst.Systems.Users.Artificial.Permissions;

public sealed class ArtificialPermissionProvider : IPermissionProvider
{
    internal ArtificialPermissionProvider(IAmethystUser user)
    {
        User = user;
    }


    public IAmethystUser User { get; }

    private List<IPermissionProvider> _childProviders = new();
    public IReadOnlyList<IPermissionProvider> ChildProviders => _childProviders;

    public bool SupportsChildProviders => true;

    public void AddChild(IPermissionProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);

        if (_childProviders.Contains(provider))
            return;

        _childProviders.Add(provider);
    }

    public void RemoveChild(IPermissionProvider provider)
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


    public PermissionAccess HasPermission(string permission)
    {
        return HandlePermission(p => p.HasPermission(permission));
    }

    public PermissionAccess HasPermission(PermissionAccess type, int x, int y)
    {
        return HandlePermission(p => p.HasPermission(type, x, y));
    }

    public PermissionAccess HasPermission(PermissionAccess type, int x, int y, int width, int height)
    {
        return HandlePermission(p => p.HasPermission(type, x, y, width, height));
    }
}
