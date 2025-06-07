using Amethyst.Server.Entities.Players;

namespace Amethyst.Systems.Characters.Base.Factories;

public interface IDefaultModelFactory
{
    ICharacterModel CreateModel(PlayerEntity player);
}
