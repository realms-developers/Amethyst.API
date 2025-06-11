using Amethyst.Server.Entities.Base;
using Amethyst.Server.Entities.Items;
using Terraria.DataStructures;
using Terraria;
using Terraria.Localization;

namespace Amethyst.Server.Entities.Players;

public sealed partial class PlayerEntity : IServerEntity
{
    public PlayerGameplayOperations GameplayOperations { get; }

    public void GiveItem(int id, int stack, byte prefix)
        => (GameplayOperations.GiveItem ?? PlayerGameplayOperations.DefaultGiveItem).Invoke(this, id, stack, prefix);

    public void AddBuff(int type, TimeSpan duration)
        => (GameplayOperations.AddBuff ?? PlayerGameplayOperations.DefaultAddBuff).Invoke(this, type, duration);

    public void Heal(int amount)
        => (GameplayOperations.Heal ?? PlayerGameplayOperations.DefaultHeal).Invoke(this, amount);

    public void Hurt(int damage, string text, bool pvp = false)
        => Hurt(damage, PlayerDeathReason.ByCustomReason(text), pvp);

    public void Hurt(int damage, PlayerDeathReason? reason = null, bool pvp = false)
        => (GameplayOperations.Hurt ?? PlayerGameplayOperations.DefaultHurt).Invoke(this, reason, damage, pvp);

    public void Kill(string text, bool pvp = false)
        => Kill(PlayerDeathReason.ByCustomReason(text), pvp);

    public void Kill(PlayerDeathReason? reason = null, bool pvp = false)
        => (GameplayOperations.Kill ?? PlayerGameplayOperations.DefaultKill).Invoke(this, reason, pvp);

    public sealed class PlayerGameplayOperations
    {
        public static Action<PlayerEntity, int, int, byte> DefaultGiveItem { get; set; } = static (player, id, stack, prefix) =>
        {
            ItemUtils.CreateItem(player.Position.X, player.Position.Y, player.Index, id, stack, prefix);
        };
        public Action<PlayerEntity, int, int, byte>? GiveItem { get; set; }

        public static Action<PlayerEntity, int, TimeSpan> DefaultAddBuff { get; set; } = static (player, type, duration) =>
        {
            int time = (int)duration.TotalSeconds * 60;
            NetMessage.SendData(55, -1, -1, NetworkText.Empty, player.Index, type, number3: time);
        };
        public Action<PlayerEntity, int, TimeSpan>? AddBuff { get; set; }

        public static Action<PlayerEntity, int> DefaultHeal { get; set; } = static (player, amount) =>
        {
            NetMessage.SendData(66, -1, -1, NetworkText.Empty, player.Index, amount);
        };
        public Action<PlayerEntity, int>? Heal { get; set; }

        public static Action<PlayerEntity, PlayerDeathReason?, int, bool> DefaultHurt { get; set; } = static (player, reason, damage, pvp) =>
        {
            NetMessage.SendPlayerHurt(player.Index, reason ?? PlayerDeathReason.LegacyDefault(), damage, -1, false, pvp, 0);
        };
        public Action<PlayerEntity, PlayerDeathReason?, int, bool>? Hurt { get; set; }

        public static Action<PlayerEntity, PlayerDeathReason?, bool> DefaultKill { get; set; } = static (player, reason, pvp) =>
        {
            NetMessage.SendPlayerDeath(player.Index, reason ?? PlayerDeathReason.LegacyDefault(), short.MaxValue, -1, pvp);
        };
        public Action<PlayerEntity, PlayerDeathReason?, bool>? Kill { get; set; }
    }
}
