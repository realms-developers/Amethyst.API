using Amethyst.Systems.Characters.Enums;

namespace Amethyst.Systems.Characters.Base.Interactions;

public interface ICharacterSynchroniser
{
    public ICharacterProvider Provider { get; }

    public void SyncSlot(SyncType sync, int slot);
    public void SyncLife(SyncType sync);
    public void SyncMana(SyncType sync);
    public void SyncPlayerInfo(SyncType sync);
    public void SyncQuests(SyncType sync);
}
