namespace Amethyst.Commands;

public sealed class CommandInvokeContext
{
    internal CommandInvokeContext(ICommandSender sender, string text, List<string> arguments)
    {
        Sender = sender;
        ArgumentsText = text;
        Arguments = arguments;
    }

    public ICommandSender Sender { get; }
    public string ArgumentsText { get; }
    public List<string> Arguments { get; }
}