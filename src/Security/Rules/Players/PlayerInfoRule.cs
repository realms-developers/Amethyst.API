using Amethyst.Network.Managing;
using Amethyst.Network.Packets;
using Amethyst.Players;
using Amethyst.Players.SSC.Enums;

namespace Amethyst.Security.Rules.Players;

public sealed class PlayerInfoRule : ISecurityRule
{
    public string Name => "coresec_playerInfo";

    public void Load(NetworkInstance net)
    {
        net.SecureIncoming[4].Add(OnPlayerInfo);
    }

    private bool OnPlayerInfo(in IncomingPacket packet)
    {
        var reader = packet.GetReader();
        reader.BaseStream.Position += 3;

        string name = reader.ReadString();
        if (SecurityManager.Configuration.EnableNicknameFilter == true)
        {
            foreach (char character in name)
            {
                if (!SecurityManager.Configuration.NicknameFilter.Contains(character))
                {
                    packet.Player.Kick("security.invalidSymbols", [ character ]);
                    return true;
                }
            }
        }

        if (PlayerManager.IsSSCEnabled && packet.Player.Jail.IsJailed)
        {
            packet.Player.Character?.SyncPlayerInfo(SyncType.Local);
            return true;
        }

        return false;
    }

    public void Unload(NetworkInstance net)
    {
        net.SecureIncoming[4].Remove(OnPlayerInfo);
    }
}
