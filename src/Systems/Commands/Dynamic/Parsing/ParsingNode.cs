using System.Reflection.Emit;
using Amethyst.Systems.Users.Base;

namespace Amethyst.Systems.Commands.Dynamic.Parsing;

public static class ParsingNode
{
    internal static readonly Dictionary<Type, ArgumentParser> Parsers = [];

    internal static void Initialize()
    {
        Parsers.Add(typeof(string), static (IAmethystUser user, string inputText, out string? errorMessage) =>
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

    public static void AddParser(Type type, ArgumentParser parser)
    {
        if (Parsers.ContainsKey(type))
            return;

        Parsers.Add(type, parser);
    }
    public static void RemoveParser(Type type)
    {
        Parsers.Remove(type);
    }

    private static ArgumentParser GenerateGenericParser(Type type)
    {
        var method = new DynamicMethod(
            $"Parse_{type.Name}",
            typeof(object),
            [typeof(IAmethystUser), typeof(string), typeof(string).MakeByRefType()],
            typeof(ParsingNode).Module,
            skipVisibility: true
        );

        var il = method.GetILGenerator();

        var tryParseMethod = type.GetMethod(
            "TryParse",
            new[] { typeof(string), type.MakeByRefType() }
        );

        var lblParseSuccess = il.DefineLabel();
        var lblAfterParse = il.DefineLabel();
        var valueLocal = il.DeclareLocal(type);
        var resultLocal = il.DeclareLocal(typeof(bool));

        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Ldloca_S, valueLocal);
        il.Emit(OpCodes.Call, tryParseMethod!);
        il.Emit(OpCodes.Stloc, resultLocal);

        il.Emit(OpCodes.Ldloc, resultLocal);
        il.Emit(OpCodes.Brtrue_S, lblParseSuccess);

        il.Emit(OpCodes.Ldarg_2);
        il.Emit(OpCodes.Ldstr, "PARSE_FAILED");
        il.Emit(OpCodes.Stind_Ref);

        if (type.IsValueType)
        {
            var defaultLocal = il.DeclareLocal(type);
            il.Emit(OpCodes.Ldloca_S, defaultLocal);
            il.Emit(OpCodes.Initobj, type);
            il.Emit(OpCodes.Ldloc, defaultLocal);
            il.Emit(OpCodes.Box, type);
        }
        else
        {
            il.Emit(OpCodes.Ldnull);
        }
        il.Emit(OpCodes.Br_S, lblAfterParse);

        il.MarkLabel(lblParseSuccess);

        il.Emit(OpCodes.Ldarg_2);
        il.Emit(OpCodes.Ldnull);
        il.Emit(OpCodes.Stind_Ref);

        il.Emit(OpCodes.Ldloc, valueLocal);
        if (type.IsValueType)
            il.Emit(OpCodes.Box, type);

        il.MarkLabel(lblAfterParse);
        il.Emit(OpCodes.Ret);

        var parser = (ArgumentParser)method.CreateDelegate(typeof(ArgumentParser));
        return parser;
    }
}
