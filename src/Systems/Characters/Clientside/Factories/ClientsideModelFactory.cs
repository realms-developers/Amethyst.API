using Amethyst.Server.Entities.Players;
using Amethyst.Systems.Characters.Base;
using Amethyst.Systems.Characters.Base.Factories;
using Amethyst.Systems.Characters.Utilities;

namespace Amethyst.Systems.Characters.Clientside.Factories;

public sealed class ClientsideModelFactory : IDefaultModelFactory
{
    public ICharacterModel CreateModel(PlayerEntity player)
    {
        var model = new EmptyCharacterModel();
        model.Name = player.Name;
        return model;
    }
}
