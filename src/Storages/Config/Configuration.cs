namespace Amethyst.Storages.Config;

public sealed class Configuration<T> where T : struct
{
    public Configuration(string name, T defaultValue)
    {
        Name = name; 
        _data = defaultValue;
    }

    public string Name { get; }
    public T Data => _data;

    private T _data;

    public void Load()
    {
        var data = Data;
        ConfigDiskStorage.ReadOrCreate(Name, ref data);
        _data = data;
    }

    public void Save()
    {
        ConfigDiskStorage.Write(Name, _data);
    }

    public void Modify(ModifyFunc modifyFunc, bool save = true)
    {
        modifyFunc(ref _data);
        if (save)
            Save();
    }

    public delegate void ModifyFunc(ref T config);
}