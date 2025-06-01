using Amethyst.Server.Network.Packets;

namespace Amethyst.Systems.Characters.Base.Interactions;

public interface ICharacterHandler
{
    public ICharacterProvider Provider { get; }

    public bool InReadonlyMode { get; set; }

    public void HandleSlot(PlayerSlot packet);
    public void HandleSetLife(PlayerLife packet);
    public void HandleSetMana(PlayerMana packet);
    public void HandlePlayerInfo(PlayerInfo packet);
    public void HandleQuests(PlayerTownNPCQuestsStats packet);
}
