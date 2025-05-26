using Amethyst.Server.Entities;
using Amethyst.Server.Entities.Players;
using Amethyst.Systems.Characters.Base;
using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Extensions;
using Amethyst.Systems.Users.Base.Messages;
using Amethyst.Systems.Users.Base.Permissions;
using Amethyst.Systems.Users.Base.Suspension;

namespace Amethyst.Systems.Users.Players;

public sealed class PlayerUser : IAmethystUser
{
    internal PlayerUser(string name, int netIndex, string ip, string uuid, IProviderBuilder<IMessageProvider> messageBuilder,
        IProviderBuilder<IPermissionProvider> permissionBuilder,
        IProviderBuilder<IExtensionProvider> extensionBuilder,
        IProviderBuilder<ISuspensionProvider>? suspensionBuilder = null)
    {
        Name = name;
        NetworkIndex = netIndex;
        IP = ip;
        UUID = uuid;
        Messages = messageBuilder.BuildFor(this);
        Permissions = permissionBuilder.BuildFor(this);
        Extensions = extensionBuilder.BuildFor(this);
        Suspensions = suspensionBuilder?.BuildFor(this);
    }

    public string Name { get; }

    public IMessageProvider Messages { get; }

    public IPermissionProvider Permissions { get; }

    public IExtensionProvider Extensions { get; }

    public ICharacterProvider? Character { get; }

    public ISuspensionProvider? Suspensions { get; }

    public int NetworkIndex { get; }

    public PlayerEntity Player => EntityTrackers.Players[NetworkIndex];

    public string IP { get; set; }

    public string UUID { get; set; }
}
