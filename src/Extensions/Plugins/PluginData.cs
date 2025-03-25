using System.Reflection;

namespace Amethyst.Extensions.Plugins;

public sealed class PluginData
{
    internal PluginData(string path, Assembly assembly)
    {
        Path = path;
        Assembly = assembly;
    }

    public string Path { get; }
    public Assembly Assembly { get; }
}