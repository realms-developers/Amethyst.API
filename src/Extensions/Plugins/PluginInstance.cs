using Amethyst.Extensions.Base;
using Amethyst.Extensions.Base.Result;
using Amethyst.Extensions.Plugins.Services;
using Amethyst.Hooks;
using Amethyst.Hooks.Args.Extensions;

namespace Amethyst.Extensions.Plugins;

public abstract class PluginInstance
{
    protected PluginInstance()
    {
        Services = new ServiceManager(this);

        Services.RegisterService<CommandsService>(new CommandsService(this));

        HookRegistry.GetHook<PluginPreloadArgs>()
            .Invoke(new PluginPreloadArgs(this));
    }

    public IExtension Root { get; internal set; } = null!;
    public ServiceManager Services { get; }

    internal ExtensionHandleResult RequestLoad()
    {
        try
        {
            Services.LoadAllServices();

            Load();

            Services.PostLoadAllServices();

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
            Services.UnloadAllServices();

            Unload();

            Services.PostUnloadAllServices();

            return new ExtensionHandleResult(Root.LoadIdentifier, ExtensionResult.SuccessOperation, "Plugin unloaded successfully.");
        }
        catch (Exception ex)
        {
            return new ExtensionHandleResult(Root.LoadIdentifier, ExtensionResult.InternalError, ex.Message);
        }
    }

    protected abstract void Unload();
}
