namespace Amethyst.Systems.Commands.Base.Metadata;

public sealed class CommandSyntax
{
    public CommandSyntax(string defaultCulture)
    {
        _defaultCulture = defaultCulture;
    }

    private string _defaultCulture;
    private Dictionary<string, string[]> _syntax = new();

    public string[]? this[string culture]
    {
        get
        {
            return _syntax.Count == 0
                ? throw new KeyNotFoundException("No syntax available.")
                : _syntax.TryGetValue(culture, out var syntax) ? syntax :
                    _syntax.TryGetValue(_defaultCulture, out var defaultSyntax) ? defaultSyntax :
                    throw new KeyNotFoundException($"No syntax available for culture {culture}.");
        }
    }

    public CommandSyntax Add(string culture, string[] syntax)
    {
        if (_syntax.ContainsKey(culture))
            throw new ArgumentException($"Syntax for culture {culture} already exists.");

        _syntax[culture] = syntax;
        return this;
    }
}
