using Amethyst.Extensions.Base.Result;

namespace Amethyst.Extensions.Base;

public interface IExtensionHandler
{
    bool SupportsUnload { get; }

    ExtensionHandleResult Load();
    ExtensionHandleResult Unload();
}
