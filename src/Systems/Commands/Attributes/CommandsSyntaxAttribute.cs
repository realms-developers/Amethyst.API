namespace Amethyst.Systems.Commands.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class CommandsSyntaxAttribute(params string[] syntax) : Attribute
{
    public string[] Syntax { get; } = syntax;
}
