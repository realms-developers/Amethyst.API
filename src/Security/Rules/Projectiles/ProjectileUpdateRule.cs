using Amethyst.Network.Managing;
using Amethyst.Network.Packets;
using Microsoft.Xna.Framework;
using Terraria;

namespace Amethyst.Security.Rules.Projectiles;

public sealed class ProjectileUpdateRule : ISecurityRule
{
    public string Name => "coresec_projUpdate";

    public void Load(NetworkInstance net)
    {
        net.SecureIncoming[27].Add(OnUpdateProjectile);
    }

    private bool OnUpdateProjectile(in IncomingPacket packet)
    {
        BinaryReader reader = packet.GetReader();

        short identity = reader.ReadInt16();
        Vector2 position = reader.ReadVector2();
        Vector2 velocity = reader.ReadVector2();

        if (packet.Player.Jail.IsJailed)
        {
            packet.Player.Utils.RemoveProjectile(identity);
            return true;
        }

        if (position.IsBadVector2() || !position.IsInTerrariaWorld(0))
        {
            AmethystLog.Security.Debug(Name, $"security.badVec2 (position) => {packet.Player.Name} [Projectile Identity: {identity}; Bad: {position.IsBadVector2()}; InWorld: {position.IsInTerrariaWorld(0)}; X: {position.X / 16}; Y: {position.Y / 16}]");
            packet.Player.Kick("security.badVec2");
            return true;
        }

        if (velocity.IsBadVector2() || velocity.X > 5000 || velocity.X < -5000 || velocity.Y > 5000 || velocity.Y < -5000)
        {
            AmethystLog.Security.Debug(Name, $"security.badVec2 (velocity) => {packet.Player.Name} [Projectile Identity: {identity}; Bad: {velocity.IsBadVector2()}; InWorld: {velocity.IsInTerrariaWorld(0)}; X: {velocity.X}; Y: {velocity.Y}]");
            packet.Player.Kick("security.badVec2");
            return true;
        }

        int owner = reader.ReadByte();
        int type = reader.ReadInt16();
        BitsByte flags = reader.ReadByte();
        BitsByte flags2 = (byte)(flags[2] ? reader.ReadByte() : 0);
        float[] ai = NetMessage.buffer[packet.Sender].ReUseTemporaryProjectileAI();
        ai[0] = flags[0] ? reader.ReadSingle() : 0f;
        ai[1] = flags[1] ? reader.ReadSingle() : 0f;
        int bannerIdToRespondTo = flags[3] ? reader.ReadUInt16() : 0;
        int damage = flags[4] ? reader.ReadInt16() : 0;
        float knockBack = flags[5] ? reader.ReadSingle() : 0f;
        int originalDamage = flags[6] ? reader.ReadInt16() : 0;
        int projUuid = flags[7] ? reader.ReadInt16() : -1;
        if (projUuid >= 1000)
        {
            projUuid = -1;
        }
        ai[2] = flags2[0] ? reader.ReadSingle() : 0f;

        if ((SecurityManager.Configuration.ProjectileFixedAI1.TryGetValue((int)ai[0], out float value) && ai[0] != value) ||
            (SecurityManager.Configuration.ProjectileFixedAI2.TryGetValue((int)ai[1], out float value2) && ai[1] != value2) ||
            (SecurityManager.Configuration.ProjectileMinAI1.TryGetValue((int)ai[0], out float value3) && ai[0] < value3) ||
            (SecurityManager.Configuration.ProjectileMaxAI1.TryGetValue((int)ai[0], out float value4) && ai[0] < value4) ||
            (SecurityManager.Configuration.ProjectileMinAI2.TryGetValue((int)ai[1], out float value5) && ai[1] < value5) ||
            (SecurityManager.Configuration.ProjectileMaxAI2.TryGetValue((int)ai[1], out float value6) && ai[1] < value6))
        {
            packet.Player.Utils.RemoveProjectile(identity);
            return true;
        }

        if (packet.Player.IsHeldItemBanned)
        {
            packet.Player.Utils.RemoveProjectile(identity);
            packet.Player.ReplyError("security.itemBanned", packet.Player.Utils.HeldItem.type);
            return true;
        }

        if (SecurityManager.ProjectileBans.Contains(type))
        {
            packet.Player.Utils.RemoveProjectile(identity);
            packet.Player.ReplyError("security.projBanned", type);
            return true;
        }

        owner = packet.Sender;
        Projectile? projectile = Main.projectile.FirstOrDefault(p => p != null && p.owner == owner && p.identity == identity);

        bool isNew = projectile == null || !projectile.active;

        if (isNew && packet.Player._securityThreshold.Fire(7))
        {
            packet.Player.Utils.RemoveProjectile(identity);
            packet.Player.Jail.SetTemp(TimeSpan.FromSeconds(3));
            return true;
        }

        return false;
    }

    public void Unload(NetworkInstance net)
    {
        net.SecureIncoming[7].Remove(OnUpdateProjectile);
    }
}
