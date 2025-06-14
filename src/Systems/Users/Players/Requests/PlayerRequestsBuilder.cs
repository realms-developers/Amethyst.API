using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Requests;
using Amethyst.Systems.Users.Common.Requests;

namespace Amethyst.Systems.Users.Players.Requests;

public sealed class PlayerRequestsBuilder : IProviderBuilder<IRequestProvider>
{
    public IRequestProvider BuildFor(IAmethystUser user)
    {
        return new CommonRequestProvider();
    }
}
