using Amethyst.Core;
using Amethyst.Network.Managing;
using Amethyst.Network.Packets;
using Microsoft.Xna.Framework;
using Terraria;

namespace Amethyst.Security.Rules.World;

public sealed class ProjectileUpdateRule : ISecurityRule
{
    public string Name => "coresec_projUpdate";

    public void Load(NetworkInstance net)
    {
        net.SecureIncoming[27].Add(OnUpdateProjectile);
    }

    private bool OnUpdateProjectile(in IncomingPacket packet)
    {
        var reader = packet.GetReader();

        int identity = reader.ReadInt16();
        Vector2 position = reader.ReadVector2();
        Vector2 velocity = reader.ReadVector2();

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

        return false;
    }

    public void Unload(NetworkInstance net)
    {
        net.SecureIncoming[7].Remove(OnUpdateProjectile);
    }
}
