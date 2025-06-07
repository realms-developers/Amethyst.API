using Amethyst.Systems.Characters.Base;
using Amethyst.Systems.Characters.Base.Interactions;
using Amethyst.Systems.Characters.Utilities;
using Amethyst.Systems.Users.Base;

namespace Amethyst.Systems.Characters.Clientside;

public sealed class ClientsideCharacterProvider : ICharacterProvider
{
    public ClientsideCharacterProvider(IAmethystUser user)
    {
        User = user;

        _model = new EmptyCharacterModel();
    }

    public IAmethystUser User { get; }

    public bool CanSaveModel => false;

    public ICharacterModel CurrentModel => _model;

    private ICharacterModel _model;

    public ICharacterHandler Handler { get; set; } = null!;

    public ICharacterEditor Editor { get; set; } = null!;

    public ICharacterSynchroniser Synchronizer { get; set; } = null!;

    public int LoadoutIndex { get; set; }

    public void LoadModel(ICharacterModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        _model = model;
    }

    public void Save()
    {
        throw new NotSupportedException("Saving is not supported in ClientsideCharacterProvider.");
    }
}
