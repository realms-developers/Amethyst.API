using Amethyst.Network.Handling.Base;
using Amethyst.Network.Handling.Packets.Handshake;
using Amethyst.Network.Packets;
using Amethyst.Server.Entities.Players;
using Terraria;
using Terraria.Localization;

namespace Amethyst.Network.Handling.Packets.Projectiles;

public sealed class ProjectilesHandler : INetworkHandler
{
    public string Name => "net.amethyst.ProjectilesHandler";

    public void Load()
    {
        NetworkManager.SetMainHandler<ProjectileKill>(OnProjectileKill);
        NetworkManager.SetMainHandler<ProjectileKillPortal>(OnProjectileKillPortal);
        NetworkManager.SetMainHandler<ProjectileUpdate>(OnProjectileUpdate);
    }

    private void OnProjectileUpdate(PlayerEntity plr, ref ProjectileUpdate packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        byte owner = packet.ProjectileType == 949 ? (byte)255 : (byte)plr.Index;
        short identity = packet.ProjectileIdentity;
        int projIndex = Main.projectile.FirstOrDefault(p => p != null && p.active && p.owner == owner && p.identity == identity)?.whoAmI ?? GetNewProjectileIndex();

        Projectile projectile = Main.projectile[projIndex];
        if (!projectile.active || projectile.type != packet.ProjectileType)
        {
            projectile.SetDefaults(packet.ProjectileType);
        }
        projectile.identity = identity;
        projectile.position = packet.Position;
        projectile.velocity = packet.Velocity;
        projectile.type = packet.ProjectileType;
        projectile.damage = packet.Damage;
        projectile.bannerIdToRespondTo = packet.BannerId;
        projectile.originalDamage = packet.OriginalDamage;
        projectile.knockBack = packet.KnockBack;
        projectile.owner = owner;

        for (int num40 = 0; num40 < Projectile.maxAI; num40++)
        {
            projectile.ai[num40] = packet.AI[num40];
        }
        if (packet.ProjectileUUID >= 0)
        {
            projectile.projUUID = packet.ProjectileUUID;
            Main.projectileIdentity[owner, packet.ProjectileUUID] = projIndex;
        }

        projectile.ProjectileFixDesperation();
        if (Main.netMode == 2)
        {
            NetMessage.TrySendData(27, -1, plr.Index, NetworkText.Empty, projIndex);
        }
    }

    private void OnProjectileKillPortal(PlayerEntity plr, ref ProjectileKillPortal packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        for (int i = 0; i < Main.maxProjectiles; i++)
        {
            Projectile projectile = Main.projectile[i];
            if (projectile != null && projectile.active && projectile.type == 602 && projectile.ai[0] == (float)packet.AI1Type && projectile.owner == packet.PlayerIndex)
            {
                projectile.Kill();

                NetMessage.TrySendData(29, -1, plr.Index, NetworkText.Empty, projectile.identity, packet.PlayerIndex);
                return;
            }
        }
    }

    private void OnProjectileKill(PlayerEntity plr, ref ProjectileKill packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        for (int i = 0; i < Main.maxProjectiles; i++)
        {
            Projectile projectile = Main.projectile[i];
            if (projectile != null && projectile.active && projectile.identity == packet.ProjectileIdentity && projectile.owner == plr.Index)
            {
                projectile.Kill();

                NetMessage.TrySendData(29, -1, plr.Index, NetworkText.Empty, packet.ProjectileIdentity, plr.Index);
                return;
            }
        }
    }

    private static int GetNewProjectileIndex()
    {
        for (int i = 0; i < Main.maxProjectiles; i++)
        {
            if (Main.projectile[i] == null || !Main.projectile[i].active)
            {
                return i;
            }
        }

        return Projectile.FindOldestProjectile();
    }

    public void Unload()
    {
        NetworkManager.SetMainHandler<ProjectileKill>(null);
        NetworkManager.SetMainHandler<ProjectileKillPortal>(null);
        NetworkManager.SetMainHandler<ProjectileUpdate>(null);
    }
}
