using Amethyst.Kernel;
using Newtonsoft.Json;

namespace Amethyst.Storages.Config;

internal static class ConfigDiskStorage
{
    private static string GetPath(string name) => Path.Combine(AmethystSession.Profile.SavePath, name);

    internal static void ReadOrCreate<T>(string name, ref T def)
    {
        if (!File.Exists(GetPath($"{name}.json")))
        {
            Write(name, def!);
            return;
        }

        def = Read<T>(name);
    }

    internal static T Read<T>(string name)
    {
        string json = File.ReadAllText(GetPath($"{name}.json"));
        return JsonConvert.DeserializeObject<T>(json)!;
    }

    internal static bool TryRead<T>(string name, out T? result)
    {
        string path = GetPath($"{name}.json");
        if (!File.Exists(path))
        {
            result = default;
            return false;
        }

        string json = File.ReadAllText(path);
        result = JsonConvert.DeserializeObject<T>(json);
        return true;
    }

    internal static void Write(string name, object value)
    {
        string json = JsonConvert.SerializeObject(value, Formatting.Indented);
        File.WriteAllText(GetPath($"{name}.json"), json);
    }
}
