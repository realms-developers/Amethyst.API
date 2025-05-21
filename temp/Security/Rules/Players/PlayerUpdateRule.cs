using Amethyst.Network.Managing;
using Amethyst.Network.Packets;
using Microsoft.Xna.Framework;
using Terraria;

namespace Amethyst.Security.Rules.Players;

public sealed class PlayerUpdateRule : ISecurityRule
{
    public string Name => "coresec_playerUpdate";

    public void Load(NetworkInstance net)
    {
        net.SecureIncoming[13].Add(OnPlayerUpdate);
    }

    private bool OnPlayerUpdate(in IncomingPacket packet)
    {
        BinaryReader reader = packet.GetReader();

        reader.ReadByte();

        BitsByte bb1 = reader.ReadByte();
        BitsByte bb2 = reader.ReadByte();
        BitsByte bb3 = reader.ReadByte();
        BitsByte bb4 = reader.ReadByte();

        reader.ReadByte();
        Vector2 position = reader.ReadVector2();

        if (packet.Player.Jail.IsJailed)
        {
            packet.Player.Utils.Teleport(packet.Player._lastPos.X, packet.Player._lastPos.Y);
            return true;
        }

        if (position.IsBadVector2() || !position.IsInTerrariaWorld(0))
        {
            AmethystLog.Security.Debug(Name, $"security.badVec2 (position) => {packet.Player.Name} [Bad: {position.IsBadVector2()}; InWorld: {position.IsInTerrariaWorld(0)}; X: {position.X / 16}; Y: {position.Y / 16}]");
            packet.Player.Kick("security.badVec2");
            return true;
        }

        packet.Player._lastPos = position;

        if (bb2[2])
        {
            Vector2 velocity = reader.ReadVector2();

            if (velocity.IsBadVector2() || velocity.X > 5000 || velocity.X < -5000 || velocity.Y > 5000 || velocity.Y < -5000)
            {
                AmethystLog.Security.Debug(Name, $"security.badVec2 (velocity) => {packet.Player.Name} [Bad: {velocity.IsBadVector2()}; InWorld: {velocity.IsInTerrariaWorld(0)}; X: {velocity.X}; Y: {velocity.Y}]");
                packet.Player.Kick("security.badVec2");
                return true;
            }
        }
        if (bb3[6])
        {
            Vector2 returnPotion = reader.ReadVector2();

            if (returnPotion.IsBadVector2() || position.IsInTerrariaWorld(0))
            {
                packet.Player.Kick("security.badVec2");
                return true;
            }
        }

        return false;
    }

    public void Unload(NetworkInstance net)
    {
        net.SecureIncoming[13].Remove(OnPlayerUpdate);
    }
}
