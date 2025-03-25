using Amethyst.Core;
using Amethyst.Text;

namespace Amethyst.Commands.Implementations;

public static class BasicCommands
{
    [ServerCommand(CommandType.Shared, "cmds", "$LOCALIZE commands.desc.showCommands", null)]
    [CommandsSyntax("[page]", "[-r(aw)]")]
    public static void Commands(CommandInvokeContext ctx, int page = 0, string args = "")
    {
        Func<CommandRunner, bool> whereExpression = (p) => 
        {
            return  p.IsDisabled == false && 
                    p.Data.Settings.HasFlag(CommandSettings.Hidden) == false &&
                    (p.Data.Permission == null || ctx.Sender.HasPermission(p.Data.Permission)) && 
                    (p.Data.Type == CommandType.Console ? ctx.Sender.Type == SenderType.Console : true) && 
                    (p.Data.Type == CommandType.Debug ? AmethystSession.Profile.DebugMode : true);
        };

        if (args == "-raw" || args == "-r")
        {
            var pages = PagesCollection.CreateFromList(CommandsManager.Commands
                    .Where(whereExpression)
                    .Select(p => $"[c/51db99:/{p.Data.Name}]{(p.Data.Syntax != null ? $" {string.Join(' ', p.Data.Syntax)}" : "")}"));

            ctx.Sender.ReplyPage(pages, "$LOCALIZE commands.text.availableCommands", null, null, false, page);
            return;
        }
        else
        {
            var pages = PagesCollection.SplitByPages(CommandsManager.Commands
                    .Where(whereExpression)
                    .Select(p => $"[c/51db99:/{p.Data.Name}]{(p.Data.Syntax != null ? $" {string.Join(' ', p.Data.Syntax)}" : "")} - {ctx.Sender.Language.LocalizeDirect(p.Data.Description)}"));

            ctx.Sender.ReplyPage(pages, "$LOCALIZE commands.text.availableCommands", null, null, false, page);
            return;
        }
    }

    [ServerCommand(CommandType.Shared, "help", "$LOCALIZE commands.desc.showHelp", null)]
    [CommandsSyntax("[page]")]
    public static void Help(CommandInvokeContext ctx, int page = 0)
    {
        var pages = ctx.Sender.Language.HelpPages;
        ctx.Sender.ReplyPage(pages, "$LOCALIZE commands.text.help", null, null, true, 0);
    }
}