using Amethyst.Kernel;
using Amethyst.Server.Entities.Players;
using Amethyst.Systems.Commands;
using Amethyst.Systems.Commands.Base;
using Amethyst.Systems.Commands.Base.Metadata;
using Amethyst.Systems.Commands.Dynamic.Attributes;
using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Common.Permissions;
using Amethyst.Systems.Users.Players;
using Amethyst.Text;
using Terraria.IO;

namespace Amethyst.Systems;

public static class BasicCommands
{
    [Command(["??"], "amethyst.desc.seePage")]
    [CommandSyntax("en-US", "[page]")]
    [CommandSyntax("ru-RU", "[страница]")]
    public static void SeePage(IAmethystUser user, CommandInvokeContext ctx, int page = 0)
    {
        if (user.Commands.ActivePage == null || user.Commands.ActivePage.Pages.Count == 0)
        {
            ctx.Messages.ReplyError("amethyst.basic.noActivePage");
            return;
        }

        ctx.Messages.ReplyPage(user.Commands.ActivePage, "amethyst.activePage", null, null, false, page);
    }

    [Command(["=", "!!"], "amethyst.desc.repeatLastCommand")]
    [CommandSyntax("en-US", "[additional arguments]")]
    [CommandSyntax("ru-RU", "[дополнительные аргументы]")]
    public static void RepeatLastCommand(IAmethystUser user, CommandInvokeContext ctx)
    {
        CompletedCommandInfo? lastcmd = user.Commands.History.FirstOrDefault(p => !p.Command.Metadata.Names.Any(p => p == "!!" || p == "="));
        if (lastcmd == null)
        {
            ctx.Messages.ReplyError("amethyst.basic.noLastCommand");
            return;
        }

        string additionalArgs = string.Empty;
        if (ctx.Args.Length > 0)
        {
            additionalArgs = " " + string.Join(' ', ctx.Args);
        }

        user.Commands.RunCommand(lastcmd.Command, lastcmd.CommandArgs + additionalArgs);
    }

    [Command(["help"], "commands.desc.showHelp")]
    [CommandSyntax("en-US", "[page]")]
    [CommandSyntax("ru-RU", "[страница]")]
    public static void Help(IAmethystUser user, CommandInvokeContext ctx, int page = 0)
    {
        bool WhereExpression(ICommand p)
        {
            return p.PreferredUser.IsInstanceOfType(user) &&
                   p.Metadata.Names.Length > 0 &&
                   !p.Metadata.Rules.HasFlag(CommandRules.Disabled) &&
                   !p.Metadata.Rules.HasFlag(CommandRules.Hidden) &&
                   (p.Metadata.Permission == null || user.Permissions.HasPermission(p.Metadata.Permission) == Users.Base.Permissions.PermissionAccess.HasPermission);
        }

        var pages = PagesCollection.AsPage(CommandsOrganizer.Repositories.Where(p => user.Commands.Repositories.Contains(p.Name))
                .SelectMany(repo => repo.RegisteredCommands)
                .Where(WhereExpression)
                .Select(p =>
                $"[c/51db99:/{p.Metadata.Names.First()}]{(p.Metadata.Syntax?[ctx.Messages.Language] != null ? $" {string.Join(' ', p.Metadata.Syntax[ctx.Messages.Language]!)}" : "")} - {Localization.Get(p.Metadata.Description, user.Messages.Language)}"));

        ctx.Messages.ReplyPage(pages, "amethyst.basic.availableCommands", null, null, false, page);
    }

    [Command(["cmds"], "commands.desc.showCommands")]
    [CommandSyntax("en-US", "[page]")]
    [CommandSyntax("ru-RU", "[страница]")]
    public static void Commands(IAmethystUser user, CommandInvokeContext ctx, int page = 0)
    {
        bool WhereExpression(ICommand p)
        {
            return p.PreferredUser.IsInstanceOfType(user) &&
                   p.Metadata.Names.Length > 0 &&
                   !p.Metadata.Rules.HasFlag(CommandRules.Disabled) &&
                   !p.Metadata.Rules.HasFlag(CommandRules.Hidden) &&
                   (p.Metadata.Permission == null || user.Permissions.HasPermission(p.Metadata.Permission) == Users.Base.Permissions.PermissionAccess.HasPermission);
        }

        var pages = PagesCollection.AsPage(CommandsOrganizer.Repositories.Where(p => user.Commands.Repositories.Contains(p.Name))
                .SelectMany(repo => repo.RegisteredCommands)
                .Where(WhereExpression)
                .Select(p =>
                $"[c/51db99:/{p.Metadata.Names.First()}]{(p.Metadata.Syntax?[ctx.Messages.Language] != null ? $" {string.Join(' ', p.Metadata.Syntax[ctx.Messages.Language]!)}" : "")}"), 5);
        //.Select(p => $"[c/51db99:/{p.Data.Name}]{(p.Data.Syntax != null ? $" {string.Join(' ', p.Data.Syntax)}" : "")}"));

        ctx.Messages.ReplyPage(pages, "amethyst.basic.availableCommands", null, null, false, page);
    }

