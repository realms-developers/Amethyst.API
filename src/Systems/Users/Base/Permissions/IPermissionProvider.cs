namespace Amethyst.Systems.Users.Base.Permissions;

public interface IPermissionProvider
{
    IAmethystUser User { get; }

    bool SupportsChildProviders { get; }

    void AddChild(IPermissionProvider provider);
    void RemoveChild(IPermissionProvider provider);

    PermissionAccess HasPermission(string permission);

    public PermissionAccess HasPermission(PermissionAccess type, int x, int y);
    public PermissionAccess HasPermission(PermissionAccess type, int x, int y, int width, int height);
}
