using Amethyst.Storages.Config;

namespace Amethyst.Core.Profiles;

public sealed class ProfileConfigContainer
{
    private readonly Dictionary<string, object> _configs = [];

    public Configuration<T> Get<T>() where T : struct
    {
        string name = typeof(T).FullName!;

        if (!_configs.TryGetValue(name, out object? value))
        {
            var cfg = new Configuration<T>(name, default);
            cfg.Load();

            _configs.Add(name, cfg);

            return cfg;
        }
        else
        {
            return (Configuration<T>)value;
        }
    }
}
