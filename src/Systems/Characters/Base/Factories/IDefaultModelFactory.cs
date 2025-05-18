using Amethyst.Gameplay.Players;

namespace Amethyst.Systems.Characters.Base.Factories;

public interface IDefaultModelFactory
{
    ICharacterModel CreateModel(NetPlayer player);
}
