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

    public static IReadOnlyList<PluginContainer> LoadedPluginContainers => Containers.ToList().AsReadOnly();

    internal static List<string> LogSkipped = [];
    internal static List<string> LogLoaded = [];
    internal static Dictionary<string, Exception> LogFailed = [];
    internal static bool InFirstLoadStage = true;

    internal static void InvokeLoad(PluginContainer container) => OnPluginLoad?.Invoke(container);
    internal static void InvokeUnload(PluginContainer container) => OnPluginUnload?.Invoke(container);

    internal static void LoadPlugins()
    {
        // Create the directory if it doesn't exist
        if (!Directory.Exists(PluginsPath))
        {
            Directory.CreateDirectory(PluginsPath);
            AmethystLog.Main.Info(nameof(PluginLoader), $"Created plugins directory at '{PluginsPath}'");
        }

        LoadExtended();

        LoadFromDirectory(PluginsPath);

        InFirstLoadStage = false;
    }

    internal static void LoadExtended()
    {
        IEnumerable<string> dirs = Directory.EnumerateDirectories(PluginsPath, "*", SearchOption.TopDirectoryOnly)
            .OrderBy(Path.GetFileName);

        foreach (string dir in dirs)
        {
            LoadFromDirectory(dir); // loads all .dll plugins from directory.

            string localizationDir = Path.Combine(dir, "localization");

            IEnumerable<string> cultures = Directory.EnumerateDirectories(localizationDir, "*", SearchOption.TopDirectoryOnly);

            foreach (string culture in cultures)
            {
                string fixedCulture = new DirectoryInfo(culture).Name;

                Localization.Load(fixedCulture, localizationDir);
            }
        }
    }

    internal static void LoadFromDirectory(string path)
    {
        IEnumerable<string> files = Directory.EnumerateFiles(path, "*.dll", SearchOption.TopDirectoryOnly)
            .OrderBy(Path.GetFileName);

        foreach (string file in files)
        {
            // Get filename once using Path.GetFileName
            string fileName = Path.GetFileName(file);

            if (!AmethystSession.ExtensionsConfiguration.AllowedPlugins.Contains(fileName))
            {
                LogSkipped.Add(fileName);
                //AmethystLog.Main.Warning(nameof(PluginLoader), $"Skipped '{fileName}'.");
                continue;
            }

            //AmethystLog.Main.Info(nameof(PluginLoader), $"Loading '{fileName}'...");

            // Reuse fileName here too
            PluginContainer? container = CreateContainer(fileName, File.ReadAllBytes(file));
            if (container == null)
            {
                continue;
            }

            if (container.Load())
            {
                LogLoaded.Add(fileName);
            }
            else
            {
                LogFailed.Add(fileName, new NoInstancePluginException(fileName));
            }
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

        PluginContainer container = new(name, _currentLoadID, data, asm);
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
            LogFailed.Add(name, ex);
            // AmethystLog.Main.Critical(nameof(PluginLoader), $"Failed to load assembly '{name}':");
            // AmethystLog.Main.Critical(nameof(PluginLoader), ex.ToString());
        }

        return null;
    }
}
