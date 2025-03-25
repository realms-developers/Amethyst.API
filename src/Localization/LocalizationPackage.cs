using System.Reflection;
using System.Text;
using Amethyst.Core;
using Newtonsoft.Json;

namespace Amethyst.Localization;

public sealed class LocalizationPackage
{
    public static LocalizationPackage? LoadFromFile(string name, string path)
    {
        if (File.Exists(path) == false)
        {
            AmethystLog.System.Error("Localization", $"Failed to load LocalizationPackage '{name}': file on path '{path}' does not exists!");
            return null;
        }

        string text = File.ReadAllText(path);
        return LoadFromJson(name, text);
    }

    public static LocalizationPackage? LoadFromJson(string name, string text)
    {
        try
        {
            var keys = JsonConvert.DeserializeObject<Dictionary<string,string>>(text);
            if (keys == null) return null;

            var package = new LocalizationPackage(name, keys);
            return package;
        }
        catch (Exception ex)
        {
            AmethystLog.System.Error("Localization", $"Failed to load LocalizationPackage '{name}':");
            AmethystLog.System.Error("Localization", ex.ToString());
        }

        return null;
    }

    internal LocalizationPackage(string name, Dictionary<string, string> localization)
    {
        Name = name;
        _localizationKeys = localization;
    }

    private Dictionary<string, string> _localizationKeys;

    public string Name { get; }
    public IReadOnlyDictionary<string, string> Keys => _localizationKeys.AsReadOnly();

    public string? LocalizeKey(string name)
    {
        if (_localizationKeys.TryGetValue(name, out var value))
            return value;

        return null;
    }
}