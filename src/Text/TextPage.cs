namespace Amethyst.Text;

public sealed class TextPage
{
    public TextPage(string name, List<string> lines, string? showPermission, bool isDisabled)
    {
        _lines = lines;

        Name = name;
        ShowPermission = showPermission;
        IsDisabled = isDisabled;
    }

    internal List<string> _lines;

    public string Name { get; }

    public string? ShowPermission { get; set; }

    public bool IsDisabled { get; set; }

    public IReadOnlyList<string> Lines => _lines.AsReadOnly();

    public void Add(string line) => _lines.Add(line);

    public void Replace(int index, string line)
    {
        _lines.RemoveAt(index);
        _lines.Add(line);
    }

    public void Remove(string line) => _lines.Remove(line);
}