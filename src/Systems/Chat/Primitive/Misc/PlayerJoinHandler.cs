using Amethyst.Server.Entities.Players;
using Amethyst.Systems.Chat.Misc.Base;
using Amethyst.Systems.Chat.Misc.Context;
using Amethyst.Text;

namespace Amethyst.Systems.Chat.Primitive.Misc;

public sealed class PlayerJoinHandler : IMiscMessageRenderer<PlayerJoinedMessageContext>, IMiscOutput<PlayerJoinedMessageContext>
{
    public string Name => "PlayerJoin_R+O";

    public MiscRenderedMessage<PlayerJoinedMessageContext>? Render(PlayerJoinedMessageContext ctx)
    {
        return new MiscRenderedMessage<PlayerJoinedMessageContext>($"[c/0f5727:>>>] {ctx.Player.Name} has joined.", "158f3e", ctx);
    }

    public void OutputMessage(MiscRenderedMessage<PlayerJoinedMessageContext> message)
    {
        PlayerUtils.BroadcastText(message.Text, message.Color.R, message.Color.G, message.Color.B);
        AmethystLog.System.Info("Chat", message.Text.RemoveColorTags());
    }
}
