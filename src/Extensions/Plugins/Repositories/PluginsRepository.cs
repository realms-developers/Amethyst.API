using System.Reflection;
using Amethyst.Extensions.Base;
using Amethyst.Extensions.Base.Utility;
using Amethyst.Extensions.Repositories;
using Amethyst.Extensions.Result;

namespace Amethyst.Extensions.Plugins.Repositories;

public sealed class PluginsRepository : IExtensionRepository
{
    public string Name => "StandardPluginsRepository";

    private List<IExtension> _extensions = new List<IExtension>();
    private ExtensionState _state = ExtensionState.NotInitialized;

    public IRepositoryRuler Ruler { get; set; } = new PluginsRepositoryRuler();

    public IEnumerable<IExtension> GetExtensions()
    {
        return _extensions.AsReadOnly().AsEnumerable();
    }

    public IEnumerable<ExtensionHandleResult> LoadExtensions()
    {
        if (_state == ExtensionState.Initialized)
        {
            yield break;
        }

        _extensions.Clear();

        foreach (var file in FileUtility.GetExtensions("extensions/plugins"))
        {
            var assembly = Assembly.Load(File.ReadAllBytes(file));
            var ext = AssemblyUtility.TryCreateExtension<PluginInstance>(assembly, out var attribute);

            if (ext != null)
            {
                var metadata = new ExtensionMetadata(attribute!.Name, attribute.Author, attribute.Description, attribute.Version);
                var plugin = new PluginExtension(metadata, ext, this);
                plugin.Handler = new PluginExtensionHandler(plugin);

                _extensions.Add(plugin);

                yield return plugin.Handler.Load();
            }
            else
            {
                yield return new ExtensionHandleResult(ExtensionResult.ExternalError, $"Failed to load plugin from {file}");
            }
        }
    }

    public IEnumerable<ExtensionHandleResult> UnloadExtensions()
    {
        if (_state != ExtensionState.Initialized)
        {
            yield break;
        }

        foreach (var ext in _extensions)
        {
            yield return ext.Handler.Unload();
        }

        _extensions.Clear();
        _state = ExtensionState.Deinitialized;
    }
}
