namespace Amethyst.Gameplay.Players.SSC.Interfaces;

public interface ISSCProvider
{
    public ICharacterWrapper CreateServersideWrapper(NetPlayer player);
    public CharacterModel GetModel(string name);
}
