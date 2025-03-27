using System.Globalization;
using System.Text.RegularExpressions;
using Amethyst.Network;
using Newtonsoft.Json;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

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

    public static class Items
    {
        internal static string[] RussianNames = new string[ItemID.Count];
        internal static string[] EnglishNames = new string[ItemID.Count];
        internal static bool IsInitialized = false;

        internal static void Initialize()
        {
            if (IsInitialized) return;
            IsInitialized = true;

            LanguageManager.Instance.SetLanguage(GameCulture.FromLegacyId((byte)GameCulture.CultureName.English));

            RussianNames = new string[ItemID.Count];
            EnglishNames = new string[ItemID.Count];

            for (int i = 0; i < ItemID.Count; i++)
                EnglishNames[i] = Lang.GetItemNameValue(i);

            LanguageManager.Instance.SetLanguage(GameCulture.FromLegacyId((byte)GameCulture.CultureName.Russian));

            for (int i = 0; i < ItemID.Count; i++)
                RussianNames[i] = Lang.GetItemNameValue(i);

            LanguageManager.Instance.SetLanguage(GameCulture.FromLegacyId((byte)GameCulture.CultureName.English));
        }

        public static List<ItemFindData> FindItem(bool isRussian, string input)
        {
            string[] names = isRussian ? RussianNames : EnglishNames;

            if (int.TryParse(input, out int itemIndex))
            {
                return new List<ItemFindData>(1)
                {
                    new ItemFindData(itemIndex, names[itemIndex])
                };
            }

            if (input.StartsWith('['))
            {
                NetItem? item = GetItemFromTag(input);
                if (item != null)
                {
                    return new List<ItemFindData>(1)
                    {
                        new ItemFindData(item.Value.ID, names[item.Value.ID])
                    };
                }
            }

            List<ItemFindData> startsResult = new List<ItemFindData>();
            List<ItemFindData> containsResult = new List<ItemFindData>();

            for (int i = 0; i < names.Length; i++)
            {
                if (input.Equals(names[i], StringComparison.OrdinalIgnoreCase))
                    return new List<ItemFindData>(1)
                    {
                        new ItemFindData(i, names[i])
                    };

                if (names[i].StartsWith(input, StringComparison.OrdinalIgnoreCase))
                    startsResult.Add(new ItemFindData(i, names[i]));
            }

            if (startsResult.Count > 0)
                return startsResult;

            return containsResult;
        }

        public static NetItem? GetItemFromTag(string tag)
        {
            Regex regex = new Regex(@"\[i(tem)?(?:\/s(?<Stack>\d{1,4}))?(?:\/p(?<Prefix>\d{1,3}))?:(?<NetID>-?\d{1,4})\]");
            Match match = regex.Match(tag);
            if (!match.Success)
                return null;

            return new NetItem(
                id: int.Parse(match.Groups["NetID"].Value, CultureInfo.InvariantCulture),

                stack: string.IsNullOrWhiteSpace(match.Groups["Stack"].Value) ? (short)1 :
                    short.Parse(match.Groups["Stack"].Value, CultureInfo.InvariantCulture),

                prefix: string.IsNullOrWhiteSpace(match.Groups["Prefix"].Value) ? byte.MinValue :
                    byte.Parse(match.Groups["Prefix"].Value, CultureInfo.InvariantCulture));
        }
    }

    public sealed record ItemFindData(int ItemID, string Name);
}
