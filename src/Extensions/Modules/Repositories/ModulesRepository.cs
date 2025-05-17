using System.Reflection;
using Amethyst.Extensions.Base;
using Amethyst.Extensions.Base.Utility;
using Amethyst.Extensions.Repositories;
using Amethyst.Extensions.Result;

namespace Amethyst.Extensions.Modules.Repositories;

public sealed class ModulesRepository : IExtensionRepository
{
    public string Name => "StandardModuleRepository";

    private List<IExtension> _extensions = new List<IExtension>();
    private ExtensionState _state = ExtensionState.NotInitialized;

    public IRepositoryRuler Ruler { get; set; } = new ModulesRepositoryRuler();

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

        foreach (var file in FileUtility.GetExtensions("extensions/Modules"))
        {
            var assembly = Assembly.Load(File.ReadAllBytes(file));
            var ext = AssemblyUtility.TryFindExtensionType(assembly, out var attribute);

            if (ext != null)
            {
                var metadata = new ExtensionMetadata(attribute!.Name, attribute.Author, attribute.Description, attribute.Version);

                var attributedMethod = ext.GetMethods().Where(p => p.GetCustomAttribute<ModuleInitializeAttribute>() != null).FirstOrDefault(p => p != null);

                if (attributedMethod != null && attributedMethod.IsStatic && attributedMethod.ReturnType == typeof(void) && attributedMethod.GetParameters().Length == 0)
                {
                    var instance = Activator.CreateInstance(ext);
                    var initializer = attributedMethod.CreateDelegate<ModuleInitializer>(instance);

                    var module = new ModuleExtension(metadata, this, initializer);
                    module.Handler = new ModuleExtensionHandler(module);

                    _extensions.Add(module);
                    yield return module.Handler.Load();
                }
            }
            else
            {
                yield return new ExtensionHandleResult(ExtensionResult.ExternalError, $"Failed to load Module from {file}");
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
