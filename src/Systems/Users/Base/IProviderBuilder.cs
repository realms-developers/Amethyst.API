using Amethyst.Systems.Users.Base;

namespace Amethyst.Systems.Base.Users;

public interface IProviderBuilder<TProvider>
{
    TProvider BuildFor(IAmethystUser user);
}
