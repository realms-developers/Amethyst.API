using Amethyst.Text;

namespace Amethyst.Localization;

public sealed class Language
{
    internal Language(string name)
    {
        Name = name;
        HelpPages = new PagesCollection();

        _packages = new List<LocalizationPackage>(32);
    }

    private List<LocalizationPackage> _packages;

    public string Name { get; }
    public PagesCollection HelpPages { get; }

    public void AddPackage(LocalizationPackage package)
    {
        RemovePackage(package.Name);
        _packages.Add(package);
    }

    public void RemovePackage(LocalizationPackage package)
    {
        _packages.Remove(package);
    }

    public void RemovePackage(string packageName)
    {
        _packages.RemoveAll(p => p.Name == packageName);
    }

    private const string LocalizeTrigger = "$LOCALIZE ";

    public string LocalizeDirect(string value)
    {
        if (value.StartsWith(LocalizeTrigger, StringComparison.InvariantCulture) == false)
            return value;

        value = value.Substring(LocalizeTrigger.Length);

        foreach (var pkg in _packages)
        {
            var text = pkg.LocalizeKey(value);
            if (text != null)
                return text;
        }

        return value;
    }
}
