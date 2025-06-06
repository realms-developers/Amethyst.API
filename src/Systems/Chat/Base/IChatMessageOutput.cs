using Amethyst.Systems.Chat.Base.Models;

namespace Amethyst.Systems.Chat.Base;

public interface IChatMessageOutput
{
    string Name { get; }

    void OutputMessage(MessageRenderResult message);
}
