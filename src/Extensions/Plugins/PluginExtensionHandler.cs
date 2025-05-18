using Amethyst.Extensions.Base;
using Amethyst.Extensions.Base.Result;

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

        if (Extension.Repository.Ruler.IsExtensionAllowed(Extension.Metadata.Name))
        {
            IsInitialized = true;
            return Extension.PluginInstance.RequestLoad();
        }

        Unload();

        return new ExtensionHandleResult(Extension.LoadIdentifier, ExtensionResult.NotAllowed, "Plugin is not allowed to be loaded.");
    }

    public ExtensionHandleResult Unload()
    {
        if (!IsInitialized)
        {
            return new ExtensionHandleResult(Extension.LoadIdentifier, ExtensionResult.ExternalError, "Extension not loaded.");
        }

        IsInitialized = false;

        ExtensionHandleResult result = Extension.PluginInstance.RequestUnload();

        Extension.LoadContext.Unload();

        return result;
    }
}
