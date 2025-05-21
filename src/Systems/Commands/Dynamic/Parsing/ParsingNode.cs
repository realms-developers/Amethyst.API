using System.Reflection.Emit;
using Amethyst.Systems.Users.Base;

namespace Amethyst.Systems.Commands.Dynamic.Parsing;

public static class ParsingNode
{
    internal static readonly Dictionary<Type, ArgumentParser> Parsers = [];

    internal static void Initialize()
    {
        Parsers.Add(typeof(string), (IAmethystUser user, string inputText, out string? errorMessage) =>
        {
            errorMessage = null;
            return inputText;
        });

        foreach (var type in new[]
        {
            typeof(bool),
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal)
        })
        {
            Parsers.Add(type, GenerateGenericParser(type));
        }
    }

    private static ArgumentParser GenerateGenericParser(Type type)
    {
        var genericType = type.GetGenericArguments()[0];
        var method = new DynamicMethod(
            $"Parse_{genericType.Name}",
            typeof(object),
            [typeof(string), typeof(string).MakeByRefType()],
            typeof(ParsingNode).Module,
            skipVisibility: true
        );

        var il = method.GetILGenerator();
        var tryParse = genericType.GetMethod(
            "TryParse",
            [typeof(string), genericType.MakeByRefType()]
        );

        if (tryParse == null)
            throw new InvalidOperationException($"No TryParse method found for {genericType}");

        // Declare local: <TYPE> value
        var valueLocal = il.DeclareLocal(genericType);
        // Declare local: bool result
        var resultLocal = il.DeclareLocal(typeof(bool));

        // Call <TYPE>.TryParse(input, out value)
        il.Emit(OpCodes.Ldarg_0); // input
        il.Emit(OpCodes.Ldloca_S, valueLocal); // out value
        il.EmitCall(OpCodes.Call, tryParse, null);
        il.Emit(OpCodes.Stloc, resultLocal);

        // if (result)
        var successLabel = il.DefineLabel();
        il.Emit(OpCodes.Ldloc, resultLocal);
        il.Emit(OpCodes.Brtrue_S, successLabel);

        // else: errorMessage = "PARSE_FAILED"; return null;
        il.Emit(OpCodes.Ldarg_1); // ref errorMessage
        il.Emit(OpCodes.Ldstr, "PARSE_FAILED");
        il.Emit(OpCodes.Stind_Ref);
        il.Emit(OpCodes.Ldnull);
        il.Emit(OpCodes.Ret);

        // success: errorMessage = null; return value;
        il.MarkLabel(successLabel);
        il.Emit(OpCodes.Ldarg_1); // ref errorMessage
        il.Emit(OpCodes.Ldnull);
        il.Emit(OpCodes.Stind_Ref);
        il.Emit(OpCodes.Ldloc, valueLocal);
        if (genericType.IsValueType)
            il.Emit(OpCodes.Box, genericType);
        il.Emit(OpCodes.Ret);

        var parser = (ArgumentParser)method.CreateDelegate(typeof(ArgumentParser));
        return parser;
    }
}
