
using Amethyst.Core;
using Amethyst.Network.Managing;
using Amethyst.Network.Packets;
using Amethyst.Players;
using Amethyst.Security;
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
                    AmethystLog.Network.Critical(nameof(BasicNetworkProvider), $"Failed to handle OutcomingPacket '{packetId}' packet :");
                    AmethystLog.Network.Critical(nameof(BasicNetworkProvider), ex.ToString());
                }
            });
        }

        if (result.IsHandled)
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

        if (SecurityManager.Configuration.DisabledPackets.Contains(packetId) || packetId <= 0 || packetId >= MessageID.Count)
        {
            AmethystLog.Security.Debug("Network", $"Invalid or blocked packet: {packetId}");
            return;
        }

        if (length < 1 || length > 999)
        {
            player.Kick(Localization.Get("packet.invalidLength", player.Language));
            return;
        }

        // terraria code asmr
        if (Netplay.Clients[self.whoAmI].State < 10 && packetId > 12 && packetId != 67 && packetId != 56 &&
            packetId != 82 && packetId != 93 && packetId != 16 && packetId != 42 && packetId != 50 && packetId != 38
            && packetId != 68 && packetId != 147)
        {
            return;
        }

        if (packetId == 82)
        {
            byte moduleId = (byte)BitConverter.ToUInt16(buffer, start + 1);

            if (SecurityManager.Configuration.DisabledModules.Contains(moduleId))
            {
                AmethystLog.Security.Debug("Network", $"Blocked net-module: {moduleId}");
                return;
            }

            if (player._moduleThreshold.Fire(moduleId))
            {
                AmethystLog.Security.Debug("Network", $"Blocked net-module (threshold): {moduleId}");
                return;
            }

            if (player._sentModules[moduleId] && SecurityManager.Configuration.OneTimeModules.Contains(moduleId))
            {
                AmethystLog.Security.Debug("Network", $"Blocked one-time net-module: {moduleId}");
                return;
            }
            player._sentModules[moduleId] = true;

            if (!OnGetModule(moduleId, player, start, length))
            {
                orig(self, start, length, out packetId);
            }
        }
        else
        {
            if (player._packetThreshold.Fire(packetId))
            {
                AmethystLog.Security.Debug("Network", $"Blocked packet (threshold): {packetId}");
                return;
            }

            if (player._sentPackets[packetId] && SecurityManager.Configuration.OneTimePackets.Contains(packetId))
            {
                AmethystLog.Security.Debug("Network", $"Blocked one-time packet: {packetId}");
                return;
            }
            player._sentPackets[packetId] = true;

            if (!OnGetPacket((byte)packetId, player, start, length))
            {
                orig(self, start, length, out packetId);
            }
        }
    }

    // TODO: refactor OnGetPacket and OnGetModule (because its just two copies)
    private static bool OnGetPacket(byte packetId, NetPlayer player, int start, int length)
    {
        byte[] buffer = NetMessage.buffer[player.Index].readBuffer;

        IncomingPacket packet = new(packetId, buffer, (byte)player.Index, start + 1, length - 1);

        PacketHandleResult result = new(packet);

        List<SecurityHandler<IncomingPacket>> preHandlers = NetworkManager.Instance.PreIncoming[packetId];
        if (preHandlers.Any(p => p(in packet)))
        {
            AmethystLog.Security.Debug("Security", $"Packet '{packetId}' was blocked by security.");
            return true;
        }

        List<PacketHandler<IncomingPacket>> handlers = NetworkManager.Instance.Incoming[packetId];

        if (handlers != null && handlers.Count > 0)
        {
            handlers.ForEach(handler =>
            {
                try
                {
                    handler(in packet, result);
                }
                catch (Exception ex)
                {
                    AmethystLog.Network.Critical(nameof(BasicNetworkProvider), $"Failed to handle IncomingPacket '{packetId}' packet :");
                    AmethystLog.Network.Critical(nameof(BasicNetworkProvider), ex.ToString());
                }
            });
        }

        if (result.IsHandled)
        {
            result.Log();
            return true;
        }

        PacketHandler<IncomingPacket>? replace = NetworkManager.Instance.IncomingReplace[packetId];

        if (replace != null)
        {
            replace(in packet, result);
            result.Log();

            return true;
        }
        return false;
    }


    private static bool OnGetModule(byte packetId, NetPlayer player, int start, int length)
    {
        byte[] buffer = NetMessage.buffer[player.Index].readBuffer;
        IncomingModule packet = new(packetId, buffer, (byte)player.Index, start + 3, length - 3);

        PacketHandleResult result = new(packet);

        List<SecurityHandler<IncomingModule>> preHandlers = NetworkManager.Instance.PreIncomingModules[packetId];
        if (preHandlers.Any(p => p(in packet)))
        {
            AmethystLog.Security.Debug("Security", $"NetModule '{packetId}' was blocked by security.");
            return true;
        }

        List<PacketHandler<IncomingModule>> handlers = NetworkManager.Instance.IncomingModules[packetId];
        if (handlers != null && handlers.Count > 0)
        {
            handlers.ForEach(handler =>
            {
                try
                {
                    handler(in packet, result);
                }
                catch (Exception ex)
                {
                    AmethystLog.Network.Critical(nameof(BasicNetworkProvider), $"Failed to handle IncomingModule '{packetId}' packet :");
                    AmethystLog.Network.Critical(nameof(BasicNetworkProvider), ex.ToString());
                }
            });
        }

        if (result.IsHandled)
        {
            result.Log();

            return true;
        }

        PacketHandler<IncomingModule>? replace = NetworkManager.Instance.IncomingModuleReplace[packetId];

        if (replace != null)
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
            AmethystLog.Network.Info(nameof(BasicNetworkProvider), $"Server was started on {AmethystSession.Profile.Port} with {AmethystSession.Profile.MaxPlayers} slots.");
        }
        else
        {
            AmethystLog.Network.Error(nameof(BasicNetworkProvider), $"Failed to start server! Be sure that you use different port for this server!");
        }
    }

    private void OnConnected(ISocket client)
    {
        string ip = client.GetRemoteAddress()!
            .ToString()!
            .Split(':')[0];

        int index = Netplay.FindNextOpenClientSlot();

        AmethystLog.Network.Info(nameof(BasicNetworkProvider), $"Accepted connection from {ip} (Index: {index})");

        INetworkClient netClient = new BasicNetworkClient(index, client);

        _clients[index] = netClient;

        Netplay.OnConnectionAccepted(client);
    }

    public void Broadcast(byte[] packet)
    {
        foreach (INetworkClient? client in _clients)
        {
            if (client != null && client.IsConnected && !client.IsFrozen)
            {
                client.SendPacket(packet);
            }
        }
    }

    public void Broadcast(byte[] packet, Predicate<INetworkClient> predicate)
    {
        foreach (INetworkClient? client in _clients)
        {
            if (client != null && client.IsConnected && !client.IsFrozen && predicate(client))
            {
                client.SendPacket(packet);
            }
        }
    }

    public void Broadcast(byte[] packet, params int[] ignored)
    {
        foreach (INetworkClient? client in _clients)
        {
            if (client != null &&
                client.IsConnected && !client.IsFrozen && !ignored.Contains(client.PlayerIndex))
            {
                client.SendPacket(packet);
            }
        }
    }

    public INetworkClient? GetClient(int index) => _clients[index];
}
