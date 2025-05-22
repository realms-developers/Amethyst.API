using System.Reflection;

namespace Amethyst.Hooks.Autoloading;

public static class AutoloadUtility
{
    public static void LoadFrom(Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<AutoloadHookAttribute>() != null);

        var registerMethod = typeof(HookRegistry).GetMethod("Register", BindingFlags.Public | BindingFlags.Static)
            ?? throw new InvalidOperationException("Register method not found in HookRegistry.");

        foreach (var type in types)
        {
            var genericMethod = registerMethod.MakeGenericMethod(type);
            genericMethod.Invoke(null, null);
        }
    }
}
