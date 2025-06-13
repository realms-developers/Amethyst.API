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
        MessagePlayerTeam.AddOutput(new PlayerTeamHandler());
        MessagePlayerPvP.AddOutput(new PlayerPvPHandler());
    }

    public static ChatRegistry<IChatMessageHandler> HandlerRegistry { get; } = new();
    public static ChatRegistry<IChatMessageRenderer> RendererRegistry { get; } = new();
    public static ChatRegistry<IChatMessageOutput> OutputRegistry { get; } = new();

    public static MiscMessageProvider<PlayerJoinedMessageContext> MessagePlayerJoined { get; }
        = new(new PlayerJoinHandler());

    public static MiscMessageProvider<PlayerLeftMessageContext> MessagePlayerLeft { get; }
        = new(new PlayerLeftHandler());

    public static MiscMessageProvider<PlayerTeamMessageContext> MessagePlayerTeam { get; }
        = new(new PlayerTeamHandler());

    public static MiscMessageProvider<PlayerPvPMessageContext> MessagePlayerPvP { get; }
        = new(new PlayerPvPHandler());

    public static void HandleMessage(PlayerEntity entity, string text)
    {
        PlayerMessage message = new(entity, text, DateTimeOffset.UtcNow);

        foreach (IChatMessageHandler handler in HandlerRegistry._handlers)
        {
            handler.HandleMessage(message);
        }

        if (message.IsCancelled)
        {
            return;
        }

        MessageRenderContext ctx = new(entity, message);
        foreach (IChatMessageRenderer renderer in RendererRegistry._handlers)
        {
            renderer.Render(ctx);
        }

        MessageRenderResult result = ctx.Build();
        foreach (IChatMessageOutput output in OutputRegistry._handlers)
        {
            output.OutputMessage(result);
        }
    }
}
