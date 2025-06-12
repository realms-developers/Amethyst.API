using Amethyst.Network.Handling.Packets.Handshake;
using Amethyst.Network.Packets;
using Amethyst.Network.Structures;
using Amethyst.Server.Entities.Base;

namespace Amethyst.Server.Entities.Players;

public sealed partial class PlayerEntity : IServerEntity
{
    public void Kick(string reason)
    {
        SendPacketBytes(PlayerDisconnectPacket.Serialize(
            new PlayerDisconnect
            {
                Reason = new NetText(0, reason, null)
            }
        ));

        Phase = ConnectionPhase.Disconnected;

        AmethystLog.System.Error(nameof(PlayerEntity), $"Player {Name} ({Index}) has been kicked: {reason}");

        Task.Run(async () =>
        {
            await Task.Delay(1000);
            CloseSocket();
        });
    }
}
