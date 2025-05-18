namespace Amethyst.Systems.Users.Base.Permissions;

public interface IPermissionProvider
{
    IAmethystUser User { get; }

    bool SupportsChildProviders { get; }

    void AddChildProvider(IPermissionProvider provider);
    void RemoveChildProvider(IPermissionProvider provider);

    PermissionAccess HasPermission(string permission);
    PermissionAccess HasTilePermission(int x, int y);
    PermissionAccess HasTilePermission(int x, int y, int width, int height);
    PermissionAccess HasChestPermission(int x, int y);
    PermissionAccess HasChestEditPermission(int x, int y);
    PermissionAccess HasSignPermission(int x, int y);
    PermissionAccess HasSignEditPermission(int x, int y);
    PermissionAccess HasTEPermission(int x, int y);
}
