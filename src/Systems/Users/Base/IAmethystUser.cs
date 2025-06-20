using Amethyst.Systems.Users.Base.Permissions;
using Amethyst.Systems.Users.Base.Messages;
using Amethyst.Systems.Users.Base.Extensions;
using Amethyst.Systems.Characters.Base;
using Amethyst.Systems.Users.Base.Suspension;
using Amethyst.Systems.Users.Base.Commands;
using Amethyst.Systems.Users.Base.Requests;

namespace Amethyst.Systems.Users.Base;

public interface IAmethystUser
{
    string Name { get; }

    IMessageProvider Messages { get; }

    IPermissionProvider Permissions { get; }

    IExtensionProvider Extensions { get; }

    ICommandProvider Commands { get; }

    IRequestProvider Requests { get; }

    ICharacterProvider? Character { get; }

    ISuspensionProvider? Suspensions { get; }
}
