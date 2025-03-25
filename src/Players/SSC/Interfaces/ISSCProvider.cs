namespace Amethyst.Players.SSC.Interfaces;

public interface ISSCProvider
{
    public void Initialize();

    public ICharacterWrapper CreateServersideWrapper(NetPlayer player);
    public CharacterModel GetModel(string name);
}