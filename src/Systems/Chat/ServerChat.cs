using Amethyst.Server.Entities.Players;
using Amethyst.Server.Systems.Chat.Base;
using Amethyst.Systems.Chat.Base;
using Amethyst.Systems.Chat.Base.Models;
using Amethyst.Systems.Chat.Misc;
using Amethyst.Systems.Chat.Misc.Context;
using Amethyst.Systems.Chat.Primitive.Misc;

namespace Amethyst.Systems.Chat;

public static class ServerChat
{
    static ServerChat()
    {
        RendererRegistry.Add(new PrimitiveRenderer());
        OutputRegistry.Add(new PrimitiveOutput());

        MessagePlayerJoined.AddOutput(new PlayerJoinHandler());
        MessagePlayerLeft.AddOutput(new PlayerLeftHandler());
    }

    public static ChatRegistry<IChatMessageHandler> HandlerRegistry { get; } = new();
    public static ChatRegistry<IChatMessageRenderer> RendererRegistry { get; } = new();
    public static ChatRegistry<IChatMessageOutput> OutputRegistry { get; } = new();

    public static MiscMessageProvider<PlayerJoinedMessageContext> MessagePlayerJoined { get; }
        = new(new PlayerJoinHandler());

    public static MiscMessageProvider<PlayerLeftMessageContext> MessagePlayerLeft { get; }
        = new(new PlayerLeftHandler());

    public static void HandleMessage(PlayerEntity entity, string text)
    {
        PlayerMessage message = new PlayerMessage(entity, text, DateTimeOffset.UtcNow);

        foreach (var handler in HandlerRegistry._handlers)
        {
            handler.HandleMessage(message);
        }

        if (message.IsCancelled)
        {
            return;
        }

        MessageRenderContext ctx = new MessageRenderContext(entity, message);
        foreach (var renderer in RendererRegistry._handlers)
        {
            renderer.Render(ctx);
        }

        MessageRenderResult result = ctx.Build();
        foreach (var output in OutputRegistry._handlers)
        {
            output.OutputMessage(result);
        }
    }
}
