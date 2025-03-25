using Amethyst.Storages.Config;

namespace Amethyst.Core.Profiles;

public sealed class ProfileConfigContainer
{
    private Dictionary<string, object> _configs = new Dictionary<string, object>();

    public Configuration<T> Get<T>() where T : struct
    {
        var name = typeof(T).FullName!;

        if (_configs.ContainsKey(name) == false)
        {
            var cfg = new Configuration<T>(name, default);
            cfg.Load();

            _configs.Add(name, (object)cfg);

            return cfg;
        }

        else return (Configuration<T>)_configs[name];
    }
}