using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Extensions;

namespace Amethyst.Systems.Users.Artificial.Extensions;

public sealed class ArtificialExtensionBuilder : IProviderBuilder<IExtensionProvider>
{
    public IExtensionProvider BuildFor(IAmethystUser user)
    {
        return user is not ArtificialUser
            ? throw new ArgumentException("User is not a ArtificialUser", nameof(user))
            : (IExtensionProvider)new ArtificialExtensionsProvider();
    }
}
