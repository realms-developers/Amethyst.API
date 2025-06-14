using Amethyst.Extensions.Base;
using Amethyst.Extensions.Base.Result;
using Amethyst.Hooks;
using Amethyst.Hooks.Args.Extensions;

namespace Amethyst.Extensions.Plugins;

public sealed class PluginExtensionHandler(PluginExtension extension) : IExtensionHandler
{
    public PluginExtension Extension { get; } = extension;
    public bool SupportsUnload => true;
    public bool IsInitialized { get; private set; }

    public ExtensionHandleResult Load()
    {
        if (IsInitialized)
        {
            return new ExtensionHandleResult(Extension.LoadIdentifier, ExtensionResult.ExternalError, "Extension already loaded.");
        }

        ExtensionHandleResult result = Extension.PluginInstance.RequestLoad();
        IsInitialized = true;

        HookRegistry.GetHook<PluginInitializeArgs>()
            .Invoke(new PluginInitializeArgs(Extension.PluginInstance, result));

        return result;
    }

    public ExtensionHandleResult Unload()
    {
        if (!IsInitialized)
        {
            return new ExtensionHandleResult(Extension.LoadIdentifier, ExtensionResult.ExternalError, "Extension not loaded.");
        }

        IsInitialized = false;

        ExtensionHandleResult result = Extension.PluginInstance.RequestUnload();
        HookRegistry.GetHook<PluginDeinitializeArgs>()
            .Invoke(new PluginDeinitializeArgs(Extension.PluginInstance, result));

        //Extension.LoadContext.Unload();

        return result;
    }
}
