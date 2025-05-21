using Amethyst.Network.Managing;

namespace Amethyst.Security;

public interface ISecurityRule
{
    public string Name { get; }

    public void Load(NetworkInstance net);
    public void Unload(NetworkInstance net);
}
