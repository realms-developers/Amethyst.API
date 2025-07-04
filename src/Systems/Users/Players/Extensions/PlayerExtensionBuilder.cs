using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Extensions;

namespace Amethyst.Systems.Users.Players.Extensions;

public sealed class PlayerExtensionBuilder : IProviderBuilder<IExtensionProvider>
{
    public IExtensionProvider BuildFor(IAmethystUser user)
    {
        return user is not PlayerUser
            ? throw new ArgumentException("User is not a PlayerUser", nameof(user))
            : (IExtensionProvider)new PlayerExtensionsProvider();
    }
}
