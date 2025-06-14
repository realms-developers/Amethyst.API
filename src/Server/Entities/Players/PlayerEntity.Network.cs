using Amethyst.Network;
using Amethyst.Network.Structures;
using Amethyst.Network.Utilities;
using Amethyst.Server.Entities.Base;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Amethyst.Server.Entities.Players;

public sealed partial class PlayerEntity : IServerEntity
{
    public void SendText(string text, byte r, byte g, byte b)
    {
        FastPacketWriter writer = new(82, 3 + sizeof(ushort) + 5 + text.Length*2);
        writer.WriteUInt16(1);
        writer.WriteByte(255);
        writer.WriteNetText(new NetText(0, text, null));
        writer.WriteByte(r);
        writer.WriteByte(g);
        writer.WriteByte(b);

        SendPacketBytes(writer.Build());

        writer.Dispose();
    }

    public void SendText(string text, NetColor color) => SendText(text, color.R, color.G, color.B);

    public void SendPacketBytes(byte[] data) => _client.Send(data);

    public void SendPacketBytes(byte[] data, int offset, int count) => _client.Send(data, offset, count);

    public void SendRectangle(int x, int y, byte width, byte height, TileChangeType changeType = TileChangeType.None) =>
        NetMessage.SendTileSquare(Index, x, y, width, height, changeType);

    public void SendMassTiles(int startX, int startY, int endX, int endY)
    {
        int sx = Math.Min(startX, endX) / 200;
        int sy = Math.Min(startY, endY) / 150;
        int sx2 = Math.Max(startX, endX) / 200 + 1;
        int sy2 = Math.Max(startY, endY) / 150 + 1;

        for (int i = sx; i < sx2; i++)
        {
            for (int j = sy; j < sy2; j++)
            {
                SendSection(i, j);
            }
        }
    }

    public void SendSection(int sectionX, int sectionY)
    {
        PacketSendingUtility.LoadSection(this, sectionX, sectionY, 1, 1);
    }

    public void RequestSendSection(int sectionX, int sectionY)
    {
        if (!Sections.IsSent(sectionX, sectionY))
        {
            PacketSendingUtility.LoadSection(this, sectionX, sectionY, 1, 1);
        }
    }
}
