using System.Reflection;
using Amethyst.Systems.Commands.Base;
using Amethyst.Systems.Commands.Base.Metadata;
using Amethyst.Systems.Commands.Dynamic.Attributes;
using Amethyst.Systems.Users.Base;

namespace Amethyst.Systems.Commands.Dynamic.Utilities;

internal static class ImportUtility
{
    internal static Guid CoreIdentifier { get; } = Guid.NewGuid();
    internal static int CountSameNames(string name)
    {
        int count = 0;
        foreach (CommandRepository repo in CommandsOrganizer.Repositories)
        {
            foreach (ICommand command in repo.RegisteredCommands)
            {
                count += command.Metadata.Names.Count(n => n.Contains('$') ?
                    n.Split('$')[0].Equals(name, StringComparison.OrdinalIgnoreCase) :
                    n.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
        }

        return count;
    }

    internal static void ImportFrom(Assembly assembly, Guid identifier)
    {
        foreach (Type type in assembly.GetExportedTypes())
        {
            ImportFromType(type, identifier);
        }
    }

    internal static void ImportFromType(Type type, Guid identifier)
    {
        MethodInfo[] methods = type.GetMethods();
        foreach (MethodInfo method in methods)
        {
            CommandAttribute? baseAttr = method.GetCustomAttribute<CommandAttribute>();

            if (baseAttr == null)
            {
                continue;
            }

            string[] names = baseAttr.Names;
            for (int i = 0; i < baseAttr.Names.Length; i++)
            {
                string name = baseAttr.Names[i];

                int index = CountSameNames(name);
                if (index > 0)
                {
                    names[i] = $"{name}${index}";
                }
                else
                {
                    names[i] = name;
                }
            }

            if (!TryPreCreateCommand(method, out Type? userType))
            {
                continue;
            }

            bool noLog = method.GetCustomAttribute<CommandNoLogAttribute>() != null;
            string? permission = method.GetCustomAttribute<CommandPermissionAttribute>()?.Permission;

            CommandRepositoryAttribute? repoAttr = method.GetCustomAttribute<CommandRepositoryAttribute>();

            CommandRepository repo = (repoAttr != null && CommandsOrganizer.GetRepository(repoAttr.Repository) is CommandRepository r)
                         ? r : CommandsOrganizer.Shared;

            CommandSyntax? syntax = null;

            foreach (CommandSyntaxAttribute syntaxAttr in method.GetCustomAttributes<CommandSyntaxAttribute>())
            {
                syntax ??= new CommandSyntax(syntaxAttr.Culture);
                syntax.Add(syntaxAttr.Culture, syntaxAttr.Syntax);
            }

            CommandMetadata metadata = new(baseAttr.Names, baseAttr.Description, syntax, noLog ? CommandRules.NoLogging : CommandRules.None, permission);

            DynamicCommand command = new(identifier, method, repo, metadata, userType);

            repo.Add(command);
        }
    }

    private static bool TryPreCreateCommand(MethodInfo method, out Type userType)
    {
        // needs to be public static void method, with CommandInvokeContext as second parameter and IAmethystUser OR nested type from IAmethystUser  as first

        userType = typeof(IAmethystUser);

        if (!method.IsPublic || !method.IsStatic || method.ReturnType != typeof(void))
        {
            return false;
        }

        ParameterInfo[] parameters = method.GetParameters();
        if (parameters.Length < 2)
        {
            return false;
        }

        if (parameters[0].ParameterType == typeof(IAmethystUser))
        {
            return true;
        }
        else if (parameters[0].ParameterType.IsNested)
        {
            Type? baseType = parameters[0].ParameterType.BaseType;
            if (baseType == null || baseType != typeof(IAmethystUser))
            {
                return false;
            }

            userType = baseType;
            return true;
        }

        if (parameters[1].ParameterType != typeof(CommandInvokeContext))
        {
            return false;
        }

        return true;
    }
}
