
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

    private INetworkClient?[] _clients = new INetworkClient?[256];
    private ISocket _socket = new RemadeTcpSocket();

    public void Initialize(NetworkInstance instance)
    {
        On.Terraria.MessageBuffer.GetData += OnGetData;
        On.Terraria.NetMessage.SendData += OnSendData;
    }

    private void OnSendData(On.Terraria.NetMessage.orig_SendData orig, int packetId, int remoteClient, int ignoreClient, NetworkText text, int number, float number2, float number3, float number4, int number5, int number6, int number7)
    {
        OutcomingPacket packet = new OutcomingPacket((byte)packetId, remoteClient, ignoreClient, text, number, number2, number3, number4, number5, number6, number7);

        PacketHandleResult result = new PacketHandleResult(packet);

        var handlers = NetworkManager.Instance.outcoming[packetId];
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

        var replace = NetworkManager.Instance.outcomingReplace[packetId];
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
        var buffer = self.readBuffer;
        packetId = buffer[start];

        NetPlayer player = PlayerManager.Tracker[self.whoAmI];
        if (packetId == (byte)PacketTypes.SyncCavernMonsterType || packetId <= 0 || packetId >= MessageID.Count)
        {
            return;
        }

        if (length < 1 || length > 999)
        {
            player.Kick("$LOCALIZE packet.invalidLength");
            return;
        }

        // terraria code asmr
        if (Netplay.Clients[self.whoAmI].State < 10 && packetId > 12 && packetId != 67 && packetId != 56 && packetId != 82 && packetId != 93 && packetId != 16 && packetId != 42 && packetId != 50 && packetId != 38 && packetId != 68 && packetId != 147)
        {
            return;
        }

        if (packetId == 82)
        {
            byte moduleId = (byte)BitConverter.ToUInt16(buffer, start + 1);
            if (!OnGetModule(moduleId, player, start, length))
            {
                orig(self, start, length, out packetId);
            }
        }
        else
        {
            if (!OnGetPacket((byte)packetId, player, start, length))
            {
                orig(self, start, length, out packetId);
            }
        }
    }

    // TODO: refactor OnGetPacket and OnGetModule (because its just two copies)

    private static bool OnGetPacket(byte packetId, NetPlayer player, int start, int length)
    {   
        var buffer = NetMessage.buffer[player.Index].readBuffer;
        IncomingPacket packet = new IncomingPacket(packetId, buffer, (byte)player.Index, start + 1, length - 1);

        PacketHandleResult result = new PacketHandleResult(packet);

        var handlers = NetworkManager.Instance.incoming[packetId];
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
                    AmethystLog.Network.Critical("NetworkProvider", $"Failed to handle IncomingPacket '{packetId}' packet :");
                    AmethystLog.Network.Critical("NetworkProvider", ex.ToString());
                }
            });
        }

        if (result.IsHandled == true)
        {
            result.Log();
            return true;
        }

        var replace = NetworkManager.Instance.incomingReplace[packetId];
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
        var buffer = NetMessage.buffer[player.Index].readBuffer;
        IncomingModule packet = new IncomingModule(packetId, buffer, (byte)player.Index, start + 3, length - 3);

        PacketHandleResult result = new PacketHandleResult(packet);

        var handlers = NetworkManager.Instance.incomingModules[packetId];
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
                    AmethystLog.Network.Critical("NetworkProvider", $"Failed to handle IncomingModule '{packetId}' packet :");
                    AmethystLog.Network.Critical("NetworkProvider", ex.ToString());
                }
            });
        }

        if (result.IsHandled == true)
        {
            result.Log();
            return true;
        }

        var replace = NetworkManager.Instance.incomingModuleReplace[packetId];
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
            AmethystLog.Network.Info("Network", $"Server was started on {AmethystSession.Profile.Port} with {AmethystSession.Profile.MaxPlayers} slots.");
        else
            AmethystLog.Network.Error("Network", $"Failed to start server! Be sure that you use different port for this server!");
    }

    private void OnConnected(ISocket client) 
    {
        var ip = client.GetRemoteAddress()!.ToString()!.Split(':')[0];
        var index = Netplay.FindNextOpenClientSlot();

        AmethystLog.Network.Info("Network", $"Accepted connection from {ip} (Index: {index})");

        INetworkClient netClient = new BasicNetworkClient(index, client);

        _clients[index] = netClient;

        Netplay.OnConnectionAccepted(client);
    }

    public void Broadcast(byte[] packet)
    {
        foreach (INetworkClient? client in _clients)
            if (client?.IsConnected == true && client?.IsFrozen == false)
                client?.SendPacket(packet);
    }
    public void Broadcast(byte[] packet, Predicate<INetworkClient> predicate)
    {
        foreach (INetworkClient? client in _clients)
            if (client?.IsConnected == true && client?.IsFrozen == false && predicate(client))
                client?.SendPacket(packet);
    }
    public void Broadcast(byte[] packet, params int[] ignored)
    {
        foreach (INetworkClient? client in _clients)
            if (client?.IsConnected == true && client?.IsFrozen == false && ignored.Contains(client.PlayerIndex) == false)
                client?.SendPacket(packet);
    }
    public INetworkClient? GetClient(int index) => _clients[index];
}
