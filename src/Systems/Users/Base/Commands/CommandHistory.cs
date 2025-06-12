using System.Collections;
using Amethyst.Systems.Commands;

namespace Amethyst.Systems.Users.Base.Commands;

public sealed class CommandHistory : IReadOnlyList<CompletedCommandInfo>
{
    private readonly List<CompletedCommandInfo> _commands = new();

    public CompletedCommandInfo this[int index] => _commands[index];

    public int Count => _commands.Count;

    public void Add(CompletedCommandInfo command)
    {
        ArgumentNullException.ThrowIfNull(command);
        _commands.Add(command);
    }

    public CompletedCommandInfo? GetLast()
    {
        return _commands.Count > 0 ? _commands[^1] : null;
    }

    public IEnumerator<CompletedCommandInfo> GetEnumerator() => _commands.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
