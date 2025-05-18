using Amethyst.Network.Packets;

namespace Amethyst.Systems.Characters.Base.Interactions;

public interface ICharacterHandler
{
    public ICharacterProvider Provider { get; }

    public void HandleSlot(IncomingPacket packet);
    public void HandleSetLife(IncomingPacket packet);
    public void HandleSetMana(IncomingPacket packet);
    public void HandlePlayerInfo(IncomingPacket packet);
    public void HandleQuests(IncomingPacket packet);
}
