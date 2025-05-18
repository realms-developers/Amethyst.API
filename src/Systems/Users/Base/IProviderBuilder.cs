namespace Amethyst.Systems.Users.Base;

public interface IProviderBuilder<TProvider>
{
    TProvider BuildFor(IAmethystUser user);
}
