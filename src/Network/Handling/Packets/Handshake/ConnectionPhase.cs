namespace Amethyst.Network.Handling.Packets.Handshake;

public enum ConnectionPhase
{
    Disconnected,

    WaitingProtocol,
    WaitingPlayerInfo,
    WaitingUUID,
    WaitingWorldInfoRequest,
    WaitingSectionRequest,
    WaitingPlayerSpawn,

    Connected
}
