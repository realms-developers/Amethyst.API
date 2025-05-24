using System.Collections;
using Amethyst.Server.Entities.Base;

namespace Amethyst.Server.Entities.Players.Tracking;

public sealed class PlayerTracker : IEntityTracker<PlayerEntity>
{
    public PlayerTracker()
    {
        Manager = new PlayerManager(this);
        Manager.AttachHooks();
    }

    public PlayerEntity this[int index] => _players[index];

    public IEntityManager<PlayerEntity>? Manager { get; }

    internal PlayerEntity[] _players = new PlayerEntity[255];

    public IEnumerator<PlayerEntity> GetEnumerator() => _players.Where(e => e != null).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
