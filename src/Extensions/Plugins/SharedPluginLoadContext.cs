using System.Reflection;
using System.Runtime.Loader;
using Amethyst.Extensions.Modules;
using Amethyst.Kernel.Kernel;

namespace Amethyst.Extensions.Plugins;

public sealed class SharedPluginLoadContext : AssemblyLoadContext
{
    private static readonly Lazy<SharedPluginLoadContext> _instance = new(() => new SharedPluginLoadContext());
    public static SharedPluginLoadContext Instance => _instance.Value;

    private readonly Dictionary<string, AssemblyDependencyResolver> _resolvers = [];

    private SharedPluginLoadContext() : base("SharedPlugins", isCollectible: false)
    {
    }

    public void AddPluginPath(string pluginPath)
    {
        if (!_resolvers.ContainsKey(pluginPath))
        {
            _resolvers[pluginPath] = new AssemblyDependencyResolver(pluginPath);
        }
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        // 1. Try shared dependencies first
        if (SharedDependencyContext.Instance.LoadFromAssemblyName(assemblyName) is { } sharedAssembly)
        {
            return sharedAssembly;
        }

        // 2. Try modules
        if (ModuleLoadContext.Instance.LoadFromAssemblyName(assemblyName) is { } moduleAssembly)
        {
            return moduleAssembly;
        }

        // 3. Try all plugin resolvers
        foreach (AssemblyDependencyResolver resolver in _resolvers.Values)
        {
            string? assemblyPath = resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }
        }

        return null;
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        foreach (AssemblyDependencyResolver resolver in _resolvers.Values)
        {
            string? libraryPath = resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }
        }

        return IntPtr.Zero;
    }

    public Assembly LoadPlugin(string pluginPath)
    {
        AddPluginPath(pluginPath);
        return LoadFromAssemblyPath(pluginPath);
    }
}
