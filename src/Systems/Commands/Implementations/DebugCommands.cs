using Amethyst.Commands.Attributes;
using Amethyst.Players;

namespace Amethyst.Commands.Implementations;

public static class DebugCommands
{
    [ServerCommand(CommandType.Debug, "grantroot", "commands.desc.grantRoot", null)]
    [CommandsSettings(CommandSettings.IngameOnly)]
    public static void GrantRoot(CommandInvokeContext ctx)
    {
        if (ctx.Sender is not NetPlayer plr)
        {
            throw new InvalidCastException();
        }

        plr.IsRootGranted = !plr.IsRootGranted;

        ctx.Sender.ReplyInfo(
            plr.IsRootGranted ? "commands.text.rootPermissionsGranted" : "commands.text.rootPermissionsRemoved");
    }
}
