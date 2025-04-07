using System.Diagnostics;
using Amethyst.Commands.Attributes;
using Amethyst.Core;
using Amethyst.Text;
using Terraria.IO;

namespace Amethyst.Commands.Implementations;

public static class BasicCommands
{

    [ServerCommand(CommandType.Console, "exit", "commands.desc.shutdown", null)]
    public static void Exit(CommandInvokeContext _) => AmethystKernel.StopServer();


    [ServerCommand(CommandType.Console, "save", "commands.desc.save", null)]
    public static void Save(CommandInvokeContext ctx)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        WorldFile.SaveWorld();

        sw.Stop();

        ctx.Sender.ReplySuccess($"/save => {sw.Elapsed.TotalSeconds}s");
    }

    [ServerCommand(CommandType.Shared, "help", "commands.desc.showCommands", null)]
    [CommandsSyntax("[page]")]
    public static void Help(CommandInvokeContext ctx, int page = 0)
    {
        bool whereExpression(CommandRunner p)
        {
            return !p.IsDisabled &&
                    !p.Data.Settings.HasFlag(CommandSettings.Hidden) &&
                    (p.Data.Permission == null || ctx.Sender.HasPermission(p.Data.Permission)) &&
                    (p.Data.Type != CommandType.Console || ctx.Sender.Type == SenderType.Console) &&
                    (p.Data.Type != CommandType.Debug || AmethystSession.Profile.DebugMode);
        }

        var pages = PagesCollection.SplitByPages(CommandsManager.Commands
                .Where(whereExpression)
                .Select(p =>
                $"[c/51db99:/{p.Data.Name}]{(p.Data.Syntax != null ? $" {string.Join(' ', p.Data.Syntax)}" : "")} - {Localization.Get(p.Data.Description, ctx.Sender.Language)}"));

        ctx.Sender.ReplyPage(pages, "commands.text.availableCommands", null, null, false, page);
    }

    [ServerCommand(CommandType.Shared, "cmds", "commands.desc.showCommands", null)]
    [CommandsSyntax("[page]")]
    public static void Commands(CommandInvokeContext ctx, int page = 0)
    {
        bool whereExpression(CommandRunner p)
        {
            return !p.IsDisabled &&
                    !p.Data.Settings.HasFlag(CommandSettings.Hidden) &&
                    (p.Data.Permission == null || ctx.Sender.HasPermission(p.Data.Permission)) &&
                    (p.Data.Type != CommandType.Console || ctx.Sender.Type == SenderType.Console) &&
                    (p.Data.Type != CommandType.Debug || AmethystSession.Profile.DebugMode);
        }

        var pages = PagesCollection.CreateFromList(CommandsManager.Commands
                .Where(whereExpression)
                .Select(p => $"[c/51db99:/{p.Data.Name}]{(p.Data.Syntax != null ? $" {string.Join(' ', p.Data.Syntax)}" : "")}"), 120, 5);
        //.Select(p => $"[c/51db99:/{p.Data.Name}]{(p.Data.Syntax != null ? $" {string.Join(' ', p.Data.Syntax)}" : "")}"));

        ctx.Sender.ReplyPage(pages, "commands.text.availableCommands", null, null, false, page);
    }

    #region Language

    [ServerCommand(CommandType.Shared, "lang ru", "установить русский язык.", null)]
    public static void Russian(CommandInvokeContext ctx)
    {
        ctx.Sender.Language = "ru-RU";
        ctx.Sender.ReplySuccess("Язык успешно изменен!");
    }

    [ServerCommand(CommandType.Shared, "lang en", "Set english language.", null)]
    public static void English(CommandInvokeContext ctx)
    {
        ctx.Sender.Language = "en-US";
        ctx.Sender.ReplySuccess("Language was successfully changed!");
    }

    /*
    [ServerCommand(CommandType.Shared, "lang", "commands.desc.lang", null)]
    [CommandsSyntax("<culture>")]
    public static void Language(CommandInvokeContext ctx, string culture)
    {
        culture = culture.Trim();

        // First check if the exact culture is loaded (case-insensitive)
        string? exactMatch = Localization.LoadedCultures
            .FirstOrDefault(c => string.Equals(c, culture, StringComparison.OrdinalIgnoreCase));

        if (exactMatch != null)
        {
            ctx.Sender.Language = exactMatch;
            ctx.Sender.ReplySuccess("commands.lang.success");
            return;
        }

        // If no exact match, try matching the language part (e.g. "en" matches "en-US")
        string? languageMatch = Localization.LoadedCultures
            .FirstOrDefault(c => c.IndexOf('-') > 0 &&
                               c.StartsWith(culture + "-", StringComparison.OrdinalIgnoreCase));

        if (languageMatch != null)
        {
            ctx.Sender.Language = languageMatch;
            ctx.Sender.ReplySuccess("commands.lang.success");
            return;
        }

        // If still no match, try matching just the first part of loaded cultures
        // (e.g. "es" matches "es-ES" even if the input was "es-MX")
        string? fallbackMatch = Localization.LoadedCultures
            .FirstOrDefault(c => c.IndexOf('-') > 0 &&
                               c.Split('-')[0].Equals(culture, StringComparison.OrdinalIgnoreCase));

        if (fallbackMatch != null)
        {
            ctx.Sender.Language = fallbackMatch;
            ctx.Sender.ReplySuccess("commands.lang.success");
            return;
        }

        // No matches found
        ctx.Sender.ReplyError("commands.lang.invalid_culture");

        Languages(ctx);
    }

    [ServerCommand(CommandType.Shared, "langs", "commands.desc.langs", null)]
    public static void Languages(CommandInvokeContext ctx)
    {
        IReadOnlyCollection<string> cultures = Localization.LoadedCultures;

        ctx.Sender.ReplySuccess("commands.langs");
        ctx.Sender.ReplyInfo(string.Join(", ", cultures));
    }
    */

    #endregion
}
