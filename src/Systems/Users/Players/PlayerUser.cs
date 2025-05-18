using Amethyst.Gameplay.Players;
using Amethyst.Gameplay.Players.SSC.Interfaces;
using Amethyst.Systems.Characters.Base;
using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Extensions;
using Amethyst.Systems.Users.Base.Messages;
using Amethyst.Systems.Users.Base.Permissions;

namespace Amethyst.Systems.Users.Players;

public sealed class PlayerUser : IAmethystUser
{
    internal PlayerUser(string name, int netIndex, string ip, string uuid, IUsersService<PlayerUser, PlayerUserMetadata> service)
    {
        Name = name;
        NetworkIndex = netIndex;
        IP = ip;
        UUID = uuid;
        MessageProvider = service.MessageProviderBuilder.BuildFor(this);
        Permissions = service.PermissionProviderBuilder.BuildFor(this);
        Extensions = service.ExtensionProviderBuilder.BuildFor(this);
    }

    public string Name { get; }

    public IMessageProvider MessageProvider { get; }

    public IPermissionProvider Permissions { get; }

    public IExtensionProvider Extensions { get; }

    public ICharacterProvider? Character { get; }

    public int NetworkIndex { get; }

    public NetPlayer Player => PlayerManager.Tracker[NetworkIndex];

    public string IP { get; set; }

    public string UUID { get; set; }
}
