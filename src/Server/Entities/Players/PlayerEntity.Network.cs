using Amethyst.Network;
using Amethyst.Server.Entities.Base;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Amethyst.Server.Entities.Players;

public sealed partial class PlayerEntity : IServerEntity
{
    public PlayerNetworkOperations NetworkOperations { get; }

    public void SendText(string text, byte r, byte g, byte b)
        => (NetworkOperations.SendText ?? PlayerNetworkOperations.DefaultSendText).Invoke(this, text, r, g, b);

    public void SendPacketBytes(byte[] data)
        => (NetworkOperations.SendPacketBytes ?? PlayerNetworkOperations.DefaultSendPacketBytes).Invoke(this, data);

    public void SendRectangle(int x, int y, byte width, byte height, TileChangeType changeType = TileChangeType.None)
        => (NetworkOperations.SendTileSquare ?? PlayerNetworkOperations.DefaultSendTileSquare).Invoke(this, x, y, width, height, changeType);

    public void SendMassTiles(in Rectangle rectangle)
        => SendMassTiles(rectangle.X, rectangle.Y, rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);

    public void SendMassTiles(int startX, int startY, int endX, int endY)
    {
        int sx = Math.Min(startX, endX) / 200;
        int sy = Math.Min(startY, endY) / 150;
        int sx2 = Math.Max(startX, endX) / 200 + 1;
        int sy2 = Math.Max(startY, endY) / 150 + 1;

        for (int i = sx; i < sx2; i++)
        {
            for (int j = sy; j < sy2; i++)
            {
                SendSection(i, j);
            }
        }
    }

    public void SendSection(int sectionX, int sectionY) =>
        (NetworkOperations.SendSection ?? PlayerNetworkOperations.DefaultSendSection).Invoke(this, sectionX, sectionY);

    public void RequestSendSection(int sectionX, int sectionY) =>
        (NetworkOperations.RequestSendSection ?? PlayerNetworkOperations.DefaultRequestSendSection).Invoke(this, sectionX, sectionY);

    public sealed class PlayerNetworkOperations
    {
        public static Action<PlayerEntity, string, byte, byte, byte> DefaultSendText { get; set; } = static (player, text, r, g, b) =>
        {
            byte[] packet = new PacketWriter().SetType(82)
                .PackUInt16(1) // text id
                .PackByte(255)
                .PackByte(0)
                .PackString(text)
                .PackByte(r)
                .PackByte(g)
                .PackByte(b)
                .BuildPacket();

            player.SendPacketBytes(packet);
        };
        public Action<PlayerEntity, string, byte, byte, byte>? SendText { get; set; }

        public static Action<PlayerEntity, byte[]> DefaultSendPacketBytes { get; set; } = static (player, bytes) =>
        {
            Netplay.Clients[player.Index].Socket.AsyncSend(bytes, 0, bytes.Length, Netplay.Clients[player.Index].ServerWriteCallBack);
        };
        public Action<PlayerEntity, byte[]>? SendPacketBytes { get; set; }

        public static Action<PlayerEntity, int, int> DefaultRequestSendSection { get; set; } = static (player, sectionX, sectionY) =>
        {
            if (!Netplay.Clients[player.Index].TileSections[sectionX, sectionY])
            {
                NetMessage.SendSection(player.Index, sectionX, sectionY);
            }
        };
        public Action<PlayerEntity, int, int>? RequestSendSection { get; set; }

        public static Action<PlayerEntity, int, int> DefaultSendSection { get; set; } = static (player, sectionX, sectionY) =>
        {
            NetMessage.SendSection(player.Index, sectionX, sectionY);
        };
        public Action<PlayerEntity, int, int>? SendSection { get; set; }

        public static Action<PlayerEntity, int, int, int, int, TileChangeType> DefaultSendTileSquare { get; set; } = static (player, x, y, width, height, changeType) =>
        {
            NetMessage.SendTileSquare(player.Index, x, y, width, height, changeType);
        };
        public Action<PlayerEntity, int, int, int, int, TileChangeType>? SendTileSquare { get; set; }
    }
}
