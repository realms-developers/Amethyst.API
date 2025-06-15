using Amethyst.Network.Structures;
using Amethyst.Server.Entities.Players;
using Amethyst.Systems.Characters.Base;
using Amethyst.Systems.Characters.Base.Interactions;
using Amethyst.Systems.Characters.Enums;
using Amethyst.Systems.Users.Players;
using Terraria;

namespace Amethyst.Systems.Characters.Serverside.Interactions;

public sealed class ServersideCharacterEditor : ICharacterEditor
{
    public ServersideCharacterEditor(ICharacterProvider provider)
    {
        Provider = provider;

        if (provider.User is not PlayerUser)
        {
            throw new InvalidOperationException("Provider user is not a PlayerUser.");
        }

        PlayerUser user = (PlayerUser)provider.User;

        Player = user.Player;
        TPlayer = user.Player.TPlayer;
    }

    public ICharacterProvider Provider { get; }

    public PlayerEntity Player { get; }
    public Player TPlayer { get; }

    public bool SetColor(SyncType? sync, PlayerColorType colorType, NetColor color)
    {
        Provider.CurrentModel.Colors[(byte)colorType] = color;

        SyncIfNeeded(sync, Provider.Synchronizer.SyncPlayerInfo);

        return true;
    }

    public bool SetHides(SyncType? sync, bool[]? hideAccessories = null, byte? hideMisc = null)
    {
        if (hideAccessories != null)
        {
            for (int i = 0; i < hideAccessories.Length; i++)
            {
                Provider.CurrentModel.HideAccessories[i] = hideAccessories[i];
            }
        }

        if (hideMisc != null)
        {
            Provider.CurrentModel.HideMisc = hideMisc.Value;
        }

        SyncIfNeeded(sync, Provider.Synchronizer.SyncPlayerInfo);

        return true;
    }

    public bool SetLife(SyncType? sync, int? current, int? max)
    {
        if (current != null)
        {
            Player.TPlayer.statLife = current.Value;
        }

        if (max != null)
        {
            Provider.CurrentModel.MaxLife = max.Value;
        }

        SyncIfNeeded(sync, Provider.Synchronizer.SyncLife);

        return true;
    }

    public bool SetMana(SyncType? sync, int? current, int? max)
    {
        if (current != null)
        {
            Player.TPlayer.statMana = current.Value;
        }

        if (max != null)
        {
            Provider.CurrentModel.MaxMana = max.Value;
        }

        SyncIfNeeded(sync, Provider.Synchronizer.SyncMana);

        return true;
    }

    public bool SetQuests(SyncType? sync, int completed)
    {
        Provider.CurrentModel.QuestsCompleted = completed;

        SyncIfNeeded(sync, Provider.Synchronizer.SyncQuests);

        return true;
    }

    public bool SetSkin(SyncType? sync, byte? hairId = null, byte? hairColor = null, byte? skinVariant = null)
    {
        if (hairId != null)
        {
            Provider.CurrentModel.Hair = hairId.Value;
        }

        if (hairColor != null)
        {
            Provider.CurrentModel.HairDye = hairColor.Value;
        }

        if (skinVariant != null)
        {
            Provider.CurrentModel.SkinVariant = skinVariant.Value;
        }

        SyncIfNeeded(sync, Provider.Synchronizer.SyncPlayerInfo);

        return true;
    }

    public bool SetSlot(SyncType? sync, int slot, NetItem item)
    {
        if (slot >= 59 && slot <= 88)
        {
            int fixedSlot = 260 + 30 * Provider.LoadoutIndex + (slot - 59);
            SetSlot(sync, fixedSlot, item);

            if (sync != null) // if sync is null, then sync not needed
            {
                Provider.Synchronizer.SyncSlot(SyncType.Exclude, fixedSlot); // there is Exclude sync, because its not needed to sync with owner (it can break something)
            }
        }

        Provider.CurrentModel.Slots[slot] = item;

        if (sync != null)
        {
            Provider.Synchronizer.SyncSlot(sync.Value, slot);
        }

        return true;
    }

    public bool SetStats(SyncType? sync, PlayerInfo1? stats1 = null, PlayerInfo2? stats2 = null, PlayerInfo3? stats3 = null)
    {
        if (stats1 != null)
        {
            Provider.CurrentModel.Info1 = stats1.Value;
        }

        if (stats2 != null)
        {
            Provider.CurrentModel.Info2 = stats2.Value;
        }

        if (stats3 != null)
        {
            Provider.CurrentModel.Info3 = stats3.Value;
        }

        SyncIfNeeded(sync, Provider.Synchronizer.SyncPlayerInfo);

        return true;
    }

    private void SyncIfNeeded(SyncType? sync, Action<SyncType> action)
    {
        if (sync != null)
        {
            action(sync.Value);
        }
    }
}
