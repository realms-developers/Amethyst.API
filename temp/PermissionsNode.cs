namespace Amethyst.Systems.Permissions;

public class PermissionsNode<T> where T : IPermissionProvider
{
    internal readonly List<IPermissionWorker<T>> Workers = [];

    internal PermissionAccess HandleResult(Func<IPermissionWorker<T>, PermissionAccess> invokeFunc)
    {
        PermissionAccess access = PermissionAccess.None;

        foreach (IPermissionWorker<T> worker in Workers)
        {
            PermissionAccess result = invokeFunc(worker);

            if (result == PermissionAccess.HasPermission)
            {
                access = result;
            }

            if (result == PermissionAccess.Blocked)
            {
                return result;
            }
        }

        return access;
    }

    public bool Register(IPermissionWorker<T> worker)
    {
        if (Workers.Contains(worker))
        {
            return false;
        }

        Workers.Add(worker);

        return true;
    }

    public bool Unregister(IPermissionWorker<T> worker) => Workers.Remove(worker);
}
