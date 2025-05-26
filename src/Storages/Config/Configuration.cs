namespace Amethyst.Storages.Config;

public sealed class Configuration<T>(string name, T defaultValue) where T : class
{
    public string Name { get; } = name;
    public ref T Data => ref _data;

    private T _data = defaultValue;

    public void Load()
    {
        if (ConfigDiskStorage.TryRead(Name, out T? loadedData) && loadedData != null)
        {
            _data = loadedData; // Completely replace defaults
        }
        else
        {
            Save(); // Write defaults if file doesn't exist
        }
    }

    public void Save() => ConfigDiskStorage.Write(Name, _data);

    public delegate void ModifyFunc(ref T config);
}
