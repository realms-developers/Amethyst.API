using Amethyst.Systems.Characters.Base.Factories;
using Amethyst.Systems.Characters.Serverside;
using Amethyst.Systems.Characters.Serverside.Factories;

namespace Amethyst.Systems.Characters;

public static class CharactersOrganizer
{
    public static ICharacterFactory<ServersideCharacterProvider> ServersideFactory { get; } = new ServersideCharacterFactory();
}
