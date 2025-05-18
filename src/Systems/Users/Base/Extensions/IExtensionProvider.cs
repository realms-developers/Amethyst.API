namespace Amethyst.Systems.Users.Base.Extensions;

public interface IExtensionProvider
{
    void AddExtension(IUserExtension extension);
    void RemoveExtension(IUserExtension extension);

    IUserExtension? GetExtension(string name);
    IEnumerable<IUserExtension> GetAllExtensions();

    void LoadAll(IAmethystUser user);
    void UnloadAll(IAmethystUser user);
}
