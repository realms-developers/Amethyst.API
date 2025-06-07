using Amethyst.Systems.Characters.Base.Factories;
using Amethyst.Systems.Characters.Clientside;
using Amethyst.Systems.Characters.Clientside.Factories;
using Amethyst.Systems.Characters.Serverside;
using Amethyst.Systems.Characters.Serverside.Factories;

namespace Amethyst.Systems.Characters;

public static class CharactersOrganizer
{
    public static ICharacterFactory<ServersideCharacterProvider> ServersideFactory { get; } = new ServersideCharacterFactory();
    public static ICharacterFactory<ClientsideCharacterProvider> ClientsideFactory { get; } = new ClientsideCharacterFactory();
}
