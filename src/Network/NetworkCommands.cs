using Amethyst.Network.Handling;
using Amethyst.Network.Handling.Base;
using Amethyst.Server.Entities;
using Amethyst.Server.Entities.Players;
using Amethyst.Systems.Commands.Base;
using Amethyst.Systems.Commands.Dynamic.Attributes;
using Amethyst.Systems.Users.Base;
using Amethyst.Text;

namespace Amethyst.Network;

public static class NetworkCommands
{
    [Command(["net uinfo"], "amethyst.desc.networkUserInfo")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.network.userInfo")]
    [CommandSyntax("en-US", "<player>")]
    [CommandSyntax("ru-RU", "<игрок>")]
    public static void NetworkUserInfo(IAmethystUser user, CommandInvokeContext ctx, PlayerEntity player)
    {
        ctx.Messages.ReplySuccess("amethyst.networkManagement.playerInfo", player.Name);
        ctx.Messages.ReplyInfo("amethyst.networkManagement.playerInfoIndex", player.Index);
        ctx.Messages.ReplyInfo("amethyst.networkManagement.playerInfoIP", player.IP);
        ctx.Messages.ReplyInfo("amethyst.networkManagement.playerInfoProtocol", player.Protocol);
        ctx.Messages.ReplyInfo("amethyst.networkManagement.playerInfoPlatform", player.PlatformType.ToString());
        ctx.Messages.ReplyInfo("amethyst.networkManagement.playerInfoPhase", player.Phase.ToString());
    }

    [Command(["net close"], "amethyst.desc.networkClose")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.network.close")]
    [CommandSyntax("en-US", "<player>")]
    [CommandSyntax("ru-RU", "<игрок>")]
    public static void NetworkClose(IAmethystUser user, CommandInvokeContext ctx, PlayerEntity player)
    {
        if (player.Active)
        {
            player.CloseSocket();
            EntityTrackers.Players.Manager!.Remove(player.Index);
            ctx.Messages.ReplySuccess("amethyst.networkManagement.playerClosed", player.Name);
        }
        else
        {
            ctx.Messages.ReplyError("amethyst.networkManagement.playerNotActive", player.Name);
        }
    }

    [Command(["net lock"], "amethyst.desc.networkLock")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.network.lock")]
    public static void NetworkLock(IAmethystUser user, CommandInvokeContext ctx)
    {
        if (NetworkManager.IsLocked)
        {
            ctx.Messages.ReplyError("amethyst.networkManagement.networkAlreadyLocked");
            return;
        }

        NetworkManager.IsLocked = true;
        ctx.Messages.ReplySuccess("amethyst.networkManagement.networkLocked");
    }

    [Command(["net unlock"], "amethyst.desc.networkUnlock")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.network.unlock")]
    public static void NetworkUnlock(IAmethystUser user, CommandInvokeContext ctx)
    {
        if (!NetworkManager.IsLocked)
        {
            ctx.Messages.ReplyError("amethyst.networkManagement.networkAlreadyUnlocked");
            return;
        }

        NetworkManager.IsLocked = false;
        ctx.Messages.ReplySuccess("amethyst.networkManagement.networkUnlocked");
    }

    [Command(["net socketacceptdelay"], "amethyst.desc.networkSocketAcceptDelay")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.network.socketAcceptDelay")]
    [CommandSyntax("en-US", "<delay>")]
    [CommandSyntax("ru-RU", "<задержка>")]
    public static void NetworkSocketAcceptDelay(IAmethystUser user, CommandInvokeContext ctx, int delay)
    {
        if (delay < 0)
        {
            ctx.Messages.ReplyError("amethyst.networkManagement.socketAcceptDelayNegative");
            return;
        }

        NetworkManager.SocketAcceptDelay = delay;
        ctx.Messages.ReplySuccess("amethyst.networkManagement.socketAcceptDelaySet", delay);
    }

    [Command(["nethdlr enable"], "amethyst.desc.networkHandlerEnable")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.network.handlerEnable")]
    [CommandSyntax("en-US", "<handler>")]
    [CommandSyntax("ru-RU", "<обработчик>")]
    public static void NetworkHandlerEnable(IAmethystUser user, CommandInvokeContext ctx, string handlerName)
    {
        if (HandlerManager.LoadHandler(handlerName))
        {
            ctx.Messages.ReplySuccess("amethyst.networkManagement.handlerEnabled", handlerName);
        }
        else
        {
            ctx.Messages.ReplyError("amethyst.networkManagement.handlerNotFound", handlerName);
        }
    }

    [Command(["nethdlr disable"], "amethyst.desc.networkHandlerDisable")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.network.handlerDisable")]
    [CommandSyntax("en-US", "<handler>")]
    [CommandSyntax("ru-RU", "<обработчик>")]
    public static void NetworkHandlerDisable(IAmethystUser user, CommandInvokeContext ctx, string handlerName)
    {
        if (HandlerManager.UnloadHandler(handlerName))
        {
            ctx.Messages.ReplySuccess("amethyst.networkManagement.handlerDisabled", handlerName);
        }
        else
        {
            ctx.Messages.ReplyError("amethyst.networkManagement.handlerNotFound", handlerName);
        }
    }

    [Command(["nethdlr list"], "amethyst.desc.networkHandlerList")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.network.handlerList")]
    [CommandSyntax("en-US", "[page]")]
    [CommandSyntax("ru-RU", "[страница]")]
    public static void NetworkHandlerList(IAmethystUser user, CommandInvokeContext ctx, int page = 0)
    {
        PagesCollection collection = PagesCollection.AsListPage(HandlerManager.GetHandlers().Select(h => h.Name), 80);
        if (collection.Pages.Count == 0)
        {
            ctx.Messages.ReplyError("amethyst.networkManagement.noHandlers");
            return;
        }

        ctx.Messages.ReplyPage(collection, "amethyst.networkManagement.handlersTitle", null, null, false, page);
    }

    [Command(["nethdlr status"], "amethyst.desc.networkHandlerStatus")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.network.handlerStatus")]
    [CommandSyntax("en-US", "<handler>")]
    [CommandSyntax("ru-RU", "<обработчик>")]
    public static void NetworkHandlerStatus(IAmethystUser user, CommandInvokeContext ctx, string handlerName)
    {
        var handler = HandlerManager.GetHandlers().FirstOrDefault(h => h.Name.Equals(handlerName, StringComparison.OrdinalIgnoreCase));
        if (handler == null)
        {
            ctx.Messages.ReplyError("amethyst.networkManagement.handlerNotFound", handlerName);
            return;
        }

        bool isEnabled = HandlerRuler.IsHandlerEnabled(handler.Name);
        ctx.Messages.ReplySuccess("amethyst.networkManagement.handlerStatus", handler.Name,
            isEnabled ? Localization.Get("amethyst.def.enabled", ctx.Messages.Language)
                      : Localization.Get("amethyst.def.disabled", ctx.Messages.Language));
    }

    [Command(["netcfg syncplayers"], "amethyst.desc.networkRulerSyncPlayers")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.network.rulerSyncPlayers")]
    [CommandSyntax("en-US", "<true/false>")]
    public static void NetworkRulerSyncPlayers(IAmethystUser user, CommandInvokeContext ctx, bool sync)
    {
        HandlersConfiguration.Instance.SyncPlayers = sync;
        HandlersConfiguration.Configuration.Save();

        ctx.Messages.ReplySuccess("amethyst.networkManagement.syncPlayersSet", sync ? "amethyst.def.enabled" : "amethyst.def.disabled");
    }

    [Command(["netcfg worldname"], "amethyst.desc.networkRulerWorldName")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.network.rulerWorldName")]
    [CommandSyntax("en-US", "<name>")]
    [CommandSyntax("ru-RU", "<имя>")]
    public static void NetworkRulerWorldName(IAmethystUser user, CommandInvokeContext ctx, string name)
    {
        PacketsNetworkConfiguration.Instance.FakeWorldName = name;
        PacketsNetworkConfiguration.Configuration.Save();

        ctx.Messages.ReplySuccess("amethyst.networkManagement.worldNameSet", name);
    }

    [Command(["netcfg worldid"], "amethyst.desc.networkRulerWorldID")]
    [CommandRepository("root")]
    [CommandPermission("amethyst.network.rulerWorldID")]
    [CommandSyntax("en-US", "<id>")]
    [CommandSyntax("ru-RU", "<идентификатор>")]
    public static void NetworkRulerWorldID(IAmethystUser user, CommandInvokeContext ctx, int id)
    {
        PacketsNetworkConfiguration.Instance.FakeWorldID = id;
        PacketsNetworkConfiguration.Configuration.Save();

        ctx.Messages.ReplySuccess("amethyst.networkManagement.worldIDSet", id);
    }
}
