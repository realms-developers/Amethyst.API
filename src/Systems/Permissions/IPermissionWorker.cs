namespace Amethyst.Systems.Permissions;

public interface IPermissionWorker<T>
{
    public PermissionAccess HasPermission(T target, string permission);
    public PermissionAccess HasTilePermission(T target, int x, int y, int? width = null, int? height = null);
    public PermissionAccess HasChestPermission(T target, int x, int y);
    public PermissionAccess HasChestEditPermission(T target, int x, int y);
    public PermissionAccess HasSignPermission(T target, int x, int y);
    public PermissionAccess HasSignEditPermission(T target, int x, int y);
    public PermissionAccess HasTEPermission(T target, int x, int y);
}