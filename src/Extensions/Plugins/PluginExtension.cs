using Amethyst.Extensions.Base;
using Amethyst.Extensions.Repositories;

namespace Amethyst.Extensions.Plugins;

public sealed class PluginExtension : IExtension
{
    public PluginExtension(ExtensionMetadata metadata, PluginInstance pluginInstance, IExtensionRepository repository)
    {
        Metadata = metadata;
        PluginInstance = pluginInstance;
        Repository = repository;
    }

    public Guid LoadIdentifier { get; } = Guid.NewGuid();

    public ExtensionMetadata Metadata { get; }

    public IExtensionRepository Repository { get; }

    public IExtensionHandler Handler { get; set; } = null!;

    public PluginInstance PluginInstance { get; set; }
}
