using System.Reflection;
using Amethyst.Commands;
using Amethyst.Core;

namespace Amethyst.Extensions.Modules;

public static class ModuleLoader
{
    internal static List<AmethystModule> Modules = [];
    internal static string ModulesPath = Path.Combine("extensions", "modules");

    internal static void LoadModules()
    {
        // Create the directory if it doesn't exist
        if (!Directory.Exists(ModulesPath))
        {
            Directory.CreateDirectory(ModulesPath);
            AmethystLog.Main.Info("ModuleLoader", $"Created modules directory at '{ModulesPath}'");
        }

        IEnumerable<string> files = Directory.EnumerateFiles(ModulesPath, "*.dll");

        foreach (string file in files)
        {
            if (AmethystSession.ExtensionsConfiguration.AllowedModules.Contains(file.Split('/').Last()) == false)
            {
                continue;
            }

            AmethystLog.Main.Info("ModuleLoader", $"Loading '{file}'...");

            Assembly assembly = Assembly.LoadFrom(file);
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

        module.LoadDependencies();
        try
        {
            module.InitializeDelegate.ForEach(p => p());
            CommandsManager.ImportCommands(assembly, null);
        }
        catch (Exception ex)
        {
            AmethystLog.Main.Critical("ModuleLoader", $"Failed to load module '{module.Name}':");
            AmethystLog.Main.Critical("ModuleLoader", ex.ToString());
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

        AmethystModule module = new(attribute.Name, attribute.Dependencies, initDelegates);
        return module;
    }
}
