using System.Reflection;
using Amethyst.Commands;
using Amethyst.Core;

namespace Amethyst.Extensions.Modules;

public static class ModuleLoader
{
    internal static List<AmethystModule> Modules = new List<AmethystModule>();
    internal static string ModulesPath = Path.Combine("extensions", "modules");

    internal static void LoadModules()
    {
        var files = Directory.EnumerateFiles(ModulesPath, "*.dll");

        foreach (var file in files)
        {
            if (AmethystSession.ExtensionsConfiguration.AllowedModules.Contains(file.Split('/').Last()) == false)
                continue;

            AmethystLog.Main.Info("ModuleLoader", $"Loading '{file}'...");

            Assembly assembly = Assembly.LoadFrom(file);
            foreach (var type in assembly.GetTypes())
                TryLoadModule(assembly, type);
        }
    }

    internal static void TryLoadModule(Assembly assembly, Type type)
    {
        var module = CreateInstance(type);
        if (module == null) return;

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
        var attribute = type.GetCustomAttribute<AmethystModuleAttribute>();

        if (attribute == null) return null;

        List<ModuleInitializeDelegate> initDelegates = new List<ModuleInitializeDelegate>();

        foreach (var method in type.GetMethods())
        {
            var initAttribute = method.GetCustomAttribute<ModuleInitializeAttribute>();
            if (initAttribute == null) continue;

            if (method.ReturnType != typeof(void) || method.GetParameters().Length != 0) continue;

            initDelegates.Add((Delegate.CreateDelegate(typeof(ModuleInitializeDelegate), method) as ModuleInitializeDelegate)!);
        }

        AmethystModule module = new AmethystModule(attribute.Name, attribute.Dependencies, initDelegates);
        return module;
    }
}
