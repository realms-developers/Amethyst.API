using Amethyst.Extensions.Base.Result;

namespace Amethyst.Extensions.Base.Repositories;

public interface IExtensionRepository
{
    public string Name { get; }

    IRepositoryRuler Ruler { get; set; }

    IEnumerable<ExtensionHandleResult> LoadExtensions();
    IEnumerable<ExtensionHandleResult> UnloadExtensions();

    IEnumerable<IExtension> Extensions { get; }
    IEnumerable<ExtensionHandleResult> Results { get; }

    IReadOnlyDictionary<IExtension, ExtensionHandleResult> ExtensionMap { get; }
}
