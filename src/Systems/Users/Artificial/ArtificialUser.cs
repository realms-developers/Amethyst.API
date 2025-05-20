using Amethyst.Systems.Characters.Base;
using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Extensions;
using Amethyst.Systems.Users.Base.Messages;
using Amethyst.Systems.Users.Base.Permissions;

namespace Amethyst.Systems.Users.Artificial;

public sealed class ArtificialUser : IAmethystUser
{
    public ArtificialUser(string name, IProviderBuilder<IMessageProvider> messageBuilder,
        IProviderBuilder<IPermissionProvider> permissionBuilder,
        IProviderBuilder<IExtensionProvider> extensionBuilder)
    {
        Name = name;
        MessageProvider = messageBuilder.BuildFor(this);
        Permissions = permissionBuilder.BuildFor(this);
        Extensions = extensionBuilder.BuildFor(this);
    }


    public string Name { get; }

    public IMessageProvider MessageProvider { get; }

    public IPermissionProvider Permissions { get; }

    public IExtensionProvider Extensions { get; }

    public ICharacterProvider? Character { get; }
}
