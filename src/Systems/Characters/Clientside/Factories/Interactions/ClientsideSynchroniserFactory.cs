using Amethyst.Systems.Characters.Base;
using Amethyst.Systems.Characters.Base.Factories;
using Amethyst.Systems.Characters.Base.Interactions;
using Amethyst.Systems.Characters.Clientside.Interactions;

namespace Amethyst.Systems.Characters.Clientside.Factories.Interactions;

public sealed class ClientsideSynchroniserFactory : IInteractionFactory<ICharacterSynchroniser>
{
    public ICharacterSynchroniser BuildFor(ICharacterProvider provider)
    {
        return new ClientsideCharacterSynchroniser(provider);
    }
}
