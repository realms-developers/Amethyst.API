using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Commands;
using Amethyst.Systems.Users.Common.Commands;

namespace Amethyst.Systems.Users.Artificial.Commands;

public sealed class ArtificialCommandBuilder : IProviderBuilder<ICommandProvider>
{
    public ICommandProvider BuildFor(IAmethystUser user)
    {
        if (user is not ArtificialUser ArtificialUser)
        {
            throw new InvalidOperationException("User must be a ArtificialUser to build a command provider.");
        }

        return new CommonCommandProvider(ArtificialUser, 0, ["shared"]);
    }
}
