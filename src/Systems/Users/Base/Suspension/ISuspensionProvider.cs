namespace Amethyst.Systems.Users.Base.Suspension;

public interface ISuspensionProvider
{
    IAmethystUser User { get; }

    bool IsSuspended { get; }

    IReadOnlyList<ISuspension> Suspensions { get; }

    void Suspend(ISuspension suspension);
    void Unsuspend(ISuspension suspension);
}
