using System.Net;
using Amethyst.Kernel;
using Amethyst.Server.Entities;
using Amethyst.Server.Network.Core;
using Amethyst.Server.Network.Core.Delegates;
using Amethyst.Server.Network.Core.Packets;

namespace Amethyst.Server.Network;

public static class NetworkManager
{
    internal static Dictionary<Type, object> Providers = new Dictionary<Type, object>();

    internal static PacketInvokeHandler?[] InvokeHandlers = new PacketInvokeHandler?[256];
    internal static List<PacketInvokeHandler>?[] DirectHandlers = new List<PacketInvokeHandler>?[256];
    internal static List<PacketInvokeHandler> OverlapHandlers = new List<PacketInvokeHandler>();

    internal static AmethystTcpServer? TcpServer;

    public static void Initialize()
    {
        RegisterPacketHandlers();

        TcpServer = new AmethystTcpServer(IPAddress.Any, AmethystSession.Profile.Port);
        Task.Run(TcpServer.Start);
    }

    internal static void HandlePacket(NetworkClient client, ReadOnlySpan<byte> data)
    {
        if (data.Length < 3 || data.Length > 1000)
        {
            AmethystLog.Network.Error(nameof(NetworkManager), "Received invalid packet data");
            return;
        }

        try
        {
            var ignore = false;

            if (OverlapHandlers.Count > 0)
            {
                foreach (var handler in OverlapHandlers)
                {
                    handler(EntityTrackers.Players[client._index], data, ref ignore);

                    if (ignore)
                        return;
                }
            }

            var directHandlers = DirectHandlers[data[2]];
            if (directHandlers != null)
            {
                foreach (var handler in directHandlers)
                {
                    handler(EntityTrackers.Players[client._index], data, ref ignore);

                    if (ignore)
                        return;
                }
            }

            var packetId = data[2];
            if (InvokeHandlers[packetId] == null)
            {
                AmethystLog.Network.Error(nameof(NetworkManager), $"No handler registered for packet ID {packetId}");
                return;
            }

            var player = EntityTrackers.Players[client._index];
            InvokeHandlers[packetId]!(player, data, ref ignore);
        }
        catch (Exception ex)
        {
            AmethystLog.Network.Error(nameof(NetworkManager), $"Error handling packet ID {data[2]}: {ex.Message}");
            return;
        }
    }

    private static void RegisterPacketHandlers()
    {
        foreach (var type in typeof(NetworkManager).Assembly.GetTypes())
        {
            if (!type.IsSubclassOf(typeof(IPacket<>)))
                continue;

            var packetType = type.GetGenericArguments()[0];
            var providerType = typeof(PacketProvider<>).MakeGenericType(packetType);
            var provider = Activator.CreateInstance(providerType);

            if (provider == null)
            {
                AmethystLog.Network.Error(nameof(NetworkManager), $"Failed to create instance of packet provider for {packetType.Name}");
                continue;
            }

            providerType.GetMethod("Hookup")?.Invoke(provider, null);
            Providers.Add(packetType, provider);
        }
    }

    public static void AddHandler<TPacket>(PacketHook<TPacket> hook, int priority = 0)
        => InteractWithProvider<TPacket>(provider => provider.RegisterHandler(hook, priority));

    public static void RemoveHandler<TPacket>(PacketHook<TPacket> hook)
        => InteractWithProvider<TPacket>(provider => provider.UnregisterHandler(hook));

    public static void AddSecurityHandler<TPacket>(PacketHook<TPacket> hook, int priority = 0)
        => InteractWithProvider<TPacket>(provider => provider.RegisterSecurityHandler(hook, priority));

    public static void RemoveSecurityHandler<TPacket>(PacketHook<TPacket> hook)
        => InteractWithProvider<TPacket>(provider => provider.UnregisterSecurityHandler(hook));

    public static void SetMainHandler<TPacket>(PacketHook<TPacket>? hook)
        => InteractWithProvider<TPacket>(provider => provider.SetMainHandler(hook));

    private static void InteractWithProvider<TPacket>(Action<PacketProvider<TPacket>> action)
    {
        if (Providers.TryGetValue(typeof(TPacket), out var provider))
        {
            action((PacketProvider<TPacket>)provider);
        }
        else
        {
            AmethystLog.Network.Error(nameof(NetworkManager), $"No provider found for packet type {typeof(TPacket).Name}");
        }
    }
}
