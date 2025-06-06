using Amethyst.Systems.Commands.Base;
using Amethyst.Systems.Commands.Dynamic.Parsing;

namespace Amethyst.Systems.Commands;

public static class CommandsOrganizer
{
    static CommandsOrganizer()
    {
        ParsingNode.Initialize();
    }

    public static IReadOnlyList<CommandRepository> Repositories => _repositories.Values.ToList().AsReadOnly();

    public static CommandRepository Shared => _repositories["shared"];
    public static CommandRepository Root => _repositories["root"];
    public static CommandRepository Debug => _repositories["debug"];

    private static readonly Dictionary<string, CommandRepository> _repositories = new()
    {
        { "shared", new CommandRepository("shared") },
        { "root", new CommandRepository("root") },
        { "debug", new CommandRepository("debug") }
    };

    private static readonly string[] _defRepos = { "shared", "root", "debug" };

    public static void AddRepository(CommandRepository repository)
    {
        if (_repositories.ContainsKey(repository.Name))
            throw new ArgumentException($"Repository with name {repository.Name} already exists.");

        _repositories[repository.Name] = repository;
    }

    public static void RemoveRepository(CommandRepository repository)
    {
        if (_defRepos.Contains(repository.Name))
            throw new ArgumentException($"Repository with name {repository.Name} is a default repository and cannot be removed.");

        if (!_repositories.Remove(repository.Name))
            throw new ArgumentException($"Repository with name {repository.Name} does not exist.");
    }

    public static CommandRepository? GetRepository(string name)
    {
        if (_repositories.TryGetValue(name, out var repository))
            return repository;

        return null;
    }
}
