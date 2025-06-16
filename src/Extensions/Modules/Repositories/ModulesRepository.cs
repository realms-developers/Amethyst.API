using System.Reflection;
using Amethyst.Extensions.Base;
using Amethyst.Extensions.Base.Metadata;
using Amethyst.Extensions.Base.Repositories;
using Amethyst.Extensions.Base.Result;
using Amethyst.Extensions.Base.Utility;
using Amethyst.Systems.Commands.Dynamic.Utilities;

namespace Amethyst.Extensions.Modules.Repositories;

public sealed class ModulesRepository : IExtensionRepository
{
    public string Name => "mdl.main";

    private readonly List<IExtension> _extensions = [];
    private readonly List<ExtensionHandleResult> _results = [];

    private IReadOnlyDictionary<IExtension, ExtensionHandleResult>? _extensionMap;
    private ExtensionState _state = ExtensionState.NotInitialized;

    public IRepositoryRuler Ruler { get; set; } = new ModulesRepositoryRuler();

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

        foreach (string file in FileUtility.GetExtensions("modules"))
        {
            if (!Ruler.IsExtensionAllowed(Path.GetFileName(file)))
            {
                var errorResult = new ExtensionHandleResult(Guid.Empty,
                    ExtensionResult.NotAllowed,
                    $"Module {Path.GetFileName(file)} is not allowed by the repository ruler.");
                _results.Add(errorResult);
                results.Add(errorResult);

                continue;
            }

            Assembly assembly = Assembly.LoadFrom(Path.Combine(Directory.GetCurrentDirectory(), file)); // Still needs the full path to load
            Type? ext = AssemblyUtility.TryFindExtensionType(assembly, out ExtensionMetadataAttribute? attribute);

            if (ext != null)
            {
                var metadata = new ExtensionMetadata(attribute!.Name, attribute.Author,
                    attribute.Description, attribute.Version);

                MethodInfo? attributedMethod = ext.GetMethods()
                    .FirstOrDefault(m => m.GetCustomAttribute<ModuleInitializeAttribute>() != null
                        && m.IsStatic
                        && m.ReturnType == typeof(void)
                        && m.GetParameters().Length == 0);

                if (attributedMethod != null)
                {
                    ModuleInitializer initializer = attributedMethod.CreateDelegate<ModuleInitializer>();

                    var module = new ModuleExtension(metadata, this, assembly, initializer);

                    module.Handler = new ModuleExtensionHandler(module);

                    _extensions.Add(module);

                    ExtensionHandleResult result = module.Handler.Load();
                    _results.Add(result);
                    results.Add(result);

                    ImportUtility.ImportFrom(assembly, module.LoadIdentifier);
                }
            }
            else
            {
                var errorResult = new ExtensionHandleResult(Guid.Empty,
                    ExtensionResult.ExternalError,
                    $"Failed to load Module from {file}");

                _results.Add(errorResult);
                results.Add(errorResult);
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
            ExtensionHandleResult result = ext.Handler.Unload();
            _results.Add(result);
            results.Add(result);
        }

        _extensions.Clear();
        _extensionMap = null;
        _state = ExtensionState.Deinitialized;

        return results;
    }
}
