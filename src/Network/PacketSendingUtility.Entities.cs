
using Amethyst.Network.Packets;
using Amethyst.Network.Structures;
using Amethyst.Server.Entities.Players;
using Terraria;

namespace Amethyst.Network;

public static partial class PacketSendingUtility
{
    public static Action<PlayerEntity, int> SyncNPC { get; set; } = DirectSyncNPC;

    public static void DirectSyncNPC(PlayerEntity entity, int npcId)
    {
        NPC npc = Main.npc[npcId];
        if (npc == null || !npc.active)
        {
            return;
        }

        var bitsByte1 = new NetBitsByte
        {
            [0] = npc.direction > 0,
            [1] = npc.directionY > 0,
            [2] = npc.ai[0] != 0f,
            [3] = npc.ai[1] != 0f,
            [4] = npc.ai[2] != 0f,
            [5] = npc.ai[3] != 0f,
            [6] = npc.spriteDirection > 0,
            [7] = npc.life == npc.lifeMax
        };
        var bitsByte2 = new NetBitsByte
        {
            [0] = npc.statsAreScaledForThisManyPlayers > 1,
            [1] = npc.SpawnedFromStatue,
            [2] = npc.strengthMultiplier != 1f
        };
        float[] ai = new float[NPC.maxAI];
        for (int i = 0; i < NPC.maxAI; i++)
        {
            if (npc.ai[i] != 0f)
            {
                ai[i] = npc.ai[i];
            }
        }
        short netID = (short)npc.netID;
        byte? playerCountForMultiplayerDifficultyOverride = npc.statsAreScaledForThisManyPlayers > 1 ? (byte)npc.statsAreScaledForThisManyPlayers : null;
        float strengthMultiplier = npc.strengthMultiplier;
        int life = npc.life;
        var packet = new NPCUpdate
        {
            NPCIndex = (short)npcId,
            Position = npc.position,
            Velocity = npc.velocity,
            Target = npc.target == 0 ? (ushort)65535 : (ushort)npc.target,
            BitsByte1 = bitsByte1,
            BitsByte2 = bitsByte2,
            AI = ai,
            NetID = netID,
            PlayerCountForMultiplayerDifficultyOverride = playerCountForMultiplayerDifficultyOverride,
            StrengthMultiplier = strengthMultiplier,
            Life = life
        };

        byte[] data = NPCUpdatePacket.Serialize(packet);
        entity.SendPacketBytes(data);
    }

    public static Action<PlayerEntity, int> SyncProjectile { get; set; } = DirectSyncProjectile;
    public static void DirectSyncProjectile(PlayerEntity entity, int projectileId)
    {
        Projectile projectile = Main.projectile[projectileId];
        if (projectile == null || !projectile.active)
        {
            return;
        }

        var packet = new ProjectileUpdate
        {
            ProjectileIdentity = (short)projectileId,
            Position = projectile.position,
            Velocity = projectile.velocity,
            OwnerID = (byte)projectile.owner,
            ProjectileType = (short)projectile.type,
            AI = projectile.ai,
            BannerId = projectile.bannerIdToRespondTo,
            Damage = projectile.damage,
            KnockBack = projectile.knockBack,
            OriginalDamage = projectile.originalDamage,
            ProjectileUUID = projectile.projUUID
        };
        byte[] data = ProjectileUpdatePacket.Serialize(packet);
        entity.SendPacketBytes(data);
    }

    public static Action<PlayerEntity, int> SyncItem { get; set; } = DirectSyncItem;
    public static void DirectSyncItem(PlayerEntity entity, int itemId)
    {
        byte[]? data = CreateSyncItemDefaultPacket(itemId);
        if (data != null)
        {
            entity.SendPacketBytes(data);
        }
    }

    public static byte[]? CreateSyncItemDefaultPacket(int itemId, byte ownIgnore = 0)
    {
        Item item = Main.item[itemId];
        if (item == null || !item.active)
        {
            return null;
        }

        var packet = new ItemUpdateInstanced
        {
            ItemIndex = (short)itemId,
            Position = item.position,
            Velocity = item.velocity,
            ItemStack = (short)item.stack,
            ItemPrefix = item.prefix,
            OwnIgnore = ownIgnore,
            ItemType = (short)item.type
        };
        return ItemUpdateInstancedPacket.Serialize(packet);
    }

    public static byte[]? CreateSyncItemInstancedPacket(int itemId, byte ownIgnore = 0)
    {
        Item item = Main.item[itemId];
        if (item == null || !item.active)
        {
            return null;
        }

        var packet = new ItemUpdateInstanced
        {
            ItemIndex = (short)itemId,
            Position = item.position,
            Velocity = item.velocity,
            ItemStack = (short)item.stack,
            ItemPrefix = item.prefix,
            OwnIgnore = ownIgnore,
            ItemType = (short)item.type
        };
        return ItemUpdateInstancedPacket.Serialize(packet);
    }

    public static byte[]? CreateSyncItemShimmerPacket(int itemId, byte ownIgnore = 0)
    {
        Item item = Main.item[itemId];
        if (item == null || !item.active)
        {
            return null;
        }

        var packet = new ItemUpdateShimmer
        {
            ItemIndex = (short)itemId,
            Position = item.position,
            Velocity = item.velocity,
            ItemStack = (short)item.stack,
            ItemPrefix = item.prefix,
            OwnIgnore = ownIgnore,
            ItemType = (short)item.type,
            ShimmerTime = item.shimmerTime,
            IsShimmered = item.shimmered
        };
        return ItemUpdateShimmerPacket.Serialize(packet);
    }

    public static byte[]? CreateSyncItemNoPickupPacket(int itemId, byte ownIgnore = 0)
    {
        Item item = Main.item[itemId];
        if (item == null || !item.active)
        {
            return null;
        }

        var packet = new ItemUpdateNoPickup
        {
            ItemIndex = (short)itemId,
            Position = item.position,
            Velocity = item.velocity,
            ItemStack = (short)item.stack,
            ItemPrefix = item.prefix,
            OwnIgnore = ownIgnore,
            ItemType = (short)item.type,
            TimeEnemiesNoPickup = (byte)item.timeLeftInWhichTheItemCannotBeTakenByEnemies,
        };
        return ItemUpdateNoPickupPacket.Serialize(packet);
    }
}
