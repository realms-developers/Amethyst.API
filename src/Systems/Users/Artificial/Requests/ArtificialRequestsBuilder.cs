using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Requests;
using Amethyst.Systems.Users.Common.Requests;

namespace Amethyst.Systems.Users.Artificial.Requests;

public sealed class ArtificialRequestsBuilder : IProviderBuilder<IRequestProvider>
{
    public IRequestProvider BuildFor(IAmethystUser user)
    {
        return new CommonRequestProvider();
    }
}
