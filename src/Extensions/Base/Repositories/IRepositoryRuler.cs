namespace Amethyst.Extensions.Base.Repositories;

public interface IRepositoryRuler
{
    void AllowExtension(string name);
    bool DisallowExtension(string name);

    IEnumerable<string> AllowedExtensions { get; }

    bool IsExtensionAllowed(string name);
}
