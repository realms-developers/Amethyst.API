
using Amethyst.Core;
using Amethyst.Network.Managing;
using Amethyst.Network.Packets;
using Amethyst.Players;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Net.Sockets;

namespace Amethyst.Network.Implementation;

internal sealed class BasicNetworkProvider : INetworkProvider
{
    public string Name => "Amethyst.BasicNetworkProvider";

    private readonly INetworkClient?[] _clients = new INetworkClient?[256];
    private readonly ISocket _socket = new RemadeTcpSocket();

    // HashSet of allowed packets during pre-connection state (<10)
    private static readonly HashSet<byte> _allowedPreState10Packets = [
        (byte)PacketTypes.Placeholder,        // 67
        (byte)PacketTypes.NpcName,            // 56
        (byte)PacketTypes.LoadNetModule,      // 82
        (byte)PacketTypes.SocialHandshake,    // 93
        (byte)PacketTypes.PlayerHp,           // 16
        (byte)PacketTypes.PlayerMana,         // 42
        (byte)PacketTypes.PlayerBuff,         // 50
        (byte)PacketTypes.PasswordSend,       // 38
        (byte)PacketTypes.ClientUUID,         // 68
        (byte)PacketTypes.SyncLoadout         // 147
    ];

    public void Initialize(NetworkInstance instance)
    {
        On.Terraria.MessageBuffer.GetData += OnGetData;
        On.Terraria.NetMessage.SendData += OnSendData;
    }

    private void OnSendData(On.Terraria.NetMessage.orig_SendData orig, int packetId, int remoteClient,
        int ignoreClient, NetworkText text, int number, float number2,
        float number3, float number4, int number5, int number6, int number7)
    {
        OutcomingPacket packet = new((byte)packetId, remoteClient, ignoreClient, text, number, number2, number3, number4, number5, number6, number7);

        PacketHandleResult result = new(packet);

        List<PacketHandler<OutcomingPacket>> handlers = NetworkManager.Instance.Outcoming[packetId];
        if (handlers != null && handlers.Count > 0)
        {
            handlers.ForEach(handler =>
            {
                try
                {
                    handler(packet, result);
                }
                catch (Exception ex)
                {
                    AmethystLog.Network.Critical("NetworkProvider", $"Failed to handle OutcomingPacket '{packetId}' packet :");
                    AmethystLog.Network.Critical("NetworkProvider", ex.ToString());
                }
            });
        }

        if (result.IsHandled == true)
        {
            result.Log();
            return;
        }

        PacketHandler<OutcomingPacket>? replace = NetworkManager.Instance.OutcomingReplace[packetId];
        if (replace != null)
        {
            replace(packet, result);
            result.Log();
            return;
        }

        orig(packetId, remoteClient, ignoreClient, text, number, number2, number3, number4, number5, number6, number7);
    }

    private void OnGetData(On.Terraria.MessageBuffer.orig_GetData orig, MessageBuffer self, int start, int length, out int packetId)
    {
        byte[] buffer = self.readBuffer;
        packetId = buffer[start];
        NetPlayer player = PlayerManager.Tracker[self.whoAmI];

        // Early exit for invalid or special packets
        if (ShouldSkipPacketProcessing(packetId))
        {
            return;
        }

        // Validate packet length
        if (IsInvalidPacketLength(length))
        {
            player.Kick(Localization.Get("packet.invalidLength", player.Language));
            return;
        }

        // Check client state restrictions
        if (IsRestrictedClientState(self.whoAmI, packetId))
        {
            return;
        }

        // Process packet based on type
        bool handled = packetId == (byte)PacketTypes.LoadNetModule
            ? ProcessModulePacket(buffer, start, player, length)
            : ProcessRegularPacket(packetId, player, start, length);

        if (!handled)
        {
            orig(self, start, length, out packetId);
        }
    }

    private static bool OnGetPacket(byte packetId, NetPlayer player, int start, int length)
        => HandleIncomingType(
            packetId,
            player,
            (id, buf, sender) => new IncomingPacket(id, buf, sender, start + 1, length - 1),
            NetworkManager.Instance.Incoming,
            NetworkManager.Instance.IncomingReplace,
            "IncomingPacket"
        );

