using Amethyst.Systems.Characters.Base.Interactions;
using Amethyst.Systems.Users.Base;

namespace Amethyst.Systems.Characters.Base.Factories;

public interface ICharacterFactory<TCharacter> where TCharacter : ICharacterProvider
{
    public IInteractionFactory<ICharacterEditor> EditorFactory { get; set; }
    public IInteractionFactory<ICharacterHandler> HandlerFactory { get; set; }
    public IInteractionFactory<ICharacterSynchroniser> SynchronizerFactory { get; set;  }

    public ICharactersStorage Storage { get; set; }

    public IDefaultModelFactory ModelFactory { get; set; }

    TCharacter BuildFor(IAmethystUser user);
}
