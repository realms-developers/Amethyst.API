using System.Reflection;
using System.Runtime.Loader;

namespace Amethyst.Kernel.Kernel;

public sealed class SharedDependencyContext : AssemblyLoadContext
{
    public static readonly SharedDependencyContext Instance = new();

    private SharedDependencyContext()
        : base("SharedDependencies", isCollectible: false) { }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        string assemblyPath = Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "deps",
            $"{assemblyName.Name}.dll");

        return File.Exists(assemblyPath)
            ? LoadFromAssemblyPath(assemblyPath)
            : null;
    }

    public void PreloadAllDependencies()
    {
        Parallel.ForEach(Directory.EnumerateFiles(
            Path.Combine(AppContext.BaseDirectory, "..", "deps"), "*.dll"), file =>
            {
                try { LoadFromAssemblyPath(file); }
                catch (BadImageFormatException) { } // Skip native DLLs
            });
    }
}
