using Amethyst.Infrastructure;
using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Messages;

namespace Amethyst.Systems.Users.Artificial.Messages;

public sealed class ArtificialMessageBuilder : IProviderBuilder<IMessageProvider>
{
    public IMessageProvider BuildFor(IAmethystUser user)
    {
        return user is not ArtificialUser artUser
            ? throw new ArgumentException("User is not a ArtificialUser", nameof(user))
            : (IMessageProvider)new ArtificialMessageProvider(artUser, AmethystSession.Profile.DefaultLanguage);
    }
}
