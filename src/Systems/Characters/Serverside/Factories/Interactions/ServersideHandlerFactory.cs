using Amethyst.Systems.Characters.Base;
using Amethyst.Systems.Characters.Base.Factories;
using Amethyst.Systems.Characters.Base.Interactions;
using Amethyst.Systems.Characters.Serverside.Interactions;

namespace Amethyst.Systems.Characters.Serverside.Factories.Interactions;

public sealed class ServersideHandlerFactory : IInteractionFactory<ICharacterHandler>
{
    public ICharacterHandler BuildFor(ICharacterProvider provider)
    {
        return new ServersideCharacterHandler(provider);
    }
}
