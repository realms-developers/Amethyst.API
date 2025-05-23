namespace Amethyst.Server.Entities.Players;

public sealed class PlayerNetworkUtils
{
    internal PlayerNetworkUtils(PlayerEntity player)
    {
        Player = player;
    }

    public PlayerEntity Player { get; }

    public PlayerOperations Operations { get; } = new();

    public void Kick(string reason) =>
        (Operations.KickOperation ?? PlayerOperations.DefaultKickOperation).Invoke(Player, reason);

    public void SendBytes(byte[] bytes) =>
        (Operations.SendBytesOperation ?? PlayerOperations.DefaultSendBytesOperation).Invoke(Player, bytes);
}
