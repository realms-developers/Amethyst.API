using Amethyst.Server.Entities.Players;
using Amethyst.Systems.Chat.Misc.Base;
using Amethyst.Systems.Chat.Misc.Context;

namespace Amethyst.Systems.Chat.Primitive.Misc;

public sealed class PlayerPvPHandler : IMiscMessageRenderer<PlayerPvPMessageContext>, IMiscOutput<PlayerPvPMessageContext>
{
    public string Name => "PlayerJoin_R+O";

    public MiscRenderedMessage<PlayerPvPMessageContext>? Render(PlayerPvPMessageContext ctx)
    {
        return new MiscRenderedMessage<PlayerPvPMessageContext>($"{ctx.Player.Name} {(ctx.Value ? "enables" : "disables")} PvP!", "FFFFFF", ctx);
    }

    public void OutputMessage(MiscRenderedMessage<PlayerPvPMessageContext> message)
    {
        PlayerUtils.BroadcastText(message.Text, message.Color.R, message.Color.G, message.Color.B);
    }
}
