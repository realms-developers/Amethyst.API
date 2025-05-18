using System.Globalization;
using Amethyst.Systems.Commands.Attributes;
using Amethyst.Text;
using static Amethyst.Security.SecurityManager;

namespace Amethyst.Systems.Commands.Implementations;

public static class BanCommands
{
    private const string _itemPermission = "bans.itemban";
    private const string _projectilePermission = "bans.projectileban";
    private const string _tilePermission = "bans.tileban";
    private const string _wallPermission = "bans.wallban";

    #region ProjectileBans
    [ServerCommand(CommandType.Shared, "projectileban list", "bans.projectileban.list.desc", _projectilePermission)]
    [CommandsSyntax("[page]")]
    public static void ListProjectileBan(CommandInvokeContext ctx, int page = 0)
    {
        IEnumerable<int> bans = ProjectileBans.GetEnumerable();

        if (!bans.Any())
        {
            ctx.Sender.ReplyError("bans.projectileban.list.empty");
            return;
        }

        PagesCollection pages = PagesCollection.CreateFromList(bans.Select(b => b.ToString(CultureInfo.CurrentCulture)), 10);
        ctx.Sender.ReplyPage(pages, "bans.projectileban.list.header", null, null, false, page);
    }

    [ServerCommand(CommandType.Shared, "projectileban add", "bans.projectileban.add.desc", _projectilePermission)]
    [CommandsSyntax("<projectile_id>")]
    public static void AddProjectileBan(CommandInvokeContext ctx, ushort id)
    {
        ProjectileBans.Add(id);
        ctx.Sender.ReplySuccess("bans.projectileban.add.success", id);
    }

    [ServerCommand(CommandType.Shared, "projectileban rm", "bans.projectileban.rm.desc", _projectilePermission)]
    [CommandsSyntax("<projectile_id>")]
    public static void RemoveProjectileBan(CommandInvokeContext ctx, ushort id)
    {
        ProjectileBans.Remove(id);
        ctx.Sender.ReplySuccess("bans.projectileban.rm.success", id);
    }
    #endregion

    #region TileBans
    [ServerCommand(CommandType.Shared, "tileban list", "bans.tileban.list.desc", _tilePermission)]
    [CommandsSyntax("[page]")]
    public static void ListTileBan(CommandInvokeContext ctx, int page = 0)
    {
        IEnumerable<int> bans = TileBans.GetEnumerable();

        if (!bans.Any())
        {
            ctx.Sender.ReplyError("bans.tileban.list.empty");
            return;
        }

        PagesCollection pages = PagesCollection.CreateFromList(bans.Select(b => b.ToString(CultureInfo.CurrentCulture)), 10);
        ctx.Sender.ReplyPage(pages, "bans.tileban.list.header", null, null, false, page);
    }

    [ServerCommand(CommandType.Shared, "tileban add", "bans.tileban.add.desc", _tilePermission)]
    [CommandsSyntax("<tile_id>")]
    public static void AddTileBan(CommandInvokeContext ctx, ushort id)
    {
        TileBans.Add(id);
        ctx.Sender.ReplySuccess("bans.tileban.add.success", id);
    }

    [ServerCommand(CommandType.Shared, "tileban rm", "bans.tileban.rm.desc", _tilePermission)]
    [CommandsSyntax("<tile_id>")]
    public static void RemoveTileBan(CommandInvokeContext ctx, ushort id)
    {
        TileBans.Remove(id);
        ctx.Sender.ReplySuccess("bans.tileban.rm.success", id);
    }
    #endregion

    #region WallBans
    [ServerCommand(CommandType.Shared, "wallban list", "bans.wallban.list.desc", _wallPermission)]
    [CommandsSyntax("[page]")]
    public static void ListWallBan(CommandInvokeContext ctx, int page = 0)
    {
        IEnumerable<int> bans = WallBans.GetEnumerable();

        if (!bans.Any())
        {
            ctx.Sender.ReplyError("bans.wallban.list.empty");
            return;
        }

        PagesCollection pages = PagesCollection.CreateFromList(bans.Select(b => b.ToString(CultureInfo.CurrentCulture)), 10);
        ctx.Sender.ReplyPage(pages, "bans.wallban.list.header", null, null, false, page);
    }

    [ServerCommand(CommandType.Shared, "wallban add", "bans.wallban.add.desc", _wallPermission)]
    [CommandsSyntax("<wall_id>")]
    public static void AddWallBan(CommandInvokeContext ctx, ushort id)
    {
        WallBans.Add(id);
        ctx.Sender.ReplySuccess("bans.wallban.add.success", id);
    }

    [ServerCommand(CommandType.Shared, "wallban rm", "bans.wallban.rm.desc", _wallPermission)]
    [CommandsSyntax("<wall_id>")]
    public static void RemoveWallBan(CommandInvokeContext ctx, ushort id)
    {
        WallBans.Remove(id);
        ctx.Sender.ReplySuccess("bans.wallban.rm.success", id);
    }
    #endregion

    #region ItemBans
    [ServerCommand(CommandType.Shared, "itemban list", "bans.itemban.list.desc", _itemPermission)]
    [CommandsSyntax("[page]")]
    public static void ListItemBan(CommandInvokeContext ctx, int page = 0)
    {
        IEnumerable<int> bans = ItemBans.GetEnumerable();

        if (!bans.Any())
        {
            ctx.Sender.ReplyError("bans.itemban.list.empty");

            return;
        }

        PagesCollection pages = PagesCollection.CreateFromList(bans.Select(b => b.ToString(CultureInfo.CurrentCulture)), 10);

        ctx.Sender.ReplyPage(pages, "bans.itemban.list.header", null, null, false, page);
    }

    [ServerCommand(CommandType.Shared, "itemban add", "bans.itemban.add.desc", _itemPermission)]
    [CommandsSyntax("<item_id>")]
    public static void AddItemBan(CommandInvokeContext ctx, ushort id)
    {
        ItemBans.Add(id);

        ctx.Sender.ReplySuccess("bans.itemban.add.success", id);
    }

    [ServerCommand(CommandType.Shared, "itemban rm", "bans.itemban.rm.desc", _itemPermission)]
    [CommandsSyntax("<item_id>")]
    public static void RemoveItemBan(CommandInvokeContext ctx, ushort id)
    {
        ItemBans.Remove(id);

        ctx.Sender.ReplySuccess("bans.itemban.rm.success", id);
    }
    #endregion
}
