using Amethyst.Server.Entities.Base;
using Terraria;
using Terraria.Localization;

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
            NetMessage.SendData(2, player.Index, -1, NetworkText.FromLiteral(reason));
        };
        public Action<PlayerEntity, string>? Kick { get; set; }
    }
}
