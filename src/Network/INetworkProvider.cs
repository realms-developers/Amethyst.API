using Amethyst.Network.Managing;

namespace Amethyst.Network;

public interface INetworkProvider
{
    public string Name { get; }

    public void Initialize(NetworkInstance instance);

    public void StartListening();

    public void Broadcast(byte[] packet);
    public void Broadcast(byte[] packet, Predicate<INetworkClient> predicate);
    public void Broadcast(byte[] packet, params int[] ignored);

    public INetworkClient? GetClient(int index);
}
