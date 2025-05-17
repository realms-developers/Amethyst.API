namespace Amethyst.Extensions.Repositories;

public sealed class RepositorySet
{
    private readonly List<IExtensionRepository> _repositories = new();

    public IReadOnlyList<IExtensionRepository> Repositories => _repositories.AsReadOnly();

    public bool AddRepository(IExtensionRepository repository)
    {
        if (_repositories.Contains(repository))
            return false;

        _repositories.Add(repository);
        return true;
    }

    public bool RemoveRepository(IExtensionRepository repository)
    {
        if (!_repositories.Contains(repository))
            return false;

        _repositories.Remove(repository);
        return true;
    }

    public IExtensionRepository? GetRepository(string name)
    {
        foreach (IExtensionRepository repository in _repositories)
        {
            if (repository.Name == name)
                return repository;
        }

        return null;
    }
}
