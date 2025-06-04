
using Amethyst.Server.Entities.Players;
using Terraria;

namespace Amethyst.Network.Handling;

public static partial class PacketSendingUtility
{
    public static void SendFullWorld(PlayerEntity entity, int spawnX = -1, int spawnY = -1)
    {
        if (spawnX != -1 && spawnY != -1)
        {
            LoadSection(entity, spawnX / 200, spawnY / 150, 2, 1);
        }

        LoadSection(entity, Main.spawnTileX / 200, Main.spawnTileY / 150, 2, 1);
        LoadEntities(entity);
    }

    public static void LoadEntities(PlayerEntity entity)
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

    public static void LoadSection(PlayerEntity entity, int sectionX, int sectionY, short sectionsWidth, short sectionsHeight)
    {
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
                if (Main.tile[i, j] == null)
                {
                    continue;
                }

                byte[] buffer = CompressTileBlock(i * 200, j * 150, 200, 150);
                entity.SendPacketBytes(buffer);
                Console.WriteLine($"built: {buffer[2]}, len: {buffer.Length}");
            }
        }
    }
}
