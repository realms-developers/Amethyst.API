using Amethyst.Server.Entities.Base;
using Terraria;
using Terraria.Localization;

namespace Amethyst.Server.Entities.Players.Modules;

public sealed class PlayerNetworkModule : IEntityModule<PlayerEntity, PlayerNetworkModule.PlayerNetworkOperations>
{
    public PlayerNetworkModule(PlayerEntity baseEntity, PlayerNetworkOperations operations)
    {
        BaseEntity = baseEntity;
        Operations = operations;
    }

    public PlayerEntity BaseEntity { get; }

    public PlayerNetworkOperations Operations { get; }

    public void Kick(string reason) =>
        (Operations.KickOperation ?? PlayerNetworkOperations.DefaultKickOperation).Invoke(BaseEntity, reason);

    public void SendBytes(byte[] bytes) =>
        (Operations.SendBytesOperation ?? PlayerNetworkOperations.DefaultSendBytesOperation).Invoke(BaseEntity, bytes);

    public sealed class PlayerNetworkOperations
    {
        public static Action<PlayerEntity, string> DefaultKickOperation { get; set; } = static (player, reason) =>
        {
            NetMessage.SendData(2, player.Index, -1, NetworkText.FromLiteral(reason));
        };
        public Action<PlayerEntity, string>? KickOperation { get; set; }

        public static Action<PlayerEntity, byte[]> DefaultSendBytesOperation { get; set; } = static (player, bytes) =>
        {
            var socket = Netplay.Clients[player.Index].Socket;
            if (socket.IsConnected() && !Netplay.Clients[player.Index].PendingTermination)
            {
                socket.AsyncSend(bytes, 0, bytes.Length, Netplay.Clients[player.Index].ServerWriteCallBack);
            }
        };
        public Action<PlayerEntity, byte[]>? SendBytesOperation { get; set; }
    }
}
