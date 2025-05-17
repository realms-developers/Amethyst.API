namespace Amethyst.Extensions.Repositories;

public interface IRepositorySet
{
    bool AddRepository(IExtensionRepository repository);
    bool RemoveRepository(IExtensionRepository repository);

    IExtensionRepository GetRepository(string name);

    IEnumerable<IExtensionRepository> GetRepositories();
}
