namespace Amethyst.Extensions;

public interface IExtensionHandler<T> where T : IExtension
{
    bool SupportsUnload { get; }

    void Load();
    void Unload();
}
