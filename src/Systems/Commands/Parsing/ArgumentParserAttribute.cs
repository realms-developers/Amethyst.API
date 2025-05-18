namespace Amethyst.Systems.Commands.Parsing;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class ArgumentParserAttribute(Type parserType, string methodName) : Attribute
{
    public Type ParserType { get; } = parserType;
    public string MethodName { get; } = methodName;
}
