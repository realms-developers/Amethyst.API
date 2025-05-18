using Amethyst.Gameplay.Players.SSC.Interfaces;
using Amethyst.Systems.Base.Users;
using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Extensions;
using Amethyst.Systems.Users.Base.Messages;
using Amethyst.Systems.Users.Base.Permissions;

namespace Amethyst.Systems.Users.Players;

public sealed class PlayerUser : IAmethystUser
{
    internal PlayerUser(string name, IMessageProvider messageProvider, IPermissionProvider permissions, IExtensionProvider extensions, bool isSSCProvided, ISSCProvider? ssc)
    {
        Name = name;
        MessageProvider = messageProvider;
        Permissions = permissions;
        Extensions = extensions;
        IsSSCProvided = isSSCProvided;
        SSC = ssc;
    }

    public string Name { get;  }

    public IMessageProvider MessageProvider { get;  }

    public IPermissionProvider Permissions { get;  }

    public IExtensionProvider Extensions { get;  }

    public bool IsSSCProvided { get;  }

    public ISSCProvider? SSC { get;  }
}
