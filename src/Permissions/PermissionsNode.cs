namespace Amethyst.Permissions;

public class PermissionsNode<T> where T : IPermissionable
{
    internal List<IPermissionWorker<T>> Workers = new List<IPermissionWorker<T>>();

    internal PermissionAccess HandleResult(Func<IPermissionWorker<T>, PermissionAccess> invokeFunc)
    {
        PermissionAccess access = PermissionAccess.None;
        foreach (var worker in Workers)
        {
            var result = invokeFunc(worker);
            if (result == PermissionAccess.HasPermission)
                access = result;

            if (result == PermissionAccess.Blocked)
                return result;
        }

        return access;
    }

    public bool Register(IPermissionWorker<T> worker)
    {
        if (Workers.Contains(worker)) return false;

        Workers.Add(worker);
        return true;
    }

    public bool Unregister(IPermissionWorker<T> worker)
    {
        return Workers.Remove(worker);
    }
}