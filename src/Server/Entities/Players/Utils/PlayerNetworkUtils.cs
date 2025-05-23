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
        (Operations.PersonalKickOperation ?? PlayerOperations.GlobalKickOperation).Invoke(Player, reason);

    public void SendBytes(byte[] bytes) =>
        (Operations.PersonalSendBytesOperation ?? PlayerOperations.GlobalSendBytesOperation).Invoke(Player, bytes);
}
