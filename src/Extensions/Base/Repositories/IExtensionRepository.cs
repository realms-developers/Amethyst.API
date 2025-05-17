using Amethyst.Extensions.Result;

namespace Amethyst.Extensions.Repositories;

public interface IExtensionRepository
{
    IRepositoryRuler Ruler { get; set; }

    IEnumerable<ExtensionHandleResult> LoadExtensions();
    IEnumerable<ExtensionHandleResult> UnloadExtensions();
    IEnumerable<ExtensionHandleResult> ReloadExtensions();

    IEnumerable<IExtension> GetExtensions();
}
