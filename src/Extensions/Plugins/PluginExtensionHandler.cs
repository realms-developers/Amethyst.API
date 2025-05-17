using Amethyst.Extensions.Result;

namespace Amethyst.Extensions.Plugins;

public sealed class PluginExtensionHandler : IExtensionHandler
{
    public PluginExtensionHandler(PluginExtension extension)
    {
        Extension = extension;
    }

    public PluginExtension Extension { get; }
    public bool SupportsUnload => true;
    public bool IsInitialized { get; private set; }

    public ExtensionHandleResult Load()
    {
        if (IsInitialized)
            return new ExtensionHandleResult(ExtensionResult.ExternalError, "Extension already loaded.");

        IsInitialized = true;
        return Extension.PluginInstance.RequestLoad();
    }

    public ExtensionHandleResult Unload()
    {
        if (!IsInitialized)
            return new ExtensionHandleResult(ExtensionResult.ExternalError, "Extension not loaded.");

        IsInitialized = false;
        return Extension.PluginInstance.RequestUnload();
    }
}
