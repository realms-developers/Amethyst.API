using Amethyst.Network;
using Amethyst.Server.Entities.Players;
using Amethyst.Systems.Characters.Base;
using Amethyst.Systems.Characters.Base.Enums;
using Amethyst.Systems.Characters.Base.Interactions;
using Amethyst.Systems.Characters.Utilities;
using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Players;

namespace Amethyst.Systems.Characters.Serverside;

public sealed class ServersideCharacterProvider : ICharacterProvider
{
    public ServersideCharacterProvider(IAmethystUser user)
    {
        User = user;

        if (user is not PlayerUser playerUser)
        {
            throw new ArgumentException("User must be a player user.", nameof(user));
        }
        Player = playerUser.Player;

        _model = new EmptyCharacterModel();
    }

    public IAmethystUser User { get; }

    public PlayerEntity Player { get; }

    public bool CanSaveModel => true;

    public ICharacterModel CurrentModel => _model;

    public ICharacterHandler Handler { get; set; } = null!;

    public ICharacterEditor Editor { get; set; } = null!;

    public ICharacterSynchroniser Synchronizer { get; set; } = null!;

    public int LoadoutIndex { get; set; }

    private ICharacterModel _model = null!;

    public void LoadModel(ICharacterModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        _model = model;

        byte[] packet = PacketSendingUtility.CreateWorldInfoPacket();
        Player.SendPacketBytes(packet);

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
