using System.Reflection;
using Amethyst.Extensions.Base.Metadata;
using Amethyst.Extensions.Base.Repositories;

namespace Amethyst.Extensions.Base;

public interface IExtension
{
    public Guid LoadIdentifier { get; }
    public ExtensionMetadata Metadata { get; }
    public IExtensionRepository Repository { get; }
    public IExtensionHandler Handler { get; set; }
    public Assembly Assembly { get; }
}
