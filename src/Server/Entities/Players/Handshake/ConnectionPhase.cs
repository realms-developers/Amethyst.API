namespace Amethyst.Server.Entities.Players.Handshake;

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
