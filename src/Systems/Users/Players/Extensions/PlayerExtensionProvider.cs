using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Extensions;

namespace Amethyst.Systems.Users.Players.Extensions;

public sealed class PlayerExtensionsProvider : IExtensionProvider
{
    private Dictionary<string, IUserExtension> _extensions = new();

    public void AddExtension(IUserExtension extension)
    {
        if (_extensions.ContainsKey(extension.Name))
            throw new ArgumentException($"Extension with name {extension.Name} already exists.");

        _extensions.Add(extension.Name, extension);
    }

    public void RemoveExtension(IUserExtension extension)
    {
        if (!_extensions.ContainsKey(extension.Name))
            throw new ArgumentException($"Extension with name {extension.Name} does not exist.");

        _extensions.Remove(extension.Name);
    }

    public IUserExtension? GetExtension(string name)
    {
        _extensions.TryGetValue(name, out var extension);
        return extension;
    }

    public IEnumerable<IUserExtension> GetAllExtensions()
    {
        return _extensions.Values;
    }

    public void LoadAll(IAmethystUser user)
    {
        foreach (var extension in _extensions.Values)
        {
            extension.Load(user);
        }
    }

    public void UnloadAll(IAmethystUser user)
    {
        foreach (var extension in _extensions.Values)
        {
            extension.Unload(user);
        }
    }
}
