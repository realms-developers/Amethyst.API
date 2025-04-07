using Amethyst.Network;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;

namespace Amethyst.Players;

public sealed class LocalPlayerUtils
{
    internal LocalPlayerUtils(NetPlayer plr)
    {
        Player = plr;
    }

    public NetPlayer Player { get; }

    public int TileX => (int)Player.TPlayer.position.X / 16;
    public int TileY => (int)Player.TPlayer.position.Y / 16;

    public float PosX => Player.TPlayer.position.X;
    public float PosY => Player.TPlayer.position.Y;

    public Item HeldItem => Player.TPlayer.inventory[Player.TPlayer.selectedItem];

    public bool InPvP => Player.TPlayer.hostile;

    public bool HasBuff(int buffId) => Player.TPlayer.buffType.Any(p => p == buffId);

    public void SendRectangle(int x, int y, byte size, TileChangeType changeType = TileChangeType.None)
        => NetMessage.SendTileSquare(Player.Index, x, y, size, changeType);

    public void SendRectangle(int x, int y, byte width, byte height, TileChangeType changeType = TileChangeType.None)
        => NetMessage.SendTileSquare(Player.Index, x, y, width, height, changeType);

    public void SendMassTiles(in Rectangle rectangle)
        => SendMassTiles(rectangle.X, rectangle.Y, rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);

    public void SendMassTiles(int startX, int startY, int endX, int endY)
    {
        int sx = Netplay.GetSectionX(Math.Min(startX, endX));
        int sy = Netplay.GetSectionY(Math.Min(startY, endY));
        int sx2 = Netplay.GetSectionX(Math.Max(startX, endX)) + 1;
        int sy2 = Netplay.GetSectionY(Math.Max(startY, endY)) + 1;

        for (int i = sx; i < sx2; i++)
        {
            for (int j = sy; j < sy2; i++)
            {
                SendSection(i, j);
            }
        }
    }

    public void SendSection(int sectionX, int sectionY) => NetMessage.SendSection(Player.Index, sectionX, sectionY);

    public void RequestSendSection(int sectionX, int sectionY)
    {
        if (!Netplay.Clients[Player.Index].TileSections[sectionX, sectionY])
        {
            NetMessage.SendSection(Player.Index, sectionX, sectionY);
        }
    }

    public void SendStatusText(string message, bool padding)
    {
        if (padding)
        {
            message = new string('\n', 10) + message;
        }

        SendStatusText(NetworkText.FromLiteral(message));
    }

    public void SendStatusText(NetworkText text)
        => NetMessage.SendData(9, Player.Index, -1, text);

    public void GiveItem(in NetItem item)
        => GiveItem(item.ID, item.Stack, item.Prefix);

    public void GiveItem(Item item)
        => GiveItem(item.netID, item.stack, item.prefix);

    public void GiveItem(int id, int stack, byte prefix)
    {
        int itemIndex = Item.NewItem(new EntitySource_DebugCommand(), (int)PosX, (int)PosY, 16, 16, id, stack, true, prefix, true);
        Main.item[itemIndex].playerIndexTheItemIsReservedFor = Player.Index;
        NetMessage.SendData(21, Player.Index, -1, NetworkText.Empty, itemIndex, 1);
        NetMessage.SendData(22, Player.Index, -1, NetworkText.Empty, itemIndex);
    }

    public void TeleportTile(int x, int y, byte style = 0)
    {
        // sends section. why not rectangle? well, it breaking beatiful buildings!
        RequestSendSection(Netplay.GetSectionX(x), Netplay.GetSectionY(y));

        Player.TPlayer.position = new Vector2(x * 16, y * 16);
        NetMessage.SendData(65, -1, -1, NetworkText.Empty, 0, Player.Index, x * 16, y * 16, style);
    }

    public void Teleport(float x, float y, byte style = 0)
    {
        RequestSendSection(Netplay.GetSectionX((int)x / 16), Netplay.GetSectionY((int)y / 16));

        Player.TPlayer.position = new Vector2(x, y);
        NetMessage.SendData(65, -1, -1, NetworkText.Empty, 0, Player.Index, x, y, style);
    }

    public void Hurt(int damage, string text, bool pvp = false)
        => Hurt(damage, PlayerDeathReason.ByCustomReason(text), pvp);

    public void Hurt(int damage, PlayerDeathReason? reason = null, bool pvp = false)
    {
        NetMessage.SendPlayerHurt(Player.Index, reason ?? PlayerDeathReason.LegacyDefault(), damage, -1, false, pvp, 0);
    }

    public void Kill(string text, bool pvp = false)
        => Kill(PlayerDeathReason.ByCustomReason(text), pvp);

    public void Kill(PlayerDeathReason? reason = null, bool pvp = false)
    {
        NetMessage.SendPlayerDeath(Player.Index, reason ?? PlayerDeathReason.LegacyDefault(), short.MaxValue, -1, pvp);
    }

    public void AddBuff(int buffId, TimeSpan span)
        => AddBuff(buffId, (int)span.TotalSeconds * 60); // 60 is fps

    public void AddBuff(int buffId, int time) => NetMessage.SendData(55, -1, -1, NetworkText.Empty, Player.Index, buffId, number3: time);
}
