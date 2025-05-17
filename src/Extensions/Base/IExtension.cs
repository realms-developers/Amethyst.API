using Amethyst.Extensions.Repositories;

namespace Amethyst.Extensions;

public interface IExtension
{
    public IExtensionRepository Repository { get; set; }
    public IExtensionHandler<IExtension> Handler { get; set; }
}
