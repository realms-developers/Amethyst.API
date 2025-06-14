namespace Amethyst.Systems.Commands.Base.Metadata;

public sealed class CommandSyntax(string defaultCulture)
{
    private readonly string _defaultCulture = defaultCulture;
    private readonly Dictionary<string, string[]> _syntax = [];

    public string[]? this[string culture]
    {
        get
        {
            return _syntax.Count == 0
                ? null //throw new KeyNotFoundException("No syntax available.")
                : _syntax.TryGetValue(culture, out string[]? syntax) ? syntax :
                    _syntax.TryGetValue(_defaultCulture, out string[]? defaultSyntax) ? defaultSyntax :
                    null; //throw new KeyNotFoundException($"No syntax available for culture {culture}.");
        }
    }

    public CommandSyntax Add(string culture, string[] syntax)
    {
        if (_syntax.ContainsKey(culture))
        {
            throw new ArgumentException($"Syntax for culture {culture} already exists.");
        }

        _syntax[culture] = syntax;
        return this;
    }
}
