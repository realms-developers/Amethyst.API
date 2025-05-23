namespace Amethyst.Gameplay.Players.Extensions;

public interface IPlayerExtensionBuilder<T> where T : IPlayerExtension
{
    public void Initialize();
    public T Build(NetPlayer player);
}