namespace Amethyst.Network.Handling.Base;

internal static class HandlerRuler
{
    public static bool IsHandlerEnabled(string name)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));

        return !HandlersConfiguration.Instance.DisabledHandlers.Contains(name);
    }

    public static void EnableHandler(string name)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));

        if (HandlersConfiguration.Instance.DisabledHandlers.Remove(name))
        {
            HandlersConfiguration.Configuration.Save();
        }
    }

    public static void DisableHandler(string name)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));

        if (!HandlersConfiguration.Instance.DisabledHandlers.Contains(name))
        {
            HandlersConfiguration.Instance.DisabledHandlers.Add(name);
            HandlersConfiguration.Configuration.Save();
        }
    }
}
