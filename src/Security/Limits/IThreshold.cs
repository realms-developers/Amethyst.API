namespace Amethyst.Security;

public interface IThreshold : IDisposable
{
    public void Setup(int index, int max);
    public bool Fire(int index);
}
