namespace Amethyst.Network.Handling.Base;

public static class HandlerManager
{
    private static readonly List<INetworkHandler> Handlers = new();

    public static void RegisterHandler(INetworkHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler, nameof(handler));

        if (Handlers.Contains(handler))
        {
            return;
        }

        if (HandlerRuler.IsHandlerEnabled(handler.Name))
        {
            handler.Load();
            AmethystLog.Network.Info(nameof(HandlerManager), $"NetworkHandler {handler.Name} was registered -> loaded.");
        }
        else
        {
            AmethystLog.Network.Warning(nameof(HandlerManager), $"NetworkHandler {handler.Name} was registered -> not loaded (disabled).");
        }

        Handlers.Add(handler);
    }

    public static void UnregisterHandler(INetworkHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler, nameof(handler));

        if (!Handlers.Contains(handler))
        {
            return;
        }

        if (HandlerRuler.IsHandlerEnabled(handler.Name))
        {
            handler.Unload();
            AmethystLog.Network.Info(nameof(HandlerManager), $"NetworkHandler {handler.Name} was unregistered -> unloaded.");
        }
        else
        {
            AmethystLog.Network.Info(nameof(HandlerManager), $"NetworkHandler {handler.Name} was unregistered -> not unloaded (already disabled).");
        }

        Handlers.Remove(handler);
    }

    public static bool LoadHandler(string name)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));

        INetworkHandler? handler = Handlers.FirstOrDefault(h => h.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (handler == null)
        {
            return false;
        }

        if (!HandlerRuler.IsHandlerEnabled(handler.Name))
        {
            handler.Load();
            HandlerRuler.EnableHandler(name);
            AmethystLog.Network.Warning(nameof(HandlerManager), $"NetworkHandler {name} has been enabled.");
            return true;
        }
        return false;
    }

    public static bool UnloadHandler(string name)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));

        INetworkHandler? handler = Handlers.FirstOrDefault(h => h.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (handler == null)
        {
            return false;
        }

        if (HandlerRuler.IsHandlerEnabled(handler.Name))
        {
            HandlerRuler.DisableHandler(name);
            handler.Unload();
            AmethystLog.Network.Warning(nameof(HandlerManager), $"NetworkHandler {name} has been enabled.");
            return true;
        }
        return false;
    }

    public static IReadOnlyList<INetworkHandler> GetHandlers()
    {
        return Handlers.AsReadOnly();
    }
}
