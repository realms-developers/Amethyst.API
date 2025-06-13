using System.Reflection;
using Amethyst.Extensions.Base;
using Amethyst.Extensions.Base.Metadata;
using Amethyst.Extensions.Base.Repositories;

namespace Amethyst.Extensions.Modules;

public sealed class ModuleExtension(ExtensionMetadata metadata, IExtensionRepository repository, Assembly assembly, ModuleInitializer initializeDelegates) : IExtension
{
    public Guid LoadIdentifier { get; } = Guid.NewGuid();

    public ExtensionMetadata Metadata { get; } = metadata;

    public IExtensionRepository Repository { get; } = repository;

    public IExtensionHandler Handler { get; set; } = null!;

    public ModuleInitializer Initializer { get; } = initializeDelegates;

    public Assembly Assembly { get; } = assembly;
}
