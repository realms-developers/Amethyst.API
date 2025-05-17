using Amethyst.Extensions.Result;

namespace Amethyst.Extensions.Modules;

public sealed class ModuleExtensionHandler : IExtensionHandler
{
    public ModuleExtensionHandler(ModuleExtension extension)
    {
        Extension = extension;
    }

    public ModuleExtension Extension { get; }
    public bool SupportsUnload => false;

    public ExtensionHandleResult Load()
    {
        if (ModulesConfiguration.Instance.AllowedModules.Contains(Extension.Metadata.Name))
        {
            Extension.Initializer();

            return new ExtensionHandleResult(ExtensionResult.SuccessOperation, "Module loaded successfully.");
        }

        return new ExtensionHandleResult(ExtensionResult.NotAllowed, "Module is not allowed to be loaded.");
    }

    public ExtensionHandleResult Unload()
    {
        return new ExtensionHandleResult(ExtensionResult.NotAllowed, "Module unloading is not supported.");
    }
}
