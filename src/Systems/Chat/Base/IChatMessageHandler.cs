using Amethyst.Systems.Chat.Base.Models;

namespace Amethyst.Systems.Chat.Base;

public interface IChatMessageHandler
{
    public string Name { get; }

    public void HandleMessage(PlayerMessage message);
}
