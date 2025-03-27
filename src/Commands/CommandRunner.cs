using System.Reflection;
using Amethyst.Commands.Data;
using Amethyst.Commands.Parsing;
using Amethyst.Text;

namespace Amethyst.Commands;

public sealed class CommandRunner
{
    internal CommandRunner(CommandData data)
    {
        Data = data;
    }

    public CommandData Data { get; }
    public bool IsDisabled { get; set; }

    public bool NameEquals(string text)
    {
        string[] textLines = text.Split(' ');
        string[] cmdLines = Data.Name.Split(' ');

        if (cmdLines.Length > textLines.Length)
            return false;

        for (int i = 0; i < cmdLines.Length; i++)
            if (cmdLines[i] != textLines[i])
                return false;

        return true;
    }

    public void Run(ICommandSender sender, string arguments)
    {
        var parsedArgs = TextUtility.SplitArguments(arguments);
        CommandInvokeContext ctx = new CommandInvokeContext(sender, arguments, parsedArgs);
        Run(ctx);
    }

    public void Run(ICommandSender sender, List<string> arguments)
    {
        CommandInvokeContext ctx = new CommandInvokeContext(sender, string.Join(" ", arguments), arguments);
        Run(ctx);
    }

    public void Run(CommandInvokeContext ctx)
    {
        if (IsDisabled)
        {
            ctx.Sender.ReplyError(Localization.Get("commands.commandIsDisabled", ctx.Sender.Language));
            return;
        }

        ParameterInfo[] methodParams = Data.Method.GetParameters();
        object?[] invokeArgs = new object?[methodParams.Length];
        invokeArgs[0] = ctx;

        if (!ParseArguments(ctx.Sender, ctx.Arguments.ToList(), methodParams, ref invokeArgs))
            return;

        Data.Method.Invoke(null, invokeArgs);
    }

    private bool ParseArguments(ICommandSender sender, List<string> arguments, ParameterInfo[] methodParams, ref object?[] param)
    {
        int enteredParams = arguments.Count;

        for (int i = 1; i < methodParams.Length; i++)
        {
            int fixedParamIndex = i;

            ParameterInfo p = methodParams[i];

            if (i >= enteredParams)
            {
                if (p.HasDefaultValue)
                {
                    param[fixedParamIndex] = p.DefaultValue;
                    continue;
                }

                sender.ReplyError(Localization.Get("commands.notEnoughArguments", sender.Language));

                if (Data.Syntax != null)
                    sender.ReplyError(Localization.Get("commands.validSyntaxIs", sender.Language), Data.Name, string.Join(" ", Data.Syntax));
                return false;
            }

            string? paramSyntax = (Data.Syntax != null && Data.Syntax.Length <= i) ? Data.Syntax[i - 1] : null;

            ParseResult result = ParsingNode.TryParse(p.ParameterType, sender, arguments[fixedParamIndex]);
            switch (result.Type)
            {
                case ParseResultType.Success:
                    param[fixedParamIndex] = result.Result;
                    break;

                case ParseResultType.EmptyArgument:
                    if (p.HasDefaultValue)
                        param[fixedParamIndex] = p.DefaultValue;
                    else
                    {
                        sender.ReplyError(Localization.Get("commands.emptyArgument", sender.Language), paramSyntax!);
                        return false;
                    }
                    break;

                case ParseResultType.TooManyVariants:
                    sender.ReplyPage(PagesCollection.CreateFromList(result.Variants!, 120, 10), Localization.Get("commands.tooManyVariants", sender.Language), null, null, false, 0);
                    return false;

                case ParseResultType.ObjectNotFound:
                    sender.ReplyError(Localization.Get("commands.objectNotFound", sender.Language));
                    return false;

                case ParseResultType.NoParser:
                    sender.ReplyError(Localization.Get("commands.noParser", sender.Language));
                    return false;
            }
        }

        return true;
    }
}
