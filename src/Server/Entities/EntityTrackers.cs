using Amethyst.Server.Entities.Base;
using Amethyst.Server.Entities.Players;
using Amethyst.Server.Entities.Players.Tracking;

namespace Amethyst.Server.Entities;

public static class EntityTrackers
{
    public static IEntityTracker<PlayerEntity> Players { get; } = new PlayerTracker();
}
