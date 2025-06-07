namespace Amethyst.Network.Handling.Base;

public interface INetworkHandler
{
    string Name { get; }

    void Load();
    void Unload();
}
