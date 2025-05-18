using Amethyst.Network;
using Amethyst.Systems.Characters.Base.Enums;

namespace Amethyst.Systems.Characters.Base.Interactions;

public interface ICharacterEditor
{
    public ICharacterProvider Provider { get; }

    public bool SetSlot(SyncType? sync, int slot, NetItem item);
    public bool SetLife(SyncType? sync, int? current, int? max);
    public bool SetMana(SyncType? sync, int? current, int? max);
    public bool SetColor(SyncType? sync, PlayerColorType colorType, NetColor color);
    public bool SetQuests(SyncType? sync, int completed);
    public bool SetStats(SyncType? sync, PlayerInfo1? stats1 = null, PlayerInfo2? stats2 = null, PlayerInfo3? stats3 = null);
    public bool SetHides(SyncType? sync, bool[]? hideAccessories = null, byte? hideMisc = null);
    public bool SetSkin(SyncType? sync, byte? hairId = null, byte? hairColor = null, byte? skinVariant = null);
}
