namespace Amethyst.Players.Extensions;

public static class PlayerExtensions
{
    private static Dictionary<Type, dynamic> Builders = new Dictionary<Type, dynamic>();
    private static Dictionary<Type, Action<NetPlayer>> Loaders = new Dictionary<Type, Action<NetPlayer>>();

    internal static void LoadExtensions(NetPlayer player)
    {
        // collection was modified issue!
        var loaders = Loaders;
        foreach (var loader in loaders)
            loader.Value(player);
    }

    public static void RegisterBuilder<T>(IPlayerExtensionBuilder<T> builder) where T : IPlayerExtension
    {
        var type = typeof(T);
        if (Builders.ContainsKey(type)) return;

        Builders.Add(type, builder);
        Loaders.Add(type, (plr) => plr.LoadExtension<T>());
    }

    public static void UnregisterBuilder<T>() where T : IPlayerExtension
    {
        var type = typeof(T);

        foreach (var plr in PlayerManager.Tracker)
            plr.UnloadExtension<T>();

        Builders.Remove(type);
        Loaders.Remove(type);
    }

    public static IPlayerExtensionBuilder<T>? GetBuilder<T>() where T : IPlayerExtension
    {
        var type = typeof(T);
        if (Builders.TryGetValue(type, out var value)) return value;

        return null;
    }
}