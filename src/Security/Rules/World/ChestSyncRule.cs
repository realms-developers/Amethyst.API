using Amethyst.Network;
using Amethyst.Network.Managing;
using Amethyst.Network.Packets;
using Amethyst.Players;
using Terraria;

namespace Amethyst.Security.Rules.World;

public sealed class ChestSyncRule : ISecurityRule
{
    public string Name => "coresec_wldChestSync";

    public void Load(NetworkInstance net)
    {
        net.SecureIncoming[32].Add(OnChestSync);
    }

    private bool OnChestSync(in IncomingPacket packet)
    {
        BinaryReader reader = packet.GetReader();

        if (packet.Player.Jail.IsJailed)
        {
            return true;
        }

        int index = reader.ReadInt16();
        int x = reader.ReadInt16();
        int y = reader.ReadInt16();
        int action = reader.ReadByte();
        string name = string.Empty;
        if (action != 0 && action <= 20)
        {
            name = reader.ReadString();

            if (SecurityManager.Configuration.DisableChestNameFilter == true)
            {
                foreach (char character in name.ToLowerInvariant())
                {
                    if (!_nameFilter.Contains(character))
                    {
                        packet.Player.Kick("security.invalidSymbols", [character]);
                        return true;
                    }
                }
            }
        }

        if (index < 0 || index >= 8000 || name.Length > 20)
        {
            return true;
        }

        Chest? chest = Main.chest[index];

        if (packet.Player.TPlayer.chest != -1 && index == -1)
        {
            return true;
        }

        if (chest == null || packet.Player.TPlayer.chest != index || !packet.Player.Utils.InCenteredCube(chest.x, chest.y, 32))
        {
            return true;
        }

        return false;
    }

    private readonly string _nameFilter = " ~!@#$%^&*()_+`1234567890-=ё\"№;:?\\|qwertyuiopasdfghjklzxcvbnm{}[];'<>,./ёйцукенгшщзхъфывапролджэячсмитьбю";

    public void Unload(NetworkInstance net)
    {
        net.SecureIncoming[33].Remove(OnChestSync);
    }
}
