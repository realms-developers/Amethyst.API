namespace Amethyst.Systems.Characters.Base.Factories;

public interface IInteractionFactory<TProvider>
{
    TProvider BuildFor(ICharacterProvider provider);
}
