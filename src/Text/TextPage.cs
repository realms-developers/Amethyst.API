namespace Amethyst.Text;

public sealed class TextPage(string name, List<string> lines, string? showPermission, bool isDisabled)
{
    internal List<string> _lines = lines;

    public string Name { get; } = name;

    public string? ShowPermission { get; set; } = showPermission;

    public bool IsDisabled { get; set; } = isDisabled;

    public IReadOnlyList<string> Lines => _lines.AsReadOnly();

    public void Add(string line) => _lines.Add(line);

    public void Replace(int index, string line)
    {
        _lines.RemoveAt(index);
        _lines.Add(line);
    }

    public void Remove(string line) => _lines.Remove(line);
}
