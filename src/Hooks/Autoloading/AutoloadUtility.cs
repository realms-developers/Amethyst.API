using System.Reflection;

namespace Amethyst.Hooks.Autoloading;

public static class AutoloadUtility
{
    public static void LoadFrom(Assembly assembly)
    {
        IEnumerable<Type> types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract);

        MethodInfo registerMethod = typeof(HookRegistry).GetMethod("RegisterHook", BindingFlags.Public | BindingFlags.Static)
            ?? throw new InvalidOperationException("Register method not found in HookRegistry.");

        foreach (Type? type in types)
        {
            AutoloadHookAttribute? attr = type.GetCustomAttribute<AutoloadHookAttribute>();
            if (attr == null)
            {
                continue;
            }

            MethodInfo genericMethod = registerMethod.MakeGenericMethod(type);
            genericMethod.Invoke(null, [attr.CanBeIgnored, attr.CanBeChanged]);
        }
    }
}
