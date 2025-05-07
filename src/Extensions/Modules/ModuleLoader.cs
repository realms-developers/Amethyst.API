using System.Reflection;
using Amethyst.Commands;
using Amethyst.Core;

namespace Amethyst.Extensions.Modules;

public static class ModuleLoader
{
    internal static List<AmethystModule> Modules = [];
    internal static string ModulesPath = Path.Combine("extensions", "modules");

    internal static List<string> LogSkipped = [];
    internal static List<string> LogLoaded = [];
    internal static Dictionary<string, Exception> LogFailed = [];

    internal static void LoadModules()
    {
        // Create the directory if it doesn't exist
        if (!Directory.Exists(ModulesPath))
        {
            Directory.CreateDirectory(ModulesPath);
            AmethystLog.Main.Info(nameof(ModuleLoader), $"Created modules directory at '{ModulesPath}'");
        }

        LoadExtended();

        LoadFromDirectory(ModulesPath);
    }

    internal static void LoadExtended()
    {
        IEnumerable<string> dirs = Directory.EnumerateDirectories(ModulesPath, "*", SearchOption.TopDirectoryOnly)
            .OrderBy(Path.GetFileName);

        foreach (string dir in dirs)
        {
            LoadFromDirectory(dir); // loads all .dll modules from directory.

            string localizationDir = Path.Combine(dir, "localization");

            IEnumerable<string> cultures = Directory.EnumerateDirectories(localizationDir, "*", SearchOption.TopDirectoryOnly);

            foreach (string culture in cultures)
            {
                string fixedCulture = Path.GetDirectoryName(culture)!;

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
            // Get filename once (cleaner and cross-platform)
            string fileName = Path.GetFileName(file);

            if (!AmethystSession.ExtensionsConfiguration.AllowedModules.Contains(fileName))
            {
                LogSkipped.Add(fileName);
                //AmethystLog.Main.Warning(nameof(ModuleLoader), $"Skipped '{fileName}'.");
                continue;
            }

            AmethystLog.Main.Info(nameof(ModuleLoader), $"Loading '{fileName}'..."); // Log filename instead of full path

            Assembly assembly = Assembly.LoadFrom(Path.Combine(Directory.GetCurrentDirectory(), file)); // Still needs the full path to load

            foreach (Type type in assembly.GetTypes())
            {
                TryLoadModule(assembly, type);
            }
        }
    }

    internal static void TryLoadModule(Assembly assembly, Type type)
    {
        AmethystModule? module = CreateInstance(type);
        if (module == null)
        {
            return;
        }

        try
        {
            module.InitializeDelegate.ForEach(p => p());
            CommandsManager.ImportCommands(assembly, null);

            LogLoaded.Add(module.Name);
        }
        catch (Exception ex)
        {
            LogFailed.Add(module.Name, ex);
            // AmethystLog.Main.Critical(nameof(ModuleLoader), $"Failed to load module '{module.Name}':");
            // AmethystLog.Main.Critical(nameof(ModuleLoader), ex.ToString());
        }

        Modules.Add(module);
    }

    internal static AmethystModule? CreateInstance(Type type)
    {
        AmethystModuleAttribute? attribute = type.GetCustomAttribute<AmethystModuleAttribute>();

        if (attribute == null)
        {
            return null;
        }

        List<ModuleInitializeDelegate> initDelegates = [];

        foreach (MethodInfo method in type.GetMethods())
        {
            ModuleInitializeAttribute? initAttribute = method.GetCustomAttribute<ModuleInitializeAttribute>();
            if (initAttribute == null)
            {
                continue;
            }

            if (method.ReturnType != typeof(void) || method.GetParameters().Length != 0)
            {
                continue;
            }

            initDelegates.Add((Delegate.CreateDelegate(typeof(ModuleInitializeDelegate), method) as ModuleInitializeDelegate)!);
        }

        AmethystModule module = new(attribute.Name, initDelegates);
        return module;
    }
}
