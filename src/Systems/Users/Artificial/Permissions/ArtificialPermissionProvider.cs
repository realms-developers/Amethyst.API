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

    private readonly List<IPermissionProvider> _childProviders = [];
    public IReadOnlyList<IPermissionProvider> ChildProviders => _childProviders;

    public bool SupportsChildProviders => true;

    public bool HasChild<T>() where T : IPermissionProvider
    {
        return _childProviders.Any(p => p is T);
    }

    public void AddChild(IPermissionProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);

        if (_childProviders.Contains(provider))
        {
            return;
        }

        _childProviders.Add(provider);
    }

    public void RemoveChild(IPermissionProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);

        if (!_childProviders.Contains(provider))
        {
            return;
        }

        _childProviders.Remove(provider);
    }

    public void RemoveChild<T>() where T : IPermissionProvider
    {
        IPermissionProvider? provider = _childProviders.FirstOrDefault(p => p is T);
        if (provider != null)
        {
            _childProviders.Remove(provider);
        }
    }

    private PermissionAccess HandlePermission(Func<IPermissionProvider, PermissionAccess> action)
    {
        bool hasPermission = false;

        foreach (IPermissionProvider provider in _childProviders)
        {
            PermissionAccess result = action(provider);
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

    public PermissionAccess HasPermission(PermissionType type, int x, int y)
    {
        return HandlePermission(p => p.HasPermission(type, x, y));
    }

    public PermissionAccess HasPermission(PermissionType type, int x, int y, int width, int height)
    {
        return HandlePermission(p => p.HasPermission(type, x, y, width, height));
    }
}
