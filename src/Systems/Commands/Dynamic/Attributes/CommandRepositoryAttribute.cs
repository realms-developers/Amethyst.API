namespace Amethyst.Systems.Commands.Dynamic.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class CommandRepositoryAttribute : Attribute
{
    public CommandRepositoryAttribute(string repository)
    {
        Repository = repository;
    }

    public string Repository { get; }
}
