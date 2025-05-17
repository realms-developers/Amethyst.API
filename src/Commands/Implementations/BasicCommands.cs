using System.Diagnostics;
using Amethyst.Commands.Attributes;
using Amethyst.Core;
using Amethyst.Text;
using Microsoft.Extensions.Configuration;
using Terraria.IO;

namespace Amethyst.Commands.Implementations;

public static class BasicCommands
{
    [ServerCommand(CommandType.Console, "exit", "commands.desc.shutdown", null)]
    [CommandsSyntax("-f(orce)")]
    public static void Exit(CommandInvokeContext _, string args = "") => AmethystKernel.StopServer(args.Contains("-f"));

    [ServerCommand(CommandType.Console, "save", "commands.desc.save", null)]
    public static void Save(CommandInvokeContext ctx)
    {
        Stopwatch sw = new();

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

    public static void Language(CommandInvokeContext ctx)
    {
        string lang = ctx.Name.Substring(5);

        ctx.Sender.Language = lang;

        IConfigurationRoot config = new ConfigurationBuilder()
                .AddIniFile(Path.Combine(Localization.Directory, lang, CommandsManager.LanguageCFG))
                .Build();

        ctx.Sender.ReplySuccess(config["meta:reply"] ?? string.Empty);
    }
}
