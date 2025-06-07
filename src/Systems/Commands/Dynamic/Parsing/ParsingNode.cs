using System.Reflection.Emit;
using Amethyst.Kernel;
using Amethyst.Network.Structures;
using Amethyst.Server.Entities;
using Amethyst.Server.Entities.Players;
using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Players;

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

        Parsers.Add(typeof(PlayerEntity), static (IAmethystUser user, string inputText, out string? errorMessage) =>
        {
            errorMessage = null;
            if (inputText == "@me")
            {
                if (user is PlayerUser plrUser)
                {
                    return plrUser.Player;
                }
                else
                {
                    errorMessage = "YOU_ARE_NOT_A_PLAYER";
                    return null;
                }
            }

            if (inputText.StartsWith('@') && int.TryParse(inputText[1..], out int index))
            {
                if (index < 0 || index >= AmethystSession.Profile.MaxPlayers)
                {
                    errorMessage = "PLAYER_NOT_FOUND";
                    return null;
                }

                var player = EntityTrackers.Players[index];
                if (player != null && player.Active)
                {
                    return player;
                }
                else
                {
                    errorMessage = "PLAYER_NOT_FOUND";
                    return null;
                }
            }

            foreach (var player in EntityTrackers.Players)
            {
                if (player != null && player.Active && player.Name.Equals(inputText, StringComparison.OrdinalIgnoreCase))
                {
                    return player;
                }
            }

            foreach (var player in EntityTrackers.Players)
            {
                if (player != null && player.Active && player.Name.StartsWith(inputText, StringComparison.OrdinalIgnoreCase))
                {
                    return player;
                }
            }
            errorMessage = "PLAYER_NOT_FOUND";
            return null;
        });

        Parsers.Add(typeof(NetColor), static (IAmethystUser user, string inputText, out string? errorMessage) =>
        {
            errorMessage = null;
            // by R,G,B
            if (inputText.Contains(','))
            {
                var parts = inputText.Split(',');
                if (parts.Length == 3 &&
                    byte.TryParse(parts[0].Trim(), out byte r) &&
                    byte.TryParse(parts[1].Trim(), out byte g) &&
                    byte.TryParse(parts[2].Trim(), out byte b))
                {
                    errorMessage = null;
                    return new NetColor(r, g, b);
                }
            }

            // by hex
            if (inputText.Length == 6 && int.TryParse(inputText, System.Globalization.NumberStyles.HexNumber, null, out int packedValue))
            {
                errorMessage = null;
                return new NetColor(packedValue);
            }

            // by hex with #
            if (inputText.Length == 7 && inputText.StartsWith('#') && int.TryParse(inputText[1..], System.Globalization.NumberStyles.HexNumber, null, out packedValue))
            {
                errorMessage = null;
                return new NetColor(packedValue);
            }

            errorMessage = "INVALID_COLOR_FORMAT";
            return null;
        });
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
