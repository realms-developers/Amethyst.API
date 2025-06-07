
using Amethyst.Network.Handling.Packets.Handshake;
using Amethyst.Server.Entities;
using Amethyst.Server.Entities.Players;
using Terraria;

namespace Amethyst.Network;

public static partial class PacketSendingUtility
{
    public static void ExcludeBroadcastConnected(int index, byte[] packet)
    {
        foreach (PlayerEntity player in EntityTrackers.Players)
        {
            if (player.Phase != ConnectionPhase.Connected || player.Index == index)
                continue;

            player.SendPacketBytes(packet);
        }
    }
    public static void ExcludeBroadcastAll(int index, byte[] packet)
    {
        foreach (PlayerEntity player in EntityTrackers.Players)
        {
            if (player.Index == index)
                continue;

            player.SendPacketBytes(packet);
        }
    }

    public static Action<PlayerEntity, int, int> SendFullWorld { get; set; } = DirectSendFullWorld;
    public static void DirectSendFullWorld(PlayerEntity entity, int spawnX = -1, int spawnY = -1)
    {
        if (spawnX != -1 && spawnY != -1)
        {
            LoadSection(entity, spawnX / 200, spawnY / 150, 2, 1);
        }

        LoadSection(entity, Main.spawnTileX / 200, Main.spawnTileY / 150, 2, 1);
        LoadEntities(entity);
    }

    public static Action<PlayerEntity> LoadEntities { get; set; } = DirectLoadEntities;
    public static void DirectLoadEntities(PlayerEntity entity)
    {
        for (int i = 0; i < Main.maxNPCs; i++)
        {
            SyncNPC(entity, i);
        }

        for (int i = 0; i < Main.maxProjectiles; i++)
        {
            SyncProjectile(entity, i);
        }

        for (int i = 0; i < Main.maxItems; i++)
        {
            SyncItem(entity, i);
        }
    }

    public static Action<PlayerEntity, int, int, short, short> LoadSection { get; set; } = DirectLoadSection;
    public static void DirectLoadSection(PlayerEntity entity, int sectionX, int sectionY, short sectionsWidth, short sectionsHeight)
    {
        if (sectionsWidth == 1 && sectionsHeight == 1)
        {
            entity.Sections.MarkAsSent(sectionX, sectionY);

            byte[] buffer = CompressTileBlock(sectionX * 200, sectionY * 150, 200, 150);
            entity.SendPacketBytes(buffer);
            return;
        }

        int maxX = Main.maxTilesX / 200;
        int maxY = Main.maxTilesX / 150;

        int startX = Math.Clamp(sectionX - sectionsWidth, 0, maxX - 1);
        int startY = Math.Clamp(sectionY - sectionsHeight, 0, maxY - 1);
        int endX = Math.Clamp(sectionX + sectionsWidth, 0, maxX - 1);
        int endY = Math.Clamp(sectionY + sectionsHeight, 0, maxY - 1);

        for (int i = startX; i <= endX; i++)
        {
            for (int j = startY; j <= endY; j++)
            {
                entity.Sections.MarkAsSent(i, j);

                byte[] buffer = CompressTileBlock(i * 200, j * 150, 200, 150);
                entity.SendPacketBytes(buffer);
            }
        }
    }
}