    [Command(["giveroot"], "amethyst.desc.giveRoot")]
    [CommandRepository("root")]
    [CommandSyntax("en-US", "<player>")]
    [CommandSyntax("ru-RU", "<игрок>")]
    public static void GiveRoot(IAmethystUser user, CommandInvokeContext ctx, PlayerEntity player)
    {
        PlayerUser? plrUser = player.User;
        if (plrUser == null)
        {
            ctx.Messages.ReplyError("amethyst.basic.playerUserNotFound", player.Name);
            AmethystLog.System.Error($"Commands(/giveroot)", $"Player {player.Name} does not have a user associated with it.");
            return;
        }

        if (plrUser.Permissions.SupportsChildProviders)
        {
            plrUser.Permissions.AddChild(new RootPermissionProvider(plrUser));
            if (!plrUser.Commands.Repositories.Contains("root"))
            {
                plrUser.Commands.Repositories.Add("root");
            }

            ctx.Messages.ReplySuccess("amethyst.basic.givenRoot", player.Name);
            AmethystLog.System.Info($"Commands(/giveroot)", $"Granted root permission to user {plrUser.Name}.");
        }
        else
        {
            ctx.Messages.ReplyError("amethyst.basic.noChildPermissionProviders");
            AmethystLog.System.Error($"Commands(/giveroot)", $"Player {player.Name} does not support child permission providers.");
        }
    }

    [Command(["groot"], "amethyst.desc.grantroot")]
    [CommandRepository("debug")]
    public static void GrantRoot(PlayerUser user, CommandInvokeContext ctx)
    {
        if (user.Permissions.SupportsChildProviders)
        {
            user.Permissions.AddChild(new RootPermissionProvider(user));
            if (!user.Commands.Repositories.Contains("root"))
            {
                user.Commands.Repositories.Add("root");
            }

            ctx.Messages.ReplySuccess("amethyst.basic.grantedRoot");
            AmethystLog.System.Info($"Commands(/+root)", $"Granted root permission to user {user.Name}.");
        }
        else
        {
            ctx.Messages.ReplyError("amethyst.basic.noChildPermissionProviders");
            AmethystLog.System.Error($"Commands(/+root)", $"User {user.Name} does not support child permission providers.");
        }
    }

    [Command(["rroot"], "amethyst.desc.revokeRoot")]
    [CommandRepository("debug")]
    public static void RevokeRoot(PlayerUser user, CommandInvokeContext ctx)
    {
        if (user.Permissions.SupportsChildProviders)
        {
            user.Commands.Repositories.Remove("root");
            user.Permissions.RemoveChild<RootPermissionProvider>();

            ctx.Messages.ReplySuccess("amethyst.basic.revokedRoot");
            AmethystLog.System.Info($"Commands(/-root)", $"Revoked root permission from user {user.Name}.");
        }
        else
        {
            ctx.Messages.ReplyError("amethyst.basic.noChildPermissionProviders");
            AmethystLog.System.Error($"Commands(/-root)", $"User {user.Name} does not support child permission providers.");
        }
    }

    [Command(["exit"], "commands.desc.shutdown")]
    [CommandSyntax("en-US", "-f(orce)")]
    [CommandRepository("root")]
    public static void Exit(IAmethystUser user, CommandInvokeContext ctx, string? args = null)
        => AmethystSession.Launcher.StopServer(args?.Contains("-f") == true || args?.Contains("-force") == true);

    [Command(["save"], "commands.desc.save")]
    [CommandRepository("root")]
    public static void Save(IAmethystUser user, CommandInvokeContext ctx)
        => WorldFile.SaveWorld();

    [Command(["lang ru"], "Установить русский язык.")]
    public static void Russian(IAmethystUser user, CommandInvokeContext ctx)
    {
        user.Messages.Language = "ru-RU";
        ctx.Messages.ReplySuccess("Язык успешно изменен!");
    }

    [Command(["lang en"], "Set english language.")]
    public static void English(IAmethystUser user, CommandInvokeContext ctx)
    {
        user.Messages.Language = "en-US";
        ctx.Messages.ReplySuccess("Language was successfully changed!");
    }
}
