using System.Reflection;
using Amethyst.Core;

namespace Amethyst.Extensions.Plugins;

public static class PluginLoader
{
    internal static List<PluginContainer> Containers = [];
    internal static string PluginsPath = Path.Combine("extensions", "plugins");
    private static int _currentLoadID;

    public static event PluginOperationHandler? OnPluginLoad;
    public static event PluginOperationHandler? OnPluginUnload;

    internal static void InvokeLoad(PluginContainer container) => OnPluginLoad?.Invoke(container);
    internal static void InvokeUnload(PluginContainer container) => OnPluginUnload?.Invoke(container);

    internal static void LoadPlugins()
    {
        // Create the directory if it doesn't exist
        if (!Directory.Exists(PluginsPath))
        {
            Directory.CreateDirectory(PluginsPath);
            AmethystLog.Main.Info("PluginLoader", $"Created plugins directory at '{PluginsPath}'");
        }

        IEnumerable<string> files = Directory.EnumerateFiles(PluginsPath, "*.dll");

        foreach (string file in files)
        {
            if (!AmethystSession.ExtensionsConfiguration.AllowedPlugins.Contains(file
                .Split('/')
                .Last()))
            {
                continue;
            }

            AmethystLog.Main.Info("PluginLoader", $"Loading '{file}'...");

            CreateContainer(file.Split('/').Last(), File.ReadAllBytes(file))
                    ?.Load();
        }
    }

    public static PluginContainer? CreateContainer(string name, byte[] data)
    {
        Assembly? asm = TryLoadAssembly(name, data);
        if (asm == null)
        {
            return null;
        }

        _currentLoadID++;

        PluginContainer container = new(_currentLoadID, data, asm);
        Containers.Add(container);

        return container;
    }

    internal static void RemovePlugin(int id)
    {
        Containers.RemoveAll(p => p.LoadID == id);
    }

    private static Assembly? TryLoadAssembly(string name, byte[] data)
    {
        try
        {
            // can throw BadImageException
            // so this is reason why we use try-catch

            Assembly assembly = Assembly.Load(data);
            return assembly;
        }
        catch (Exception ex)
        {
            AmethystLog.Main.Critical("Plugins", $"Failed to load assembly '{name}':");
            AmethystLog.Main.Critical("Plugins", ex.ToString());
        }

        return null;
    }
}
