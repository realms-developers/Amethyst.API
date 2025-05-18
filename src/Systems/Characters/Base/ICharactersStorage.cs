namespace Amethyst.Systems.Characters.Base;

public interface ICharactersStorage
{
    ICharacterModel CreateModel(string name);

    void SaveModel(ICharacterModel character);
    void RemoveModel(ICharacterModel character);

    ICharacterModel? GetModel(string name);
}
