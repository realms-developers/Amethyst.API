using System.Reflection;
using Amethyst.Extensions.Base;
using Amethyst.Extensions.Base.Metadata;
using Amethyst.Extensions.Base.Repositories;
using Amethyst.Extensions.Base.Result;
using Amethyst.Extensions.Base.Utility;

namespace Amethyst.Extensions.Plugins.Repositories;

public sealed class PluginsRepository : IExtensionRepository
{
    public string Name => "StandardPluginsRepository";

    private readonly List<IExtension> _extensions = [];
    private readonly List<ExtensionHandleResult> _results = [];

    private IReadOnlyDictionary<IExtension, ExtensionHandleResult>? _extensionMap;
    private ExtensionState _state = ExtensionState.NotInitialized;

    public IRepositoryRuler Ruler { get; set; } = new PluginsRepositoryRuler();

    public IEnumerable<IExtension> Extensions => _extensions.AsReadOnly().AsEnumerable();

    public IEnumerable<ExtensionHandleResult> Results => _results.AsReadOnly().AsEnumerable();

    public IReadOnlyDictionary<IExtension, ExtensionHandleResult> ExtensionMap
    {
        get
        {
            if (_extensionMap != null)
            {
                return _extensionMap;
            }

            _extensionMap = _extensions
                .Zip(_results, (ext, result) => new { Extension = ext, Result = result })
                .Where(x => x.Result.LoadIdentifier != Guid.Empty)
                .ToDictionary(
                    x => x.Extension,
                    x => x.Result)
                .AsReadOnly();

            return _extensionMap;
        }
    }

    public IEnumerable<ExtensionHandleResult> LoadExtensions()
    {
        if (_state == ExtensionState.Initialized)
        {
            return [];
        }

        _extensions.Clear();
        _results.Clear();
        _extensionMap = null;

        var results = new List<ExtensionHandleResult>();

        foreach (string file in FileUtility.GetExtensions("plugins"))
        {
            try
            {
                var context = new PluginLoadContext(file);
                Assembly assembly = context.LoadPlugin();
                PluginInstance? ext = AssemblyUtility.TryCreateExtension<PluginInstance>(
                    assembly, out ExtensionMetadataAttribute? attribute);

                if (ext != null && attribute != null)
                {
                    var metadata = new ExtensionMetadata(
                        attribute.Name,
                        attribute.Author,
                        attribute.Description,
                        attribute.Version);

                    var plugin = new PluginExtension(metadata, ext, assembly, this, context);

                    plugin.Handler = new PluginExtensionHandler(plugin);

                    ext.Root = plugin;

                    _extensions.Add(plugin);

                    ExtensionHandleResult result = plugin.Handler.Load();
                    _results.Add(result);
                    results.Add(result);
                }
                else
                {
                    results.Add(new ExtensionHandleResult(
                        Guid.Empty,
                        ExtensionResult.ExternalError,
                        $"Failed to create plugin instance from {file}"));
                }
            }
            catch (Exception ex)
            {
                results.Add(new ExtensionHandleResult(
                    Guid.Empty,
                    ExtensionResult.InternalError,
                    $"Error loading plugin from {file}: {ex.Message}"));
            }
        }

        _state = ExtensionState.Initialized;

        return results;
    }

    public IEnumerable<ExtensionHandleResult> UnloadExtensions()
    {
        if (_state != ExtensionState.Initialized)
        {
            return [];
        }

        _results.Clear();
        var results = new List<ExtensionHandleResult>();

        foreach (IExtension ext in _extensions)
        {
            try
            {
                ExtensionHandleResult result = ext.Handler.Unload();
                _results.Add(result);
                results.Add(result);
            }
            catch (Exception ex)
            {
                results.Add(new ExtensionHandleResult(
                    ext.LoadIdentifier,
                    ExtensionResult.InternalError,
                    $"Error unloading {ext.Metadata.Name}: {ex.Message}"));
            }
        }

        _extensions.Clear();
        _extensionMap = null;
        _state = ExtensionState.Deinitialized;

        return results;
    }
}
