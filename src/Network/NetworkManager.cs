using System.Net;
using Amethyst.API.Network.Handling.Patches;
using Amethyst.Kernel;
using Amethyst.Network.Engine;
using Amethyst.Network.Engine.Delegates;
using Amethyst.Network.Engine.Packets;
using Amethyst.Network.Handling;
using Amethyst.Network.Handling.Mechanism.Sections;
using Amethyst.Network.Handling.NetMessagePatch;
using Amethyst.Network.Handling.Packets.Characters;
using Amethyst.Network.Handling.Packets.Chat;
using Amethyst.Network.Handling.Packets.Chests;
using Amethyst.Network.Handling.Packets.Events;
using Amethyst.Network.Handling.Packets.Handshake;
using Amethyst.Network.Handling.Packets.Items;
using Amethyst.Network.Handling.Packets.NetModules;
using Amethyst.Network.Handling.Packets.NPCs;
using Amethyst.Network.Handling.Packets.Other;
using Amethyst.Network.Handling.Packets.Platform;
using Amethyst.Network.Handling.Packets.Players;
using Amethyst.Network.Handling.Packets.Projectiles;
using Amethyst.Network.Handling.Packets.Signs;
using Amethyst.Network.Handling.Packets.TileEntities;
using Amethyst.Network.Handling.Packets.World;
using Amethyst.Network.Packets;
using Amethyst.Server.Entities;
using Amethyst.Server.Entities.Players;
using Terraria;

namespace Amethyst.Network;

public static class NetworkManager
{
    public static bool IsLocked { get; set; } = true;
    public static int MaxPlayers => AmethystSession.Profile.MaxPlayers;

    public static int SocketAcceptDelay { get; set; } = 1000;
    public static int SocketBacklog { get; set; } = 32;
    public static int SocketLiveCheck { get; set; } = 1000;

    internal static Dictionary<Type, object> Providers = [];

    internal static PacketInvokeHandler?[] InvokeHandlers = new PacketInvokeHandler?[256];
    internal static List<PacketInvokeHandler>?[] DirectHandlers = new List<PacketInvokeHandler>?[256];
    internal static List<PacketInvokeHandler> OverlapHandlers = [];

    private static readonly PacketInvokeHandler[][] _invokeHandlers = new PacketInvokeHandler[256][];
    private static PacketInvokeHandler[] _invokeOverlapHandlers = [];

    internal static AmethystTcpServer? TcpServer;

    private static readonly Timer _socketLifeUpdate = new(static (state) =>
    {
        foreach (PlayerEntity plr in EntityTrackers.Players)
        {
            try
            {
                plr.SendPacketBytes(WorldTimeSyncPacket.Serialize(new WorldTimeSync
                {
                    Time = (int)Main.time,
                    IsDay = (byte)(Main.dayTime ? 1 : 0),
                    SunModY = Main.sunModY,
                    MoonModY = Main.moonModY
                }));
            }
            catch
            {
                plr.Kick("network.socketWasClosed");
            }
        }
    }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(SocketLiveCheck));

    internal static void Initialize()
    {
        RegisterPacketHandlers();

        for (int i = 0; i < _invokeHandlers.Length; i++)
        {
            _invokeHandlers[i] = [];
        }

        NetMessagePatcher.Initialize();
        ModulesPatcher.Initialize();

        HandlerManager.RegisterHandler(new HandshakeHandler());
        HandlerManager.RegisterHandler(new CharactersHandler());
        HandlerManager.RegisterHandler(new ChatHandler());
        HandlerManager.RegisterHandler(new PlatformHandler());
        HandlerManager.RegisterHandler(new SectionHandler());
        HandlerManager.RegisterHandler(new PlayersHandler());
        HandlerManager.RegisterHandler(new OtherHandler());
        HandlerManager.RegisterHandler(new ItemsHandler());
        HandlerManager.RegisterHandler(new NPCsHandler());
        HandlerManager.RegisterHandler(new ProjectilesHandler());
        HandlerManager.RegisterHandler(new SignsHandler());
        HandlerManager.RegisterHandler(new ChestsHandler());
        HandlerManager.RegisterHandler(new WorldHandler());
        HandlerManager.RegisterHandler(new TEHandler());
        HandlerManager.RegisterHandler(new NetModulesHandler());
        HandlerManager.RegisterHandler(new EventsHandler());

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

        Main.rand ??= new();
        try
        {
            bool ignore = false;
            byte packetId = data[2];

            if (_invokeOverlapHandlers.Length > 0)
            {
                for (int i = 0; i < _invokeOverlapHandlers.Length; i++)
                {
                    PacketInvokeHandler handler = _invokeOverlapHandlers[i];
                    handler(EntityTrackers.Players[client._index], data, ref ignore);

                    if (ignore)
                    {
                        return;
                    }
                }
            }

            PacketInvokeHandler[] directHandlers = _invokeHandlers[packetId];
            if (directHandlers != null && directHandlers.Length > 0)
            {
                for (int i = 0; i < directHandlers.Length; i++)
                {
                    PacketInvokeHandler handler = directHandlers[i];
                    handler(EntityTrackers.Players[client._index], data, ref ignore);

                    if (ignore)
                    {
                        return;
                    }
                }
            }

            if (InvokeHandlers[packetId] == null)
            {
                return;
            }

            PlayerEntity player = EntityTrackers.Players[client._index];
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
        foreach (Type type in typeof(NetworkManager).Assembly.GetTypes())
        {
            if (type.Namespace != "Amethyst.Network.Packets"
                || type.GetInterfaces().Any(p => p.Name.StartsWith("IPacket", StringComparison.InvariantCulture))
                || type.Assembly.GetType($"{type.FullName}Packet") == null)
            {
                continue;
            }

            Type providerType = typeof(PacketProvider<>).MakeGenericType(type);
            object? provider = Activator.CreateInstance(providerType);

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

        _invokeOverlapHandlers = [.. OverlapHandlers];
    }

    public static void RemoveOverlapHandler(PacketInvokeHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        OverlapHandlers.Remove(handler);

        _invokeOverlapHandlers = [.. OverlapHandlers];
    }

    public static void AddDirectHandler(int packetType, PacketInvokeHandler handler)
    {
        DirectHandlers[packetType] ??= [];
        ArgumentNullException.ThrowIfNull(handler);

        if (DirectHandlers[packetType]!.Contains(handler))
        {
            AmethystLog.Network.Error(nameof(NetworkManager), $"Handler {handler.Method.Name} is already registered for packet type {packetType}");
            return;
        }
        DirectHandlers[packetType]!.Add(handler);

        _invokeHandlers[packetType] = [.. DirectHandlers[packetType]!];
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

        _invokeHandlers[packetType] = [.. DirectHandlers[packetType]!];
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
        if (Providers.TryGetValue(typeof(TPacket), out object? provider))
        {
            action((PacketProvider<TPacket>)provider);
        }
        else
        {
            AmethystLog.Network.Error(nameof(NetworkManager), $"No provider found for packet type {typeof(TPacket).Name}");
        }
    }

    internal static void AddHandler<T>(int v, object onClearAnchor)
    {
        throw new NotImplementedException();
    }
}
