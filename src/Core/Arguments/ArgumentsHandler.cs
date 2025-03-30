using System.Reflection;

namespace Amethyst.Core.Arguments;

public static class ArgumentsHandler
{
    internal static Dictionary<string, ArgumentCommandInfo> RegisteredCommands = new(32);

    internal static void Initialize()
    {
        LoadCommands(typeof(ArgumentsHandler).Assembly);
    }

    private static void LoadCommands(Assembly assembly)
    {
        foreach (Type type in assembly.GetTypes())
        {
            LoadCommands(type);
        }
    }

    private static void LoadCommands(Type type)
    {
        foreach (MethodInfo methodInfo in type.GetMethods())
        {
            ArgumentCommandAttribute? attribute = methodInfo.GetCustomAttribute<ArgumentCommandAttribute>();

            if (!methodInfo.IsStatic || attribute is null)
            {
                continue;
            }

            if (methodInfo.ReturnParameter.ParameterType != typeof(bool) || methodInfo.GetParameters().Length != 1 ||
                methodInfo.GetParameters()[0].ParameterType != typeof(string))
            {
                continue;
            }

            var argDelegate = Delegate.CreateDelegate(typeof(ArgumentCommand), methodInfo) as ArgumentCommand;
            RegisteredCommands.Add(attribute.Name, new ArgumentCommandInfo(attribute.Description, argDelegate!));
        }
    }

    internal static void HandleCommand(string name, string input)
    {
        if (RegisteredCommands.TryGetValue(name, out ArgumentCommandInfo? value))
        {
            value.CommandDelegate(input);
        }
    }
}
