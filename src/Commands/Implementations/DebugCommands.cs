using Amethyst.Commands.Attributes;
using Amethyst.Players;

namespace Amethyst.Commands.Implementations;

public static class DebugCommands
{
    [ServerCommand(CommandType.Debug, "grantroot", "commands.desc.grantRoot", null)]
    [CommandsSettings(CommandSettings.IngameOnly)]
    public static void GrantRoot(CommandInvokeContext ctx)
    {
        NetPlayer plr = (ctx.Sender as NetPlayer)!;

        plr.IsRootGranted = !plr.IsRootGranted;

        ctx.Sender.ReplyInfo(plr.IsRootGranted ? Localization.Get("commands.text.rootPermissionsGranted", ctx.Sender.Language) :
            Localization.Get("commands.text.rootPermissionsRemoved", ctx.Sender.Language));
    }
}
