using Newtonsoft.Json;

namespace Amethyst;

public static class Localization
{
    private const string _directory = "localization";

    // Nested dictionary: culture -> (key -> localized string)
    private static readonly Dictionary<string, Dictionary<string, string>> _localizationData = new();

    /// <summary>
    /// Loads all localization files for a specific culture.
    /// </summary>
    public static void Load(string culture)
    {
        string directoryPath = Path.Combine(_directory, culture);

        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException($"Localization directory for '{culture}' not found.");
        }

        // Create a new inner dictionary for this culture if it doesn't exist
        if (!_localizationData.ContainsKey(culture))
        {
            _localizationData[culture] = new Dictionary<string, string>();
        }

        string[] files = Directory.GetFiles(directoryPath, "*.json");

        foreach (string file in files)
        {
            string jsonContent = File.ReadAllText(file);
            Dictionary<string, string>? fileData = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);

            if (fileData != null)
            {
                foreach (KeyValuePair<string, string> kvp in fileData)
                {
                    // Overwrite or add the value for the key in the specific culture dictionary.
                    _localizationData[culture][kvp.Key] = kvp.Value;
                }
            }
        }
    }

    /// <summary>
    /// Loads localization files for every available culture in the Localization directory.
    /// </summary>
    public static void Load()
    {
        if (!Directory.Exists(_directory))
        {
            throw new DirectoryNotFoundException("Localization directory not found.");
        }

        // Iterate over each subdirectory (each represents a culture)
        string[] cultureDirectories = Directory.GetDirectories(_directory);

        foreach (string cultureDir in cultureDirectories)
        {
            string culture = new DirectoryInfo(cultureDir).Name;

            Load(culture);
        }
    }

    /// <summary>
    /// Gets whether or not a culture is loaded.
    /// </summary>
    /// <param name="culture">The culture to check.</param>
    /// <returns>Loaded or not.</returns>
    public static bool Loaded(string culture) => _localizationData.ContainsKey(culture);

    /// <summary>
    /// Unloads localization data for a specific culture.
    /// </summary>
    public static bool Unload(string culture) => _localizationData.Remove(culture);

    /// <summary>
    /// Unloads all localization data.
    /// </summary>
    public static void Unload() => _localizationData.Clear();

    /// <summary>
    /// Gets the localized string for the specified key.
    /// </summary>
    public static string Get(string key, string culture) =>
        _localizationData.TryGetValue(culture, out Dictionary<string, string>? cultureDict) &&
        cultureDict.TryGetValue(key, out string? value) ? value : key;
}
