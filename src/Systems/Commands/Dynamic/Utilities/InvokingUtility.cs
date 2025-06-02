using System.Reflection;
using System.Reflection.Emit;
using Amethyst.Systems.Commands.Base;
using Amethyst.Systems.Commands.Dynamic.Parsing;

namespace Amethyst.Systems.Commands.Dynamic.Utilities;

internal static class InvokingUtility
{
    internal static DynamicMethod CreateDynamicMethod(MethodInfo method)
    {
        var dynamicMethod = new DynamicMethod(
            $"Invoker_{method.Name}",
            typeof(void),
            new[] { typeof(object[]) },
            method.DeclaringType!.Module, // нужен, если типы internal
            true);

        ILGenerator il = dynamicMethod.GetILGenerator();
        var parameters = method.GetParameters();


        for (int i = 0; i < parameters.Length; i++)
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldc_I4, i);
            il.Emit(OpCodes.Ldelem_Ref);

            Type paramType = parameters[i].ParameterType;
            if (paramType.IsValueType)
                il.Emit(OpCodes.Unbox_Any, paramType);
            else
                il.Emit(OpCodes.Castclass, paramType);
        }

        il.Emit(OpCodes.Call, method);
        il.Emit(OpCodes.Ret);

        return dynamicMethod;
    }

    internal static Action<object?[]> CreateInvoker(DynamicMethod method)
    {
        var invoker = method.CreateDelegate(typeof(Action<object?[]>));
        return (Action<object?[]>)invoker;
    }

    internal static object?[]? CreateArguments(DynamicCommandInvoker invoker, CommandInvokeContext ctx)
    {
        object?[] args = [ctx.User, ctx];

        var methodParameters = invoker.Method.GetParameters();
        int offset = 2;
        for (int i = offset; i < methodParameters.Length; i++)
        {
            var parameter = methodParameters[i];

            if (i >= ctx.Args.Length - offset)
            {
                if (methodParameters[i].IsOptional)
                {
                    args[i] = methodParameters[i].DefaultValue!;
                    continue;
                }

                ctx.Messages.ReplyError("commands.notEnoughArguments");
                return null;
            }

            var parser = ParsingNode.Parsers[parameter.ParameterType];
            var arg = parser(ctx.User, ctx.Args[i - offset], out var errorMessage);
            if (errorMessage != null)
            {
                if (invoker.Command.Metadata.syntax?[ctx.Messages.Language]?.Length > i)
                {
                    ctx.Messages.ReplyError("commands.invalidSyntax+arg", invoker.Command.Metadata.syntax);
                }
                else
                {
                    ctx.Messages.ReplyError("commands.invalidSyntax");
                }

                ctx.Messages.ReplyError(errorMessage);
                return null;
            }
            args[i] = arg;
        }

        return args;
    }

}
