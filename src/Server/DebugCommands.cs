using Amethyst.Server.Entities.Players;
using Amethyst.Systems.Commands.Base;
using Amethyst.Systems.Commands.Dynamic.Attributes;
using Amethyst.Systems.Users.Base;
using Amethyst.Text;
using Terraria;

namespace Amethyst.Server;

public static class DebugCommands
{
    [Command(["debug pinfo"], "Prints player information.")]
    [CommandRepository("debug")]
    [CommandSyntax("en-US", "<player>")]
    public static void PrintPlayerInfo(IAmethystUser user, CommandInvokeContext ctx, PlayerEntity plr)
    {
        ctx.Messages.ReplySuccess($"[AMETHYST] SHOWING PLAYER INFO: {plr.Name} (PlayerEntity.Data.cs)");
        ctx.Messages.ReplyInfo($"hp: {plr.Life}, mp: {plr.Mana}, dead: {plr.IsDead}, gm: {plr.IsGodModeEnabled}");
        ctx.Messages.ReplyInfo($"in_pvp: {plr.IsInPvP}, team: {plr.Team}");
        ctx.Messages.ReplyInfo($"position: {plr.Position.X}/{plr.Position.Y}, velocity: {plr.Velocity.X}/{plr.Velocity.Y}");
        ctx.Messages.ReplyInfo($"difficulty: {plr.Difficulty}, stealth: {plr.Stealth}");
        ctx.Messages.ReplyInfo($"talk_npc: {plr.TalkNPC}");

        ctx.Messages.ReplySuccess($"[TERRARIA] SHOWING PLAYER INFO: {plr.Name} (Player.cs)");
        ctx.Messages.ReplyInfo($"active: {plr.TPlayer.active}, dead: {plr.TPlayer.dead}");
    }

    [Command(["debug npcs"], "Prints NPC information.")]
    [CommandRepository("debug")]
    [CommandSyntax("en-US", "[page]")]
    public static void PrintNPCs(IAmethystUser user, CommandInvokeContext ctx, int page = 0)
    {
        var npcs = Main.npc.Where(n => n != null && n.active);
        var pageCollection = PagesCollection.AsListPage(npcs.Select(p => $"{p.type} ({p.TypeName})"));
        ctx.Messages.ReplyPage(pageCollection, "[AMETHYST] NPCs", null, null, true, page);
    }
}
