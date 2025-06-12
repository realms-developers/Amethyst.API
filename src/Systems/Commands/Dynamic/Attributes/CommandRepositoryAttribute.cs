namespace Amethyst.Systems.Commands.Dynamic.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class CommandRepositoryAttribute(string repository) : Attribute
{
    public string Repository { get; } = repository;
}
