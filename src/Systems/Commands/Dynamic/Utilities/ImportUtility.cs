using System.Reflection;
using Amethyst.Systems.Commands.Base;
using Amethyst.Systems.Commands.Base.Metadata;
using Amethyst.Systems.Commands.Dynamic.Attributes;
using Amethyst.Systems.Users.Base;

namespace Amethyst.Systems.Commands.Dynamic.Utilities;

internal static class ImportUtility
{
    internal static Guid CoreIdentifier { get; } = Guid.NewGuid();

    internal static IEnumerable<DynamicCommand> ImportFrom(Assembly assembly, Guid identifier)
    {
        foreach (var type in assembly.GetTypes())
        {
            foreach (var item in ImportFromType(type, identifier))
            {
                yield return item;
            }
        }
    }

    internal static IEnumerable<DynamicCommand> ImportFromType(Type type, Guid identifier)
    {
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
        foreach (var method in methods)
        {
            var baseAttr = method.GetCustomAttribute<CommandAttribute>();

            if (baseAttr == null)
                continue;

            if (!TryPreCreateCommand(method, out var userType))
            {
                continue;
            }

            bool noLog = method.GetCustomAttribute<CommandNoLogAttribute>() != null;
            string? permission = method.GetCustomAttribute<CommandPermissionAttribute>()?.Permission;

            var repoAttr = method.GetCustomAttribute<CommandRepositoryAttribute>();
            CommandRepository repo = repoAttr == null ? CommandsOrganizer.Shared :
                                        CommandsOrganizer.GetRepository(repoAttr.Repository) ?? CommandsOrganizer.Shared;

            CommandSyntax? syntax = null;
            foreach (var syntaxAttr in method.GetCustomAttributes<CommandSyntaxAttribute>())
            {
                syntax ??= new CommandSyntax(syntaxAttr.Culture);
                syntax.Add(syntaxAttr.Culture, syntaxAttr.Syntax);
            }

            CommandMetadata metadata = new CommandMetadata(baseAttr.Names, baseAttr.Description, syntax, noLog ? CommandRules.NoLogging : CommandRules.None, permission);

            DynamicCommand command = new DynamicCommand(identifier, method, repo, metadata, userType);

            repo.Add(command);

            yield return command;
        }
    }

    private static bool TryPreCreateCommand(MethodInfo method, out Type userType)
    {
        // needs to be public static void method, with CommandInvokeContext as second parameter and IAmethystUser OR nested type from IAmethystUser  as first

        userType = typeof(IAmethystUser);

        if (!method.IsPublic || !method.IsStatic || method.ReturnType != typeof(void))
            return false;

        var parameters = method.GetParameters();
        if (parameters.Length == 0)
            return false;

        if (parameters[0].ParameterType == typeof(IAmethystUser))
        {
            return true;
        }
        else if (parameters[0].ParameterType.IsNested)
        {
            var baseType = parameters[0].ParameterType.BaseType;
            if (baseType == null || baseType != typeof(IAmethystUser))
                return false;

            userType = baseType;
            return true;
        }

        if (parameters[1].ParameterType != typeof(CommandInvokeContext))
            return false;



        return true;
    }
}
