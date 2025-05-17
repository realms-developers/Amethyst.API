namespace Amethyst.Extensions.Repositories;

public interface IRepositoryRuler
{
    bool AllowExtension(string name);
    bool DisallowExtension(string name);

    IEnumerable<string> GetAllowedExtensions();
    IEnumerable<string> GetDisallowedExtensions();

    bool IsExtensionAllowed(string name);
}
