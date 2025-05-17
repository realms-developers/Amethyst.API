namespace Amethyst.Extensions.Repositories;

public interface IRepositoryRuler
{
    bool AllowExtension(string name);
    bool DisallowExtension(string name);

    IEnumerable<string> GetAllowedExtensions();

    bool IsExtensionAllowed(string name);
}
