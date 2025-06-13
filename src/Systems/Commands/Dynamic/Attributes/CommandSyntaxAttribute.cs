namespace Amethyst.Systems.Commands.Dynamic.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public sealed class CommandSyntaxAttribute(string culture, params string[] syntax) : Attribute
{
    public string Culture { get; } = culture;
    public string[] Syntax { get; } = syntax;
}
