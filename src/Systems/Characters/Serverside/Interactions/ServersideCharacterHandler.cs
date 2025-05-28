using Amethyst.Network.Packets;
using Amethyst.Server.Entities.Players;
using Amethyst.Systems.Characters.Base;
using Amethyst.Systems.Characters.Base.Interactions;
using Amethyst.Systems.Users.Players;
using Terraria;

namespace Amethyst.Systems.Characters.Serverside.Interactions;

public sealed class ServersideCharacterHandler : ICharacterHandler
{
    public ServersideCharacterHandler(ICharacterProvider provider)
    {
        Provider = provider;

        if (provider.User is not PlayerUser)
            throw new InvalidOperationException("Provider user is not a PlayerUser.");

        PlayerUser user = (PlayerUser)provider.User;

        Player = user.Player;
        TPlayer = user.Player.TPlayer;
    }

    public ICharacterProvider Provider { get; }

    public PlayerEntity Player { get; }
    public Player TPlayer { get; }

    public bool InReadonlyMode { get; set; }

    public void HandlePlayerInfo(IncomingPacket packet)
    {
        throw new NotImplementedException();
    }

    public void HandleQuests(IncomingPacket packet)
    {
        throw new NotImplementedException();
    }

    public void HandleSetLife(IncomingPacket packet)
    {
        throw new NotImplementedException();
    }

    public void HandleSetMana(IncomingPacket packet)
    {
        throw new NotImplementedException();
    }

    public void HandleSlot(IncomingPacket packet)
    {
        throw new NotImplementedException();
    }
}
