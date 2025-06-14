using System.Reflection;
using System.Runtime.Loader;
using Amethyst.Extensions.Modules;
using Amethyst.Kernel.Kernel;

namespace Amethyst.Extensions.Plugins;

public sealed class PluginLoadContext(string pluginPath) : AssemblyLoadContext($"Plugin:{Path.GetFileNameWithoutExtension(pluginPath)}",
          isCollectible: true)
{
    private readonly AssemblyDependencyResolver _resolver = new(pluginPath);

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

        // 3. Try plugin's local dependencies
        string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        return assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : null;
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        string? libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        return libraryPath != null ?
            LoadUnmanagedDllFromPath(libraryPath) :
            IntPtr.Zero;
    }

    public Assembly LoadPlugin() => LoadFromAssemblyPath(pluginPath);
}
