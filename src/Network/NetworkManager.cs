using System.Net;
using Amethyst.Kernel;
using Amethyst.Server.Entities;
using Amethyst.Network.Engine;
using Amethyst.Network.Engine.Delegates;
using Amethyst.Network.Engine.Packets;
using Amethyst.Network.Engine.Patching;
using Amethyst.Network.Handling.Handshake;

namespace Amethyst.Network;

public static class NetworkManager
{
    public static bool IsLocked { get; set; } = true;
    public static int MaxPlayers => AmethystSession.Profile.MaxPlayers;

    public static int SocketAcceptDelay { get; set; } = 50;
    public static int SocketBacklog { get; set; } = 8;

    internal static Dictionary<Type, object> Providers = new Dictionary<Type, object>();

    internal static PacketInvokeHandler?[] InvokeHandlers = new PacketInvokeHandler?[256];
    internal static List<PacketInvokeHandler>?[] DirectHandlers = new List<PacketInvokeHandler>?[256];
    internal static List<PacketInvokeHandler> OverlapHandlers = new List<PacketInvokeHandler>();

    private static PacketInvokeHandler[][] _InvokeHandlers = new PacketInvokeHandler[256][];
    private static PacketInvokeHandler[] _InvokeOverlapHandlers = [];

    internal static AmethystTcpServer? TcpServer;

    internal static void Initialize()
    {
        RegisterPacketHandlers();

        for (int i = 0; i < _InvokeHandlers.Length; i++)
        {
            _InvokeHandlers[i] = [];
        }

        NetworkPatcher.Initialize();

        HandshakeHandler.Initialize();

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
            var packetId = data[2];

            if (_InvokeOverlapHandlers.Length > 0)
            {
                for (int i = 0; i < _InvokeOverlapHandlers.Length; i++)
                {
                    var handler = _InvokeOverlapHandlers[i];
                    handler(EntityTrackers.Players[client._index], data, ref ignore);

                    if (ignore)
                        return;
                }
            }

            var directHandlers = _InvokeHandlers[packetId];
            if (directHandlers != null && directHandlers.Length > 0)
            {
                for (int i = 0; i < directHandlers.Length; i++)
                {
                    var handler = directHandlers[i];
                    handler(EntityTrackers.Players[client._index], data, ref ignore);

                    if (ignore)
                        return;
                }
            }

            if (InvokeHandlers[packetId] == null)
            {
                AmethystLog.Network.Debug(nameof(NetworkManager), $"No main handler registered for packet ID {packetId}");
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
            if (type.Namespace != "Amethyst.Network.Packets"
                || type.GetInterfaces().Any(p => p.Name.StartsWith("IPacket"))
                || type.Assembly.GetType($"{type.FullName}Packet") == null)
                continue;


            var providerType = typeof(PacketProvider<>).MakeGenericType(type);
            var provider = Activator.CreateInstance(providerType);

            if (provider == null)
            {
                AmethystLog.Network.Error(nameof(NetworkManager), $"Failed to create instance of packet provider for {type.Name}");
                continue;
            }

            providerType.GetMethod("Hookup")?.Invoke(provider, null);
            Providers.Add(type, provider);
        }
    }

    public static void AddOverlapHandler(PacketInvokeHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        OverlapHandlers.Add(handler);

        _InvokeOverlapHandlers = OverlapHandlers.ToArray();
    }

    public static void RemoveOverlapHandler(PacketInvokeHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        OverlapHandlers.Remove(handler);

        _InvokeOverlapHandlers = OverlapHandlers.ToArray();
    }

    public static void AddDirectHandler(int packetType, PacketInvokeHandler handler)
    {
        DirectHandlers[packetType] ??= new List<PacketInvokeHandler>();
        ArgumentNullException.ThrowIfNull(handler);

        if (DirectHandlers[packetType]!.Contains(handler))
        {
            AmethystLog.Network.Error(nameof(NetworkManager), $"Handler {handler.Method.Name} is already registered for packet type {packetType}");
            return;
        }
        DirectHandlers[packetType]!.Add(handler);

        _InvokeHandlers[packetType] = DirectHandlers[packetType]!.ToArray();
    }

    public static void RemoveDirectHandler(int packetType, PacketInvokeHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        if (DirectHandlers[packetType] == null || !DirectHandlers[packetType]!.Contains(handler))
        {
            AmethystLog.Network.Error(nameof(NetworkManager), $"Handler {handler.Method.Name} is not registered for packet type {packetType}");
            return;
        }
        DirectHandlers[packetType]!.Remove(handler);

        _InvokeHandlers[packetType] = DirectHandlers[packetType]!.ToArray();
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