    private static bool OnGetModule(byte packetId, NetPlayer player, int start, int length)
        => HandleIncomingType(
            packetId,
            player,
            (id, buf, sender) => new IncomingModule(id, buf, sender, start + 3, length - 3),
            NetworkManager.Instance.IncomingModules,
            NetworkManager.Instance.IncomingModuleReplace,
            "IncomingModule"
        );

    private static bool HandleIncomingType<T>(
        byte packetId,
        NetPlayer player,
        Func<byte, byte[], byte, T> createPacket,
        List<PacketHandler<T>>[] handlersSource,
        PacketHandler<T>?[] replaceSource,
        string logPrefix) where T : IPacket
    {
        byte[] buffer = NetMessage.buffer[player.Index].readBuffer;
        T packet = createPacket(packetId, buffer, (byte)player.Index);

        PacketHandleResult? result = new(packet);
        List<PacketHandler<T>> handlers = handlersSource[packetId];

        if (handlers?.Count > 0)
        {
            foreach (PacketHandler<T> handler in handlers)
            {
                try
                {
                    handler(in packet, result);
                }
                catch (Exception ex)
                {
                    AmethystLog.Network.Critical("NetworkProvider",
                        $"Failed to handle {logPrefix} '{packetId}': {ex}");
                }
            }
        }

        if (result.IsHandled)
        {
            result.Log();
            return true;
        }

        if (replaceSource[packetId] is { } replace)
        {
            replace(in packet, result);
            result.Log();
            return true;
        }

        return false;
    }

    public void StartListening()
    {
        if (_socket.StartListening(OnConnected))
        {
            AmethystLog.Network.Info("Network", $"Server was started on {AmethystSession.Profile.Port} with {AmethystSession.Profile.MaxPlayers} slots.");
        }
        else
        {
            AmethystLog.Network.Error("Network", $"Failed to start server! Be sure that you use different port for this server!");
        }
    }

    private void OnConnected(ISocket client)
    {
        string ip = client.GetRemoteAddress()!
            .ToString()!
            .Split(':')[0];

        int index = Netplay.FindNextOpenClientSlot();

        AmethystLog.Network.Info("Network", $"Accepted connection from {ip} (Index: {index})");

        INetworkClient netClient = new BasicNetworkClient(index, client);

        _clients[index] = netClient;

        Netplay.OnConnectionAccepted(client);
    }

    private static bool ShouldSkipPacketProcessing(int packetId) =>
    packetId == (byte)PacketTypes.SyncCavernMonsterType ||
    packetId <= 0 ||
    packetId >= MessageID.Count;

    private static bool IsInvalidPacketLength(int length) =>
        length < 1 || length > 999;

    private static bool IsRestrictedClientState(int clientId, int packetId)
    {
        RemoteClient client = Netplay.Clients[clientId];

        return client.State < 10 &&
               packetId > 12 &&
               !_allowedPreState10Packets.Contains((byte)packetId);
    }

    private static bool ProcessModulePacket(byte[] buffer, int start, NetPlayer player, int length)
    {
        byte moduleId = (byte)BitConverter.ToUInt16(buffer, start + 1);
        return OnGetModule(moduleId, player, start, length);
    }

    private static bool ProcessRegularPacket(int packetId, NetPlayer player, int start, int length) =>
        OnGetPacket((byte)packetId, player, start, length);

    public void Broadcast(byte[] packet)
    {
        foreach (INetworkClient? client in _clients)
        {
            if (client?.IsConnected == true && client?.IsFrozen == false)
            {
                client?.SendPacket(packet);
            }
        }
    }

    public void Broadcast(byte[] packet, Predicate<INetworkClient> predicate)
    {
        foreach (INetworkClient? client in _clients)
        {
            if (client?.IsConnected == true && client?.IsFrozen == false && predicate(client))
            {
                client?.SendPacket(packet);
            }
        }
    }

    public void Broadcast(byte[] packet, params int[] ignored)
    {
        foreach (INetworkClient? client in _clients)
        {
            if (client?.IsConnected == true && client?.IsFrozen == false && ignored.Contains(client.PlayerIndex) == false)
            {
                client?.SendPacket(packet);
            }
        }
    }

    public INetworkClient? GetClient(int index) => _clients[index];
}
