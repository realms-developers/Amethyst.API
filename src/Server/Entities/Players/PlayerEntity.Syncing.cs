using Amethyst.Server.Entities.Base;
using Amethyst.Server.Entities.Players.Syncing;

namespace Amethyst.Server.Entities.Players;

public sealed partial class PlayerEntity : IServerEntity
{
    public void SyncTo(PlayerEntity player)
    {
        foreach (byte[]? packet in PlayerSyncing.CreateSyncPackets(this))
        {
            if (packet == null)
                continue;

            player.SendPacketBytes(packet);
        }
    }

    public void SyncToAll()
    {
        foreach (PlayerEntity player in EntityTrackers.Players)
        {
            if (player.Index == Index)
                continue;

            SyncTo(player);
        }
    }

    public void SyncFromAll()
    {
        foreach (PlayerEntity player in EntityTrackers.Players)
        {
            if (player.Index == Index)
                continue;

            player.SyncTo(this);
        }
    }
}
