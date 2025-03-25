namespace Amethyst.Players.Extensions;

public interface IPlayerExtension
{
    public NetPlayer Player { get; }

    public void Load();
    public void Unload();
}