using System.Reflection;
using Amethyst.Commands.Parsing;
using Amethyst.Text;

namespace Amethyst.Commands;

public sealed class CommandRunner
{
    private readonly ParameterInfo[] _methodParameters;

    internal CommandRunner(CommandData data)
    {
        Data = data;
        _methodParameters = data.Method.GetParameters();
    }

    public CommandData Data { get; }
    public bool IsDisabled { get; set; }

    public bool NameEquals(string text)
    {
        string[] textLines = text.Split();
        string[] cmdLines = Data.Name.Split();

        if (cmdLines.Length > textLines.Length)
        {
            return false;
        }

        for (int i = 0; i < cmdLines.Length; i++)
        {
            if (cmdLines[i] != textLines[i])
            {
                return false;
            }
        }

        return true;
    }

    public void Run(ICommandSender sender, string arguments)
    {
        List<string> parsedArgs = TextUtility.SplitArguments(arguments);
        Run(new(sender, arguments, parsedArgs));
    }

    public void Run(ICommandSender sender, List<string> arguments)
    {
        Run(new(sender, string.Join(' ', arguments), arguments));
    }

    public void Run(CommandInvokeContext ctx)
    {
        if (IsDisabled)
        {
            ctx.Sender.ReplyError("commands.commandIsDisabled");
            return;
        }

        object?[] invokeArgs = new object?[_methodParameters.Length];
        invokeArgs[0] = ctx;

        if (!ParseArguments(ctx.Sender, [.. ctx.Arguments], invokeArgs))
        {
            return;
        }

        try
        {
            Data.Method.Invoke(null, invokeArgs);
        }
        catch (TargetInvocationException tie)
        {
            if (tie.InnerException != null)
            {
                // Get the original exception
                Exception originalException = tie.InnerException;

                // Rethrow the original exception
                throw originalException;
            }

            throw;
        }
    }

    private bool ParseArguments(ICommandSender sender, string[] arguments, object?[] invokeArgs)
    {
        int enteredParams = arguments.Length;

        for (int i = 1; i < _methodParameters.Length; i++)
        {
            ParameterInfo p = _methodParameters[i];
            int userArgIndex = i - 1;

            if (userArgIndex >= enteredParams)
            {
                if (p.HasDefaultValue)
                {
                    invokeArgs[i] = p.DefaultValue;
                    continue;
                }

                //sender.ReplyError("commands.notEnoughArguments");
                ShowSyntaxHint(sender);
                return false;
            }

            string input = arguments[userArgIndex];
            string? paramSyntax = GetParameterSyntax(userArgIndex);

            ArgumentParserAttribute? parserAttr = p.GetCustomAttribute<ArgumentParserAttribute>();
            ParseResult result;

            if (parserAttr != null)
            {
                // Retrieve the custom parser method
                MethodInfo? parseMethod = parserAttr.ParserType.GetMethod(
                    parserAttr.MethodName,
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    [typeof(ICommandSender), typeof(string)],
                    null);

                if (parseMethod == null)
                {
                    sender.ReplyError("commands.customParserNotFound");
                    return false;
                }

                ArgumentParser customParser = (ArgumentParser)Delegate.CreateDelegate(typeof(ArgumentParser), parseMethod);

                result = customParser(sender, input);
            }
            else
            {
                // Use default type-based parser
                result = ParsingNode.TryParse(p.ParameterType, sender, input);
            }

            if (!HandleParseResult(result, sender, paramSyntax, ref invokeArgs[i]))
            {
                return false;
            }
        }

        return true;
    }

    private string? GetParameterSyntax(int userArgIndex)
    {
        return Data.Syntax != null && Data.Syntax.Length > userArgIndex
            ? Data.Syntax[userArgIndex]
            : null;
    }

    private void ShowSyntaxHint(ICommandSender sender)
    {
        if (Data.Syntax != null)
        {
            sender.ReplyError("commands.validSyntaxIs",
                Data.Name, string.Join(' ', Data.Syntax));
        }
    }

    private static bool HandleParseResult(ParseResult result, ICommandSender sender,
        string? paramSyntax, ref object? target)
    {
        switch (result.Type)
        {
            case ParseResultType.Success:
                target = result.Result;
                break;

            case ParseResultType.EmptyArgument:
                sender.ReplyError("commands.emptyArgument", paramSyntax!);
                return false;

            case ParseResultType.TooManyVariants:
                sender.ReplyPage(PagesCollection.CreateFromList(result.Variants!, 120, 10),
                    "commands.tooManyVariants", null, null, false, 0);
                return false;

            case ParseResultType.ObjectNotFound:
                sender.ReplyError("commands.objectNotFound");
                return false;

            case ParseResultType.NoParser:
                sender.ReplyError("commands.noParser");
                return false;

            default:
                sender.ReplyError("commands.invalidSyntax");
                return false;
        }

        return true;
    }
}
