using Amethyst.Extensions.Result;

namespace Amethyst.Extensions.Repositories;

public interface IExtensionRepository
{
    public string Name { get; }

    IRepositoryRuler Ruler { get; set; }

    IEnumerable<ExtensionHandleResult> LoadExtensions();
    IEnumerable<ExtensionHandleResult> UnloadExtensions();

    IEnumerable<IExtension> GetExtensions();
}
