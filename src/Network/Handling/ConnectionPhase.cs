namespace Amethyst.Network.Handling;

public enum ConnectionPhase
{
    WaitingProtocol,
    WaitingPlayerInfo,
    WaitingUUID,
    WaitingWorldInfoRequest,
    WaitingSectionRequest,

    Connected
}
