using Amethyst.Network.Handling.Handshake;
using Amethyst.Network.Packets;
using Amethyst.Network.Structures;
using Amethyst.Server.Entities.Base;

namespace Amethyst.Server.Entities.Players;

public sealed partial class PlayerEntity : IServerEntity
{
    public PlayerModerationOperations ModerationOperations { get; }

    public void Kick(string reason) =>
        (ModerationOperations.Kick ?? PlayerModerationOperations.DefaultKick).Invoke(this, reason);

    public sealed class PlayerModerationOperations
    {
        public static Action<PlayerEntity, string> DefaultKick { get; set; } = static (player, reason) =>
        {
            player.SendPacketBytes(PlayerDisconnectPacket.Serialize(
                new PlayerDisconnect
                {
                    Reason = new NetText(0, reason, null)
                }
            ));

            player.Phase = ConnectionPhase.Disconnected;
        };
        public Action<PlayerEntity, string>? Kick { get; set; }
    }
}
