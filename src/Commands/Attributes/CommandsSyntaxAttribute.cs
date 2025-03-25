namespace Amethyst.Commands;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class CommandsSyntaxAttribute : Attribute
{
    public CommandsSyntaxAttribute(params string[] syntax)
    {   
        Syntax = syntax;
    }

    public readonly string[] Syntax;
}
