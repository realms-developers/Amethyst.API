using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Suspension;
using Amethyst.Systems.Users.Players.Suspension;

namespace Amethyst.Systems.Users.Players.Suspension;

public sealed class PlayerSuspensionBuilder : IProviderBuilder<ISuspensionProvider>
{
    public ISuspensionProvider BuildFor(IAmethystUser user)
    {
        return user is not PlayerUser
            ? throw new ArgumentException("User is not a PlayerUser", nameof(user))
            : (ISuspensionProvider)new PlayerSuspensionProvider(user);
    }
}
