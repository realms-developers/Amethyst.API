namespace Amethyst.Systems.Commands;

public sealed class CommandInvokeContext
{
    internal CommandInvokeContext(ICommandSender sender, string text, List<string> arguments, string name)
    {
        Sender = sender;
        ArgumentsText = text;
        Arguments = arguments;
        Name = name;
    }

    public ICommandSender Sender { get; }
    public string ArgumentsText { get; }
    public string Name { get; }
    public List<string> Arguments { get; }
}
