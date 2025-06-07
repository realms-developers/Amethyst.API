using Amethyst.Systems.Characters.Base.Interactions;
using Amethyst.Systems.Users.Base;

namespace Amethyst.Systems.Characters.Base;

public interface ICharacterProvider
{
    public IAmethystUser User { get; }

    public bool CanSaveModel { get; }

    public ICharacterModel CurrentModel { get; }

    public ICharacterHandler Handler { get; set; }

    public ICharacterEditor Editor { get; set; }

    public ICharacterSynchroniser Synchronizer { get; set; }

    public int LoadoutIndex { get; set; }

    public void LoadModel(ICharacterModel model);

    public void Save();
}
