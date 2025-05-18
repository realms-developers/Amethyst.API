using Amethyst.Extensions.Base;
using Amethyst.Extensions.Base.Result;

namespace Amethyst.Extensions.Modules;

public sealed class ModuleExtensionHandler(ModuleExtension extension) : IExtensionHandler
{
    public ModuleExtension Extension { get; } = extension;
    public bool SupportsUnload => false;

    public ExtensionHandleResult Load()
    {
        if (Extension.Repository.Ruler.IsExtensionAllowed(Extension.Metadata.Name))
        {
            Extension.Initializer();

            return new ExtensionHandleResult(Extension.LoadIdentifier, ExtensionResult.SuccessOperation, "Module loaded successfully.");
        }

        return new ExtensionHandleResult(Extension.LoadIdentifier, ExtensionResult.NotAllowed, "Module is not allowed to be loaded.");
    }

    public ExtensionHandleResult Unload()
    {
        return new ExtensionHandleResult(Extension.LoadIdentifier, ExtensionResult.NotAllowed, "Module unloading is not supported.");
    }
}
