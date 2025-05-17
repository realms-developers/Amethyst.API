namespace Amethyst.Extensions.Plugins;

public sealed class ServiceManager
{
    internal ServiceManager(PluginInstance pluginInstance)
    {
        _pluginInstance = pluginInstance;
    }

    private readonly Dictionary<Type, IPluginService> _services = new();
    private readonly PluginInstance _pluginInstance;

    public T GetService<T>() where T : IPluginService
    {
        if (_services.TryGetValue(typeof(T), out var service))
        {
            return (T)service;
        }

        throw new KeyNotFoundException($"Service of type {typeof(T).Name} is not registered.");
    }

    public void RegisterService<T>(IPluginService service) where T : IPluginService
    {
        if (_services.ContainsKey(typeof(T)))
        {
            throw new InvalidOperationException($"Service of type {typeof(T).Name} is already registered.");
        }

        if (service.BaseInstance != _pluginInstance)
        {
            throw new InvalidOperationException("Service instance does not match plugin instance.");
        }

        _services[typeof(T)] = service;
    }

    public void UnregisterService<T>() where T : IPluginService
    {
        if (!_services.Remove(typeof(T)))
        {
            throw new KeyNotFoundException($"Service of type {typeof(T).Name} is not registered.");
        }
    }

    internal void LoadAllServices()
    {
        foreach (var service in _services.Values)
        {
            service.OnPluginLoad();
        }
    }

    internal void UnloadAllServices()
    {
        foreach (var service in _services.Values)
        {
            service.OnPluginUnload();
        }
    }
}
