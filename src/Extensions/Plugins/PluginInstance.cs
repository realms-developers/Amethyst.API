using Amethyst.Extensions.Base;
using Amethyst.Extensions.Base.Result;
using Amethyst.Extensions.Plugins.Services;

namespace Amethyst.Extensions.Plugins;

public abstract class PluginInstance
{
    protected PluginInstance()
    {
        Services = new ServiceManager(this);

        Services.RegisterService<CommandsService>(new CommandsService(this));
    }

    public IExtension Root { get; internal set; } = null!;
    public ServiceManager Services { get; }

    internal ExtensionHandleResult RequestLoad()
    {
        try
        {
            Load();

            Services.LoadAllServices();

            return new ExtensionHandleResult(Root.LoadIdentifier, ExtensionResult.SuccessOperation, "Plugin loaded successfully.");
        }
        catch (Exception ex)
        {
            return new ExtensionHandleResult(Root.LoadIdentifier, ExtensionResult.InternalError, ex.Message);
        }
    }

    protected abstract void Load();

    internal ExtensionHandleResult RequestUnload()
    {
        try
        {
            Unload();

            Services.UnloadAllServices();

            return new ExtensionHandleResult(Root.LoadIdentifier, ExtensionResult.SuccessOperation, "Plugin unloaded successfully.");
        }
        catch (Exception ex)
        {
            return new ExtensionHandleResult(Root.LoadIdentifier, ExtensionResult.InternalError, ex.Message);
        }
    }

    protected abstract void Unload();
}
