using Amethyst.Systems.Users.Base.Permissions;
using Amethyst.Systems.Users.Base.Messages;
using Amethyst.Systems.Users.Base.Extensions;
using Amethyst.Gameplay.Players.SSC.Interfaces;

namespace Amethyst.Systems.Users.Base;

public interface IAmethystUser
{
    string Name { get; }

    IMessageProvider MessageProvider { get; }

    IPermissionProvider Permissions { get; }

    IExtensionProvider Extensions { get; }

    bool IsSSCProvided { get; }
    ISSCProvider? SSC { get; }
}
