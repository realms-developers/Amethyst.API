namespace Amethyst.Storages.Config;

public sealed class Configuration<T>(string name, T defaultValue) where T : class
{
    public string Name { get; } = name;
    public T Data => _data;

    private T _data = defaultValue;

    public void Load()
    {
        T data = Data;
        ConfigDiskStorage.ReadOrCreate(Name, ref data);
        _data = data;
    }

    public void Save() => ConfigDiskStorage.Write(Name, _data);

    public void Modify(ModifyFunc modifyFunc, bool save = true)
    {
        modifyFunc(ref _data);
        if (save)
        {
            Save();
        }
    }

    public delegate void ModifyFunc(ref T config);
}
