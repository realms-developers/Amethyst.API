using Amethyst.Server.Entities.Base;
using Amethyst.Server.Entities.Items;
using Terraria.DataStructures;
using Terraria;
using Terraria.Localization;
using Amethyst.Network.Structures;
using Amethyst.Enums;

namespace Amethyst.Server.Entities.Players;

public sealed partial class PlayerEntity : IServerEntity
{
    public void GiveItem(int id, int stack, byte prefix)
    {
        ItemUtils.CreateItem(Position.X, Position.Y, Index, id, stack, prefix);
    }

    public void AddBuff(int type, TimeSpan duration)
    {
        int time = (int)duration.TotalSeconds * 60;
        NetMessage.SendData(55, -1, -1, NetworkText.Empty, Index, type, number3: time);
    }

    public void Heal(int amount)
    {
        NetMessage.SendData(66, -1, -1, NetworkText.Empty, Index, amount);
    }

    public void Hurt(int damage, string text, bool pvp = false)
    {
        Hurt(damage, PlayerDeathReason.ByCustomReason(text), pvp);
    }

    public void Hurt(int damage, PlayerDeathReason? reason = null, bool pvp = false)
    {
        NetMessage.SendPlayerHurt(Index, reason ?? PlayerDeathReason.LegacyDefault(), damage, -1, false, pvp, 0);
    }

    public void Kill(string text, bool pvp = false)
    {
        Kill(PlayerDeathReason.ByCustomReason(text), pvp);
    }

    public void Kill(PlayerDeathReason? reason = null, bool pvp = false)
    {
        NetMessage.SendPlayerDeath(Index, reason ?? PlayerDeathReason.LegacyDefault(), short.MaxValue, -1, pvp);
    }

    public void Teleport(float x, float y)
    {
        NetMessage.SendData(65, -1, -1, NetworkText.Empty, Index, x, y);
    }

    public void Teleport(NetVector2 position)
    {
        Teleport(position.X, position.Y);
    }

    public void SetPvP(bool enabled)
    {
        NetMessage.SendData((byte)PacketID.PlayerPvP, -1, -1, NetworkText.Empty, Index, enabled ? 1 : 0);
    }

    public void SetTeam(int teamId)
    {
        NetMessage.SendData((byte)PacketID.PlayerSetTeam, -1, -1, NetworkText.Empty, Index, teamId);
    }
}
