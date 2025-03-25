namespace Amethyst.Storages.Config;

public sealed class Configuration<T> where T : struct
{
    public Configuration(string name, T defaultValue)
    {
        Name = name; 
        Data = defaultValue;
    }

    public string Name { get; }
    public T Data { get; internal set; }

    public void Load()
    {
        var data = Data;
        ConfigDiskStorage.ReadOrCreate(Name, ref data);
        Data = data;
    }

    public void Save()
    {
        ConfigDiskStorage.Write(Name, Data);
    }

    public void Modify(Func<T, T> modifyFunc, bool save = true)
    {
        Data = modifyFunc(Data);
        if (save)
            Save();
    }
}