using Amethyst.Gameplay.Players;
using Amethyst.Network;
using Amethyst.Systems.Characters.Base;
using Amethyst.Systems.Characters.Base.Enums;
using Amethyst.Systems.Characters.Base.Interactions;
using Amethyst.Systems.Users.Players;
using Terraria;

namespace Amethyst.Systems.Characters.Serverside.Interactions;

public sealed class ServersideCharacterEditor : ICharacterEditor
{
    public ServersideCharacterEditor(ICharacterProvider provider)
    {
        Provider = provider;

        PlayerUser user = (provider.User as PlayerUser)!;

        Player = user.Player;
        TPlayer = user.Player.TPlayer;
    }

    public ICharacterProvider Provider { get; }

    public NetPlayer Player { get; }
    public Player TPlayer { get; }

    public bool SetColor(SyncType? sync, PlayerColorType colorType, NetColor color)
    {
        Provider.CurrentModel.Colors[(byte)colorType] = color;

        if (sync != null)
            Provider.Synchronizer.SyncPlayerInfo(sync.Value);

        return true;
    }

    public bool SetHides(SyncType? sync, bool[]? hideAccessories = null, byte? hideMisc = null)
    {
        if (hideAccessories != null)
        {
            for (int i = 0; i < hideAccessories.Length; i++)
                Provider.CurrentModel.HideAccessories[i] = hideAccessories[i];
        }

        if (hideMisc != null)
            Provider.CurrentModel.HideMisc = hideMisc.Value;

        if (sync != null)
            Provider.Synchronizer.SyncPlayerInfo(sync.Value);

        return true;
    }

    public bool SetLife(SyncType? sync, int? current, int? max)
    {
        if (current != null)
            Provider.CurrentModel.MaxLife = current.Value;

        if (max != null)
            Provider.CurrentModel.MaxLife = max.Value;

        if (sync != null)
            Provider.Synchronizer.SyncLife(sync.Value);

        return true;
    }

    public bool SetMana(SyncType? sync, int? current, int? max)
    {
        if (current != null)
            Provider.CurrentModel.MaxMana = current.Value;

        if (max != null)
            Provider.CurrentModel.MaxMana = max.Value;

        if (sync != null)
            Provider.Synchronizer.SyncMana(sync.Value);

        return true;
    }

    public bool SetQuests(SyncType? sync, int completed)
    {
        Provider.CurrentModel.QuestsCompleted = completed;

        if (sync != null)
            Provider.Synchronizer.SyncQuests(sync.Value);

        return true;
    }

    public bool SetSkin(SyncType? sync, byte? hairId = null, byte? hairColor = null, byte? skinVariant = null)
    {
        if (hairId != null)
            Provider.CurrentModel.Hair = hairId.Value;

        if (hairColor != null)
            Provider.CurrentModel.HairDye = hairColor.Value;

        if (skinVariant != null)
            Provider.CurrentModel.SkinVariant = skinVariant.Value;

        if (sync != null)
            Provider.Synchronizer.SyncPlayerInfo(sync.Value);

        return true;
    }

    public bool SetSlot(SyncType? sync, int slot, NetItem item)
    {
        if (slot >= 59 && slot <= 88)
        {
            int fixedSlot = 260 + 30 * TPlayer.CurrentLoadoutIndex + (slot - 59);
            Provider.CurrentModel.Slots[fixedSlot] = item;

            if (sync != null)
            {
                Provider.Synchronizer.SyncSlot(sync.Value, fixedSlot);
            }
        }

        Provider.CurrentModel.Slots[slot] = item;

        if (sync != null)
            Provider.Synchronizer.SyncSlot(sync.Value, slot);

        return true;
    }

    public bool SetStats(SyncType? sync, PlayerInfo1? stats1 = null, PlayerInfo2? stats2 = null, PlayerInfo3? stats3 = null)
    {
        if (stats1 != null)
            Provider.CurrentModel.Info1 = stats1.Value;

        if (stats2 != null)
            Provider.CurrentModel.Info2 = stats2.Value;

        if (stats3 != null)
            Provider.CurrentModel.Info3 = stats3.Value;

        if (sync != null)
            Provider.Synchronizer.SyncPlayerInfo(sync.Value);

        return true;
    }
}
