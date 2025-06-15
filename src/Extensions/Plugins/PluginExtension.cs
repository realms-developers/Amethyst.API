using System.Reflection;
using Amethyst.Extensions.Base;
using Amethyst.Extensions.Base.Metadata;
using Amethyst.Extensions.Base.Repositories;

namespace Amethyst.Extensions.Plugins;

public sealed class PluginExtension(ExtensionMetadata metadata, PluginInstance pluginInstance, Assembly assembly, IExtensionRepository repository) : IExtension
{
    public Guid LoadIdentifier { get; } = Guid.NewGuid();

    public ExtensionMetadata Metadata { get; } = metadata;

    public IExtensionRepository Repository { get; } = repository;

    public IExtensionHandler Handler { get; set; } = null!;

    public PluginInstance PluginInstance { get; } = pluginInstance;

    public Assembly Assembly { get; } = assembly;
}
