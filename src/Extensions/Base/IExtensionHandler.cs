using Amethyst.Extensions.Result;

namespace Amethyst.Extensions;

public interface IExtensionHandler
{
    bool SupportsUnload { get; }

    ExtensionHandleResult Load();
    ExtensionHandleResult Unload();
}
