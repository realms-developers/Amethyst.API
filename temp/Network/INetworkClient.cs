namespace Amethyst.Network;

public interface INetworkClient
{
    /// <summary>
    /// Network player index.
    /// </summary>
    public int PlayerIndex { get; }

    /// <summary>
    /// Indicates that socket is active.
    /// </summary>
    public bool IsConnected { get; }

    /// <summary>
    /// Client state that disables all incoming and outcoming packets.
    /// </summary>
    public bool IsFrozen { get; }

    public void Disconnect(string reason);
    public void SetFreeze(bool state);

    public void SendPacket(byte[] packet);
    public void SendPacket(byte[] packet, int start, int length);
}
