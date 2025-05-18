namespace Amethyst.Gameplay.Players.Extensions;

public static class PlayerExtensions
{
    private static readonly Dictionary<Type, dynamic> _builders = [];
    private static readonly Dictionary<Type, Action<NetPlayer>> _loaders = [];

    internal static void LoadExtensions(NetPlayer player)
    {
        // collection was modified issue!
        Dictionary<Type, Action<NetPlayer>> loaders = _loaders;

        foreach (KeyValuePair<Type, Action<NetPlayer>> loader in loaders)
        {
            loader.Value(player);
        }
    }

    public static void RegisterBuilder<T>(IPlayerExtensionBuilder<T> builder) where T : IPlayerExtension
    {
        Type type = typeof(T);
        if (_builders.ContainsKey(type))
        {
            return;
        }

        _builders.Add(type, builder);
        _loaders.Add(type, (plr) => plr.LoadExtension<T>());
    }

    public static void UnregisterBuilder<T>() where T : IPlayerExtension
    {
        Type type = typeof(T);

        foreach (NetPlayer plr in PlayerManager.Tracker)
        {
            plr.UnloadExtension<T>();
        }

        _builders.Remove(type);
        _loaders.Remove(type);
    }

    public static IPlayerExtensionBuilder<T>? GetBuilder<T>() where T : IPlayerExtension
    {
        Type type = typeof(T);

        return _builders.TryGetValue(type, out dynamic? value) ? (IPlayerExtensionBuilder<T>)value : null;
    }
}
