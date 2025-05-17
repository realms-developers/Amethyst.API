using Amethyst.Extensions.Base;
using Amethyst.Extensions.Repositories;

namespace Amethyst.Extensions;

public interface IExtension
{
    public Guid LoadIdentifier { get; }
    public ExtensionMetadata Metadata { get; }
    public IExtensionRepository Repository { get; }
    public IExtensionHandler Handler { get; set; }
}
