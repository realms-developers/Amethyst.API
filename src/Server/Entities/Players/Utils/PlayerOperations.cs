using Terraria;
using Terraria.Localization;

namespace Amethyst.Server.Entities.Players;

public sealed class PlayerOperations
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
