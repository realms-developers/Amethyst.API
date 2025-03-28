using System.Reflection;
using Amethyst.Commands.Parsing;
using Amethyst.Text;

namespace Amethyst.Commands;

public sealed class CommandRunner
{
    internal CommandRunner(CommandData data) => Data = data;

    public CommandData Data { get; }
    public bool IsDisabled { get; set; }

    public bool NameEquals(string text)
    {
        string[] commandParts = Data.Name.Split();
        string[] inputParts = text.Split();

        // Replace LINQ SequenceEqual and Take with manual checks
        if (commandParts.Length > inputParts.Length)
        {
            return false;
        }

        for (int i = 0; i < commandParts.Length; i++)
        {
            if (commandParts[i] != inputParts[i])
            {
                return false;
            }
        }

        return true;
    }

    public void Run(ICommandSender sender, string arguments) =>
        Run(new CommandInvokeContext(sender, arguments, TextUtility.SplitArguments(arguments)));

    public void Run(ICommandSender sender, List<string> arguments) =>
        Run(new CommandInvokeContext(sender, string.Join(' ', arguments), arguments));

    private void Run(CommandInvokeContext ctx)
    {
        if (IsDisabled)
        {
            SendErrorReply(ctx, "commands.commandIsDisabled");
            return;
        }

        ParameterInfo[] parameters = Data.Method.GetParameters();
        object?[] invokeArgs = new object?[parameters.Length];

        invokeArgs[0] = ctx;

        if (TryParseArguments(ctx, parameters, ref invokeArgs))
        {
            Data.Method.Invoke(null, invokeArgs);
        }
    }

    private bool TryParseArguments(CommandInvokeContext ctx, ParameterInfo[] parameters, ref object?[] invokeArgs)
    {
        List<string> arguments = [.. ctx.Arguments];
        ICommandSender sender = ctx.Sender;

        // Replace LINQ Skip(1).Select with manual loop
        for (int i = 1; i < parameters.Length; i++)
        {
            ParameterInfo parameter = parameters[i];
            int argIndex = i - 1;

            if (argIndex >= arguments.Count)
            {
                if (!HandleMissingArgument(sender, parameter, i, ref invokeArgs))
                {
                    return false;
                }
                continue;
            }

            ParseResult parseResult = ParsingNode.TryParse(parameter.ParameterType, sender, arguments[argIndex]);

            if (!HandleParseResult(parseResult, sender, parameter, i, ref invokeArgs))
            {
                return false;
            }
        }

        return true;
    }

    private bool HandleMissingArgument(ICommandSender sender, ParameterInfo parameter, int index, ref object?[] invokeArgs)
    {
        if (!parameter.HasDefaultValue)
        {
            SendErrorReply(sender, "commands.notEnoughArguments");
            ShowSyntaxHelp(sender);
            return false;
        }

        invokeArgs[index] = parameter.DefaultValue;
        return true;
    }

    private bool HandleParseResult(ParseResult result, ICommandSender sender, ParameterInfo parameter, int index, ref object?[] invokeArgs)
    {
        switch (result.Type)
        {
            case ParseResultType.Success:
                invokeArgs[index] = result.Result;
                return true;

            case ParseResultType.EmptyArgument when parameter.HasDefaultValue:
                invokeArgs[index] = parameter.DefaultValue;
                return true;

            case ParseResultType.EmptyArgument:
                SendErrorReply(sender, "commands.emptyArgument", GetSyntaxHelp(parameter.Position - 1)!);
                return false;

            case ParseResultType.TooManyVariants:
                ShowVariants(sender, result);
                return false;

            case ParseResultType.ObjectNotFound:
                SendErrorReply(sender, "commands.objectNotFound");
                return false;

            case ParseResultType.NoParser:
                SendErrorReply(sender, "commands.noParser");
                return false;

            default:
                return false;
        }
    }

    private static void ShowVariants(ICommandSender sender, ParseResult result) =>
        sender.ReplyPage(
            PagesCollection.CreateFromList(result.Variants!, 120, 10),
            Localization.Get("commands.tooManyVariants", sender.Language),
            null, null, false, 0);

    private string? GetSyntaxHelp(int index) =>
        Data.Syntax?[index];

    private void ShowSyntaxHelp(ICommandSender sender)
    {
        if (Data.Syntax != null)
        {
            SendErrorReply(sender, "commands.validSyntaxIs",
                Data.Name, string.Join(' ', Data.Syntax));
        }
    }

    private static void SendErrorReply(ICommandSender sender, string key, params object[] args) =>
        sender.ReplyError(Localization.Get(key, sender.Language), args);

    private static void SendErrorReply(CommandInvokeContext ctx, string key) =>
        SendErrorReply(ctx.Sender, key);
}
