namespace Amethyst.Systems.Commands.Base;

public sealed class CommandRepository
{
    public CommandRepository(string name)
    {
        Name = name;
    }

    public string Name { get; }
    public IReadOnlyList<ICommand> RegisteredCommands => _commands;

    private readonly List<ICommand> _commands = new();

    public void Add(ICommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.Metadata.Names.Length == 0)
            throw new ArgumentException($"Command has no names.");

        if (command.Metadata.Names.Any(p => p.Length == 0))
            throw new ArgumentException($"Command {command.Metadata.Names.FirstOrDefault(p => p.Length != 0) ?? "UNKNOWN"} has empty names.");

        if (command.Repository != this)
            throw new ArgumentException($"Command {command.Metadata.Names.First()} is registered in {command.Repository.Name} repository.");

        if (_commands.Contains(command))
            return;

        _commands.Add(command);
    }

    public void Remove(ICommand command)
    {
        _commands.Remove(command);
    }

    public void Clear()
    {
        _commands.Clear();
    }

    public ICommand? GetCommand(string name)
    {
        return string.IsNullOrEmpty(name)
            ? throw new ArgumentNullException(nameof(name))
            : _commands.FirstOrDefault(c => c.Metadata.Names.Contains(name));
    }

    public ICommand? FindCommand(string fullText)
    {
        if (string.IsNullOrEmpty(fullText))
            throw new ArgumentNullException(nameof(fullText));

        string[] textLines = fullText.Split();

        foreach (var cmd in _commands)
        {
            foreach (string name in cmd.Metadata.Names)
            {
                string[] cmdLines = name.Split(' ');

                if (cmdLines.Length > textLines.Length)
                {
                    continue;
                }

                for (int i = 0; i < cmdLines.Length; i++)
                {
                    if (cmdLines[i] != textLines[i])
                    {
                        continue;
                    }
                }
            }
        }

        return null;
    }
}
