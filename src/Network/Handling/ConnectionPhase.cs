namespace Amethyst.Network.Handling;

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
