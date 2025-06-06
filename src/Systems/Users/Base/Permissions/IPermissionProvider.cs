namespace Amethyst.Systems.Users.Base.Permissions;

public interface IPermissionProvider
{
    IAmethystUser User { get; }

    bool SupportsChildProviders { get; }

    bool HasChild<T>() where T : IPermissionProvider;
    void AddChild(IPermissionProvider provider);
    void RemoveChild(IPermissionProvider provider);
    void RemoveChild<T>() where T : IPermissionProvider;

    PermissionAccess HasPermission(string permission);

    public PermissionAccess HasPermission(PermissionAccess type, int x, int y);
    public PermissionAccess HasPermission(PermissionAccess type, int x, int y, int width, int height);
}
