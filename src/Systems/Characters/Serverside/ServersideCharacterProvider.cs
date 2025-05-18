using Amethyst.Systems.Characters.Base;
using Amethyst.Systems.Characters.Base.Enums;
using Amethyst.Systems.Characters.Base.Interactions;
using Amethyst.Systems.Characters.Utilities;
using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Players;
using Terraria;

namespace Amethyst.Systems.Characters.Serverside;

public sealed class ServersideCharacterProvider : ICharacterProvider
{
    public ServersideCharacterProvider(IAmethystUser user)
    {
        User = user;

        _model = new EmptyCharacterModel();
    }

    public IAmethystUser User { get; }

    public bool CanSaveModel => true;

    public ICharacterModel CurrentModel => _model;

    public ICharacterHandler Handler { get; set; } = null!;

    public ICharacterEditor Editor { get; set; } = null!;

    public ICharacterSynchroniser Synchronizer { get; set; } = null!;

    private ICharacterModel _model = null!;

    public void LoadModel(ICharacterModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        _model = model;

        NetMessage.SendData(7, ((PlayerUser)User).NetworkIndex);

        for (int i = 0; i < _model.Slots.Length; i++)
        {
            Synchronizer.SyncSlot(SyncType.Broadcast, i);
        }

        Synchronizer.SyncPlayerInfo(SyncType.Broadcast);
        Synchronizer.SyncQuests(SyncType.Broadcast);
        Synchronizer.SyncLife(SyncType.Broadcast);
        Synchronizer.SyncMana(SyncType.Broadcast);
    }

    public void Save()
    {
        if (CanSaveModel)
        {
            _model.Save();
        }
    }
}
