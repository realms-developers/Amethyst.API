using Amethyst.Server.Entities.Players;
using Amethyst.Systems.Chat.Base.Misc.Base;
using Amethyst.Systems.Chat.Base.Misc.Context;

namespace Amethyst.Systems.Chat.Primitive.Misc;

public sealed class PlayerTeamHandler : IMiscMessageRenderer<PlayerTeamMessageContext>, IMiscOutput<PlayerTeamMessageContext>
{
    public string Name => "PlayerJoin_R+O";

    public MiscRenderedMessage<PlayerTeamMessageContext>? Render(PlayerTeamMessageContext ctx)
    {
        string teamName = ctx.TeamID switch
        {
            0 => "white",
            1 => "red",
            2 => "green",
            3 => "blue",
            4 => "yellow",
            5 => "pink",
            _ => "unknown"
        };

        return new MiscRenderedMessage<PlayerTeamMessageContext>($"{ctx.Player.Name} joins {teamName} team!", "FFFFFF", ctx);
    }

    public void OutputMessage(MiscRenderedMessage<PlayerTeamMessageContext> message)
    {
        PlayerUtils.BroadcastText(message.Text, message.Color.R, message.Color.G, message.Color.B);
    }
}
