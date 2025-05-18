namespace Amethyst.Security.Threshold;

public interface IThreshold<T> where T : Enum
{
    bool Fire(T index);
}
