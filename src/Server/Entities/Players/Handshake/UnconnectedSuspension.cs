using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Suspension;
using Amethyst.Systems.Users.Players;

namespace Amethyst.Server.Entities.Players.Handshake;

public sealed class UnconnectedSuspension : ISuspension
{
    public string Name => "player_unconnected_suspension";

    public bool IsSuspended(IAmethystUser user)
    {
        var plrUser = (PlayerUser)user;
        return plrUser.Player.Phase != ConnectionPhase.Connected;
    }
}
