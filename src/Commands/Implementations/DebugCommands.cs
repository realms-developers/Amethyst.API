using Amethyst.Players;

namespace Amethyst.Commands.Implementations;

public static class DebugCommands
{
    [ServerCommand(CommandType.Debug, "grantroot", "$LOCALIZE commands.desc.grantRoot", null)]
    [CommandsSettings(CommandSettings.IngameOnly)]
    public static void GrantRoot(CommandInvokeContext ctx)
    {
        var plr = (ctx.Sender as NetPlayer)!;
        
        plr.IsRootGranted = !plr.IsRootGranted;
        ctx.Sender.ReplyInfo(plr.IsRootGranted ? "$LOCALIZE commands.text.rootPermissionsGranted" : "$LOCALIZE commands.text.rootPermissionsRemoved");
    }
}