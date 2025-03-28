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
            ctx.Sender.ReplyError(Localization.Get("commands.commandIsDisabled", ctx.Sender.Language));
            return;
        }

        object?[] invokeArgs = new object?[_methodParameters.Length];
        invokeArgs[0] = ctx;

        if (!ParseArguments(ctx.Sender, [.. ctx.Arguments], invokeArgs))
        {
            return;
        }

        Data.Method.Invoke(null, invokeArgs);
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

                sender.ReplyError(Localization.Get("commands.notEnoughArguments", sender.Language));
                ShowSyntaxHint(sender);
                return false;
            }

            string input = arguments[userArgIndex];
            string? paramSyntax = GetParameterSyntax(userArgIndex);

            ParseResult result = ParsingNode.TryParse(p.ParameterType, sender, input);
            if (!HandleParseResult(result, sender, paramSyntax, ref invokeArgs[i]))
                return false;
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
            sender.ReplyError(Localization.Get("commands.validSyntaxIs", sender.Language),
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
                sender.ReplyError(Localization.Get("commands.emptyArgument", sender.Language), paramSyntax!);
                return false;

            case ParseResultType.TooManyVariants:
                sender.ReplyPage(PagesCollection.CreateFromList(result.Variants!, 120, 10),
                    Localization.Get("commands.tooManyVariants", sender.Language), null, null, false, 0);
                return false;

            case ParseResultType.ObjectNotFound:
                sender.ReplyError(Localization.Get("commands.objectNotFound", sender.Language));
                return false;

            case ParseResultType.NoParser:
                sender.ReplyError(Localization.Get("commands.noParser", sender.Language));
                return false;

            default:
                sender.ReplyError(Localization.Get("commands.invalidSyntax", sender.Language));
                return false;
        }

        return true;
    }
}
