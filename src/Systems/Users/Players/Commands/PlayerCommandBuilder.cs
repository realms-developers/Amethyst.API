using Amethyst.Kernel;
using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Commands;
using Amethyst.Systems.Users.Common.Commands;

namespace Amethyst.Systems.Users.Players.Commands;

public sealed class PlayerCommandBuilder : IProviderBuilder<ICommandProvider>
{
    public ICommandProvider BuildFor(IAmethystUser user)
    {
        if (user is not PlayerUser playerUser)
        {
            throw new InvalidOperationException("User must be a PlayerUser to build a command provider.");
        }

        return new CommonCommandProvider(playerUser, 500, AmethystSession.Profile.DebugMode ? ["shared", "debug"] : ["shared"]);
    }
}
