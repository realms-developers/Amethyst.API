using Amethyst.Core;
using Amethyst.Text;

namespace Amethyst.Commands.Implementations;

public static class BasicCommands
{
    [ServerCommand(CommandType.Shared, "help", "commands.desc.showCommands", null)]
    [CommandsSyntax("[page]", "[-r(aw)]")]
    public static void Commands(CommandInvokeContext ctx, int page = 0, string args = "")
    {
        Func<CommandRunner, bool> whereExpression = (p) =>
        {
            return p.IsDisabled == false &&
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

            ctx.Sender.ReplyPage(pages, Localization.Get("commands.text.availableCommands", ctx.Sender.Language), null, null, false, page);
        }
        else
        {
            var pages = PagesCollection.SplitByPages(CommandsManager.Commands
                    .Where(whereExpression)
                    .Select(p => $"[c/51db99:/{p.Data.Name}]{(p.Data.Syntax != null ? $" {string.Join(' ', p.Data.Syntax)}" : "")} - {Localization.Get(p.Data.Description, ctx.Sender.Language)}"));

            ctx.Sender.ReplyPage(pages, Localization.Get("commands.text.availableCommands", ctx.Sender.Language), null, null, false, page);
        }
    }

    [ServerCommand(CommandType.Shared, "lang ru", "установить русский язык.", null)]
    public static void LangRU(CommandInvokeContext ctx)
    {
        ctx.Sender.Language = "ru-RU";
        ctx.Sender.ReplySuccess("Язык успешно изменен!");
    }

    [ServerCommand(CommandType.Shared, "lang en", "set english language.", null)]
    public static void LangEN(CommandInvokeContext ctx)
    {
        ctx.Sender.Language = "en-US";
        ctx.Sender.ReplySuccess("Language was successfully changed!");
    }

    /*
    [ServerCommand(CommandType.Shared, "help", "commands.desc.showHelp", null)]
    [CommandsSyntax("[page]")]
    public static void Help(CommandInvokeContext ctx, int page = 0)
    {
        var pages = ctx.Sender.Language.HelpPages;
        ctx.Sender.ReplyPage(pages, Localization.Get("commands.text.help", ctx.Sender.Language), null, null, true, 0);
    }
    */
}
