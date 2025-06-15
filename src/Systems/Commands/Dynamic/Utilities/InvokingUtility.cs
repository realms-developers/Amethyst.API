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
            $"Invoker_{method.Name}_{Guid.NewGuid().ToString().Replace("-", "")}",
            typeof(void),
            [typeof(object?[])],
            typeof(InvokingUtility).Module,
            true);

        ILGenerator il = dynamicMethod.GetILGenerator();
        ParameterInfo[] parameters = method.GetParameters();

        if (parameters.Length != 0)
        {
            Label argsOk = il.DefineLabel();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldlen);
            il.Emit(OpCodes.Conv_I4);
            il.Emit(OpCodes.Ldc_I4, parameters.Length);
            il.Emit(OpCodes.Beq, argsOk);

            il.Emit(OpCodes.Newobj, typeof(TargetParameterCountException).GetConstructor(Type.EmptyTypes)!);
            il.Emit(OpCodes.Throw);

            il.MarkLabel(argsOk);
        }

        for (int i = 0; i < parameters.Length; i++)
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldc_I4, i);
            il.Emit(OpCodes.Ldelem_Ref);

            if (parameters[i].ParameterType.IsValueType)
            {
                Label notNull = il.DefineLabel();
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Brtrue_S, notNull);
                il.Emit(OpCodes.Newobj, typeof(ArgumentNullException).GetConstructor(Type.EmptyTypes)!);
                il.Emit(OpCodes.Throw);
                il.MarkLabel(notNull);
            }
            else
            {
                Label next = il.DefineLabel();
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Brfalse_S, next);
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Isinst, parameters[i].ParameterType);
                il.Emit(OpCodes.Brtrue_S, next);
                il.Emit(OpCodes.Newobj, typeof(InvalidCastException).GetConstructor(Type.EmptyTypes)!);
                il.Emit(OpCodes.Throw);
                il.MarkLabel(next);
            }

            Type paramType = parameters[i].ParameterType;
            if (paramType.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, paramType);
            }
            else
            {
                il.Emit(OpCodes.Castclass, paramType);
            }
        }

        il.Emit(OpCodes.Call, method);
        il.Emit(OpCodes.Ret);

        return dynamicMethod;
    }

    internal static Action<object?[]> CreateInvoker(DynamicMethod method)
    {
        Delegate invoker = method.CreateDelegate(typeof(Action<object?[]>));
        return (Action<object?[]>)invoker;
    }

    internal static object?[]? CreateArguments(DynamicCommandInvoker invoker, CommandInvokeContext ctx)
    {
        ParameterInfo[] methodParameters = invoker.Method.GetParameters();
        List<object?> args = [ctx.User, ctx];

        int offset = 2;
        for (int i = offset; i < methodParameters.Length; i++)
        {
            ParameterInfo parameter = methodParameters[i];

            if (i - offset >= ctx.Args.Length)
            {
                if (methodParameters[i].IsOptional)
                {
                    args.Add(methodParameters[i].DefaultValue!);
                    continue;
                }

                ctx.Messages.ReplyError("commands.notEnoughArguments");
                return null;
            }

            ArgumentParser parser = ParsingNode.Parsers[parameter.ParameterType];
            object? arg = parser(ctx.User, ctx.Args[i - offset], out string? errorMessage);
            if (arg == null || errorMessage != null)
            {
                if (invoker.Command.Metadata.Syntax?[ctx.Messages.Language]?.Length > i)
                {
                    ctx.Messages.ReplyError("commands.invalidSyntax+arg", invoker.Command.Metadata.Syntax);
                }
                else
                {
                    ctx.Messages.ReplyError("commands.invalidSyntax");
                }

                if (errorMessage != null)
                {
                    ctx.Messages.ReplyError(errorMessage);
                }

                return null;
            }
            else
            {
                args.Add(arg);
            }
        }

        return [.. args];
    }

}
