namespace Amethyst.Systems.Permissions;

public interface IPermissionable
{
    public string Name { get; }

    public bool HasPermission(string permission);
    public bool HasTilePermission(int x, int y, int? width = null, int? height = null);
    public bool HasChestPermission(int x, int y);
    public bool HasChestEditPermission(int x, int y);
    public bool HasSignPermission(int x, int y);
    public bool HasSignEditPermission(int x, int y);
    public bool HasTEPermission(int x, int y);
}