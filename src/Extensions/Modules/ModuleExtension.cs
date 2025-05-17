using System.Reflection;
using Amethyst.Extensions.Base;
using Amethyst.Extensions.Repositories;

namespace Amethyst.Extensions.Modules;

public sealed class ModuleExtension : IExtension
{
    public ModuleExtension(ExtensionMetadata metadata, IExtensionRepository repository, Assembly assembly, ModuleInitializer initializeDelegates)
    {
        Initializer = initializeDelegates;
        Metadata = metadata;
        Assembly = assembly;
        Repository = repository;
    }

    public Guid LoadIdentifier { get; } = Guid.NewGuid();

    public ExtensionMetadata Metadata { get; }

    public IExtensionRepository Repository { get; }

    public IExtensionHandler Handler { get; set; } = null!;

    public ModuleInitializer Initializer { get; }

    public Assembly Assembly { get; }
}
