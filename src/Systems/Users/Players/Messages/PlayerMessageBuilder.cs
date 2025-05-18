using Amethyst.Infrastructure;
using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Messages;

namespace Amethyst.Systems.Users.Players.Messages;

public sealed class PlayerMessageBuilder : IProviderBuilder<IMessageProvider>
{
    public IMessageProvider BuildFor(IAmethystUser user)
    {
        if (user is not PlayerUser plrUser)
            throw new ArgumentException("User is not a PlayerUser", nameof(user));

        return new PlayerMessageProvider(plrUser, AmethystSession.Profile.DefaultLanguage);
    }
}
