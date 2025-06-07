using Amethyst.Infrastructure.Kernel;
using Amethyst.Kernel;
using Amethyst.Systems.Commands.Base;
using Amethyst.Systems.Commands.Base.Metadata;
using Amethyst.Systems.Commands.Dynamic.Attributes;
using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Common.Permissions;
using Amethyst.Systems.Users.Players;
using Amethyst.Text;
using Terraria.IO;

namespace Amethyst.Systems.Commands;

public static class BasicCommands
{
    [Command(["groot"], "amethyst.desc.grantroot")]
    [CommandRepository("debug")]
    public static void GrantRoot(PlayerUser user, CommandInvokeContext ctx)
    {
        if (user.Permissions.SupportsChildProviders)
        {
            user.Permissions.AddChild(new RootPermissionProvider(user));
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
