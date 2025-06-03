using Amethyst.Network.Packets;
using Amethyst.Server.Entities.Players;

namespace Amethyst.Network.Handling.Handshake;

public static class HandshakeHandler
{
    internal static void Initialize()
    {
        NetworkManager.SetMainHandler<PlayerConnectRequest>(OnPlayerConnectRequest);
    }

    public static void OnPlayerConnectRequest(PlayerEntity plr, ref PlayerConnectRequest packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        var cfg = HandshakeConfiguration.Instance;

        AmethystLog.Network.Info(nameof(HandshakeHandler), $"Protocol: {packet.Protocol}.");
    }
}