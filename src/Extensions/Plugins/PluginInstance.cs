using Amethyst.Extensions.Base;
using Amethyst.Extensions.Result;

namespace Amethyst.Extensions.Plugins;

public abstract class PluginInstance
{
    protected PluginInstance(IExtension root)
    {
        Root = root;
        Services = new ServiceManager(this);
    }

    public IExtension Root { get; }
    public ServiceManager Services { get; }

    internal ExtensionHandleResult RequestLoad()
    {
        try
        {
            Load();
            return new ExtensionHandleResult(ExtensionResult.SuccessOperation, "Plugin loaded successfully.");
        }
        catch (Exception ex)
        {
            return new ExtensionHandleResult(ExtensionResult.InternalError, ex.Message);
        }
    }
    protected abstract void Load();

    internal ExtensionHandleResult RequestUnload()
    {
        try
        {
            Unload();
            return new ExtensionHandleResult(ExtensionResult.SuccessOperation, "Plugin unloaded successfully.");
        }
        catch (Exception ex)
        {
            return new ExtensionHandleResult(ExtensionResult.InternalError, ex.Message);
        }
    }
    protected abstract void Unload();
}
