using System.Reflection;

namespace Amethyst.Extensions.Plugins;

public sealed class PluginContainer : IDisposable
{
    internal PluginContainer(int loadId, byte[] ilCode, Assembly assembly)
    {
        LoadID = loadId;
        ILCode = ilCode;
        Assembly = assembly;
    }

    public int LoadID { get; }
    public byte[] ILCode { get; }
    public Assembly Assembly { get; }

    public PluginInstance? PluginInstance { get; private set; }

    public void Load()
    {
        PluginInstance? instance = TryGetInstance();
        if (instance == null)
        {
            Dispose();
            return;
        }

        instance.Container = this;

        PluginInstance = instance;
        PluginInstance.RequestLoad();
    }

    private PluginInstance? TryGetInstance()
    {
        Type target = typeof(PluginInstance);

        IEnumerable<Type> pluginTypes = Assembly.GetExportedTypes().Where((p) => p.IsSubclassOf(target));
        foreach (Type type in pluginTypes)
        {
            var createdInstance = (PluginInstance?)Activator.CreateInstance(type);
            return createdInstance;
        }

        return null;
    }

    public void Dispose()
    {
        PluginInstance?.RequestUnload();
        PluginInstance = null;

        PluginLoader.RemovePlugin(LoadID);

        GC.SuppressFinalize(this);
    }
}
