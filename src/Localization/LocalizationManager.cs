namespace Amethyst.Localization;

public static class LocalizationManager
{
    internal static Language[] Languages = new Language[256];

    internal static void Initialize()
    {
        CreateLanguage("ru-RU");
        CreateLanguage("en-US");

        foreach (var file in Directory.EnumerateFiles("localization", "*.json"))
            LoadLocalizationFile(file);
    }

    internal const int MinFileLength = 7;
    internal const int CultureIndex = 5;

    internal static void LoadLocalizationFile(string file)
    {
        string path = Path.GetFileName(file);
        if (path.Length < MinFileLength) return;

        var culture = path.Substring(0, 5);
        var name = path.Substring(6, path.Length - 11);

        FindLanguage(culture)?.AddPackage(LocalizationPackage.LoadFromFile(name, file)!);
    }

    public static bool CreateLanguage(string name)
    {
        if (Languages.Any(p => p != null && p.Name == name)) return false;

        for (int i = 0; i < Languages.Length; i++)
        {
            if (Languages[i] == null)
            {
                Languages[i] = new Language(name);
                return true;
            }
        }

        return false;
    }

    public static Language? FindLanguage(string name)
    {
        return Languages.FirstOrDefault(p => p.Name == name);
    }
}