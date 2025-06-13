using Amethyst.Systems.Chat.Base.Models;

namespace Amethyst.Server.Systems.Chat.Base;

public interface IChatMessageRenderer
{
    string Name { get; }

    void Render(MessageRenderContext ctx);
}
