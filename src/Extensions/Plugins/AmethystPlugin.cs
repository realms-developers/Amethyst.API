using Amethyst.Core;

namespace Amethyst.Extensions.Plugins;

public abstract class PluginInstance
{
    public abstract string Name { get; }
    public abstract Version Version { get; }
    public virtual string? Description { get; }

    public bool IsLoaded { get; private set; }

    //internal int LoadID;
    internal PluginContainer Container = null!;

    protected abstract void Load();
    protected abstract void Unload();

    internal bool RequestLoad()
    {
        IsLoaded = true;
        PluginLoader.InvokeLoad(Container!);
        return RequestOperation("Load", Load);
    }

    internal bool RequestUnload()
    {
        IsLoaded = false;
        PluginLoader.InvokeUnload(Container!);
        return RequestOperation("Unload", Unload);
    }

    private bool RequestOperation(string logName, Action action)
    {
        try
        {
            action();
            return true;
        }
        catch (Exception ex)
        {
            if (logName == "Load" && PluginLoader.InFirstLoadStage)
            {
                PluginLoader.LogLoaded.Remove(Container.FileName);
                PluginLoader.LogFailed.Add(Container.FileName, ex);
            }
            else
            {
                AmethystLog.Main.Critical(nameof(PluginInstance), $"Failed to process '{logName}' operation for '{Name}':");
                AmethystLog.Main.Critical(nameof(PluginInstance), ex.ToString());
            }
        }

        return false;
    }
}
