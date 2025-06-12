using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Suspension;

namespace Amethyst.Systems.Users.Players.Suspension;

public sealed class PlayerSuspensionProvider(IAmethystUser user) : ISuspensionProvider
{
    public IAmethystUser User { get; } = user;

    public bool IsSuspended => _suspensions.Any(s => s.IsSuspended(User));

    public IReadOnlyList<ISuspension> Suspensions => _suspensions.AsReadOnly();

    private readonly List<ISuspension> _suspensions = new();

    public void Suspend(ISuspension suspension)
    {
        ArgumentNullException.ThrowIfNull(suspension);

        if (_suspensions.Remove(suspension))
        {
            AmethystLog.System.Debug("Suspensions", $"Suspension {suspension.Name} already exists for user {User.Name}. Replacing it.");
        }

        _suspensions.Add(suspension);
    }

    public void Unsuspend(ISuspension suspension)
    {
        ArgumentNullException.ThrowIfNull(suspension);

        if (!_suspensions.Remove(suspension))
        {
            AmethystLog.System.Debug("Suspensions", $"Suspension {suspension.Name} does not exist for user {User.Name}. Cannot remove it.");
        }
    }
}
