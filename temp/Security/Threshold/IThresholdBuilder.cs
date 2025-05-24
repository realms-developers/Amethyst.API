using Amethyst.Security.Limits;

namespace Amethyst.Security.Threshold;

public interface IThresholdBuilder<T> where T : Enum
{
    IThreshold<T> Build();
}
