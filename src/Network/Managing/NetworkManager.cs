using Amethyst.Network.Implementation;

namespace Amethyst.Network.Managing;

public static class NetworkManager
{
    internal static NetworkInstance Instance = new();
    internal static INetworkProvider Provider = new BasicNetworkProvider();

    public static PacketBinder Binding { get; } = new PacketBinder(Instance);

    internal static void Initialize() => Provider.Initialize(Instance);

    internal static void RequestListening() => Provider.StartListening();

    public static void SetupProvider(INetworkProvider provider) => Provider = provider;
}
