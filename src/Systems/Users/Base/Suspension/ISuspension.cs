namespace Amethyst.Systems.Users.Base.Suspension;

public interface ISuspension
{
    string Name { get; }

    bool IsSuspended(IAmethystUser user);
}
