namespace Amethyst.Systems.Commands.Dynamic.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public sealed class CommandSyntaxAttribute : Attribute
{
    public CommandSyntaxAttribute(string culture, params string[] syntax)
    {
        Culture = culture;
        Syntax = syntax;
    }

    public string Culture { get; }
    public string[] Syntax { get; }
}
