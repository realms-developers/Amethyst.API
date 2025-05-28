using System.Net;
using Amethyst.Kernel;
using Amethyst.Server.Entities;
using Amethyst.Server.Network.Engine;
using Amethyst.Server.Network.Engine.Packets;

namespace Amethyst.Server.Network;

public static class NetworkManager
{
    internal static List<object> Providers { get; } = new List<object>();

    internal static Dictionary<Type, object> RegisterHandlers = new();
    internal static Dictionary<Type, object> UnregisterHandlers = new();
    internal static Dictionary<Type, object> SecurityRegisterHandlers = new();
    internal static Dictionary<Type, object> SecurityUnregisterHandlers = new();
    internal static Dictionary<Type, object> SetMainHandlers = new();
    internal static PacketInvokeHandler?[] InvokeHandlers = new PacketInvokeHandler?[256];

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

            var packetId = data[2];
            if (InvokeHandlers[packetId] == null)
            {
                AmethystLog.Network.Error(nameof(NetworkManager), $"No handler registered for packet ID {packetId}");
                return;
            }

            var ignore = false;
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
            Providers.Add(provider);
        }
    }

    public static void RegisterPacketHandler<TPacket>(PacketHook<TPacket> hook, int priority = 0)
    {
        if (RegisterHandlers.TryGetValue(typeof(TPacket), out var handler))
        {
            var registerHandler = (PacketRegisterHandler<TPacket>)handler;
            registerHandler(hook, priority);
        }
        else
        {
            AmethystLog.Network.Error(nameof(NetworkManager), $"No register handler found for packet type {typeof(TPacket).Name}");
        }
    }

    public static void UnregisterPacketHandler<TPacket>(PacketHook<TPacket> hook)
    {
        if (UnregisterHandlers.TryGetValue(typeof(TPacket), out var handler))
        {
            var unregisterHandler = (PacketUnregisterHandler<TPacket>)handler;
            unregisterHandler(hook);
        }
        else
        {
            AmethystLog.Network.Error(nameof(NetworkManager), $"No unregister handler found for packet type {typeof(TPacket).Name}");
        }
    }

    public static void RegisterSecurityPacketHandler<TPacket>(PacketHook<TPacket> hook, int priority = 0)
    {
        if (SecurityRegisterHandlers.TryGetValue(typeof(TPacket), out var handler))
        {
            var registerHandler = (PacketRegisterHandler<TPacket>)handler;
            registerHandler(hook, priority);
        }
        else
        {
            AmethystLog.Network.Error(nameof(NetworkManager), $"No security register handler found for packet type {typeof(TPacket).Name}");
        }
    }

    public static void UnregisterSecurityPacketHandler<TPacket>(PacketHook<TPacket> hook)
    {
        if (SecurityUnregisterHandlers.TryGetValue(typeof(TPacket), out var handler))
        {
            var unregisterHandler = (PacketUnregisterHandler<TPacket>)handler;
            unregisterHandler(hook);
        }
        else
        {
            AmethystLog.Network.Error(nameof(NetworkManager), $"No security unregister handler found for packet type {typeof(TPacket).Name}");
        }
    }
    public static void SetMainPacketHandler<TPacket>(PacketHook<TPacket>? hook)
    {
        if (SetMainHandlers.TryGetValue(typeof(TPacket), out var handler))
        {
            var setMainHandler = (PacketSetMainHandler<TPacket>)handler;
            setMainHandler(hook);
        }
        else
        {
            AmethystLog.Network.Error(nameof(NetworkManager), $"No set main handler found for packet type {typeof(TPacket).Name}");
        }
    }
}
