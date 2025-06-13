using Amethyst.Server.Entities.Players;
using Amethyst.Systems.Chat.Misc.Base;
using Amethyst.Systems.Chat.Misc.Context;
using Amethyst.Text;

namespace Amethyst.Systems.Chat.Primitive.Misc;

public sealed class PlayerLeftHandler : IMiscMessageRenderer<PlayerLeftMessageContext>, IMiscOutput<PlayerLeftMessageContext>
{
    public string Name => "PlayerLeft_R+O";

    public MiscRenderedMessage<PlayerLeftMessageContext>? Render(PlayerLeftMessageContext ctx)
    {
        return new MiscRenderedMessage<PlayerLeftMessageContext>($"[c/0f5727:>>>] {ctx.Player.Name} has left.", "158f3e", ctx);
    }

    public void OutputMessage(MiscRenderedMessage<PlayerLeftMessageContext> message)
    {
        PlayerUtils.BroadcastText(message.Text, message.Color.R, message.Color.G, message.Color.B);
        AmethystLog.System.Info("Chat", message.Text.RemoveColorTags());
    }
}
